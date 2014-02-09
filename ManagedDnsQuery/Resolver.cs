/**********************************************************************************
 ==================================================================================
The MIT License (MIT)

Copyright (c) 2013 Tim Burnett

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 ==================================================================================
 **********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.SPF.Concretes;
using ManagedDnsQuery.SPF.Interfaces;
using ManagedDnsQuery.WHOIS.Concretes;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery
{
    public sealed class Resolver : IResolver
    {
        public bool UseRecursion { get; set; }
        public int TimeOut { get; set; }
        public int Retries { get; set; }
        private IDnsTransport Transport { get; set; }
        private IWhoisTransport WhoisTransport { get; set; }
        private ITLDHandler TldHandler { get; set; }
        private IQueryCache Cache { get; set; }
        private ISpfChecker SpfChecker { get; set; }
        private readonly IPEndPoint _defaultServer1 = new IPEndPoint(IPAddress.Parse("208.67.222.222"), 53);
        private readonly IPEndPoint _defaultServer2 = new IPEndPoint(IPAddress.Parse("208.67.220.220"), 53);

        public Resolver()
        {
            Transport = new UdpDnsTransport();
            Cache = new QueryCache();
            Retries = 3;
            TimeOut = 60;
            UseRecursion = true;
            WhoisTransport = new WhoisTcpTransport();
            TldHandler = new TldHandler();
            SpfChecker = new SpfChecker();
        }

        public Resolver(IDnsTransport transport, IQueryCache cache = null, IWhoisTransport wtransport = null, ITLDHandler tldHandler = null, ISpfChecker checker = null, int retrys = 3, int timeout = 60, bool useRecursion = true)
        {
            Transport = transport;
            SpfChecker = (checker ?? new SpfChecker());
            Retries = retrys;
            TimeOut = timeout;
            UseRecursion = useRecursion;
            Cache = (cache ?? new QueryCache());
            WhoisTransport = (wtransport?? new WhoisTcpTransport());
            TldHandler = (tldHandler?? new TldHandler());
        }

        public DNS.ExternalInterfaces.IMessage Query(string name, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In)
        {
            if (!string.IsNullOrEmpty(name) && name.TrySubstring(name.Length - 1) != ".")
                name = string.Format("{0}.", name);

            if (dnsServer == null)
                dnsServer = _defaultServer1;

            IMessage request = new Message
                                   {
                                       Header = new Header(null)
                                                    {
                                                        OpCode = OpCode.Query,
                                                        QdCount = 1,
                                                        Id = (ushort)(new Random()).Next(),
                                                        Rd = UseRecursion,
                                                    },
                                        Questions = new List<IQuestion>
                                                        {
                                                            new Question(null)
                                                                {
                                                                    QName = (queryType == RecordType.PtrRecord ? name.ToArpa() : name),
                                                                    QType = queryType,
                                                                    QClass = rClass,
                                                                },
                                                        },
                                   };

            var cached = Cache.CheckCache(request.Questions.FirstOrDefault());
            if (cached != null && cached.Answers != null && cached.Answers.Any())
                return cached.GetExternalAnswer();

            var result = Transport.SendRequest(request, dnsServer, TimeOut);

            if (result == null || result.Header == null || result.Header.RCode == ResponseCode.FormErr || result.Header.RCode == ResponseCode.ServFail)
                for (var ndx = 1; ndx < Retries; ++ndx)
                {
                    result = Transport.SendRequest(request, dnsServer, TimeOut);
                    if (result != null && result.Header != null && (result.Header.RCode != ResponseCode.FormErr && result.Header.RCode != ResponseCode.ServFail))
                        ndx = Retries;
                }

            if (result != null)
                Cache.AddCache(result);

            return result != null ? result.GetExternalAnswer() : null;
        }

        public DNS.ExternalInterfaces.IMessage AuthoratativeQuery(string name, string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In)
        {
            if (dnsServer == null)
                dnsServer = _defaultServer1;

            var auth = Query(domain, RecordType.SoaRecord, dnsServer, rClass);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var soaRecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.SoaRecord;
            if (soaRecord == null)
                return null;

            auth = Query(soaRecord.MName, RecordType.ARecord, dnsServer, rClass);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var arecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.ARecord;
            if (arecord != null)
                return Query(name, queryType, new IPEndPoint(arecord.Address, 53), rClass);

            //Support for IP6 when IP4 does not exist
            auth = Query(soaRecord.MName, RecordType.AaaaRecord, dnsServer, rClass);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var aaaarecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.AaaaRecord;

            return aaaarecord == null ? null : Query(name, queryType, new IPEndPoint(aaaarecord.Address, 53), rClass);
        }

        public string QueryWhois(string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
                return string.Empty;

            domainName = domainName.TryTrim().Replace("http://", "").Replace("https://", "");
            var firstResult = WhoisTransport.RunWhoisQuery(domainName, TldHandler.GetTldServer(domainName));

            if(string.IsNullOrEmpty(firstResult))
                return string.Empty;

            var secondResult = WhoisTransport.RunWhoisQuery(domainName, TldHandler.GetWhoisServer(firstResult));
            return !string.IsNullOrEmpty(secondResult) ? secondResult : firstResult;
        }

        public SpfResult VerifySpfRecord(string domain, string ip)
        {
            IPAddress _ip = null;
            if(IPAddress.TryParse(ip, out _ip))
                return SpfResult.NoResult;

            if(string.IsNullOrEmpty(domain.TryTrim()))
                return SpfResult.NoResult;

            var spf = GetSpfRecords(domain); //Recursivly lookup including includes and redirects.
            if (spf == null || !spf.Any())
                return SpfResult.NoResult;

            //verify not sender id
            if (spf.Any(an => !string.Equals(an.Text.TrySubstring(0, 6), "v=spf1", StringComparison.CurrentCultureIgnoreCase)))
                return SpfResult.NoResult; //Not SPF answer

            //break spf records down
            var parts = spf.SelectMany(an => an.Text.Split(' '));

            //verify 8 mechanisms, return fail if something fails.
            foreach (var part in parts)
            {
                if (string.Equals("v=spf1", part.TryTrim(), StringComparison.CurrentCultureIgnoreCase))
                    continue;
                
                //IP Mechanism
                if (string.Equals("ip", part.TrySubstring(0, 2), StringComparison.CurrentCultureIgnoreCase))
                {
                    if (SpfChecker.VerifyIpMechanism(_ip, part.TryTrim()) != SpfResult.Pass)
                        return SpfResult.Fail;
                }

                //A Mechanism

                //MX Mechanism

                //PTR Mechanism

                //EXISTS - A record exists no matter the address.
            }

            return SpfResult.Pass;
        }

        private IEnumerable<DNS.ExternalConcretes.TxtRecord> GetSpfRecords(string domain)
        {
            var records = new List<DNS.ExternalConcretes.TxtRecord>();

            var spf = AuthoratativeQuery(domain, domain, RecordType.TxtRecord, _defaultServer1); //Only query TXT records since SPF was obsoleted in aug 2013
            if (spf.Answers == null || !spf.Answers.Any())
                return records;

            var spfRecords = spf.Answers.Where(an => (an as DNS.ExternalConcretes.TxtRecord) != null).Cast<DNS.ExternalConcretes.TxtRecord>();
            if (!spfRecords.Any())
                return records;

            #region Recursive lookup for redirects and includes
            if (spfRecords.Any(an => an.Text.ToLower().Contains("redirect=")))
            {
                var tempRecs = spfRecords.Where(an => an.Text.ToLower().Contains("redirect="));
                var tempPeices = tempRecs.SelectMany(rec => rec.Text.Split(' ')).Where(txt => txt.ToLower().Contains("redirect="));

                foreach (var part in tempPeices)
                {
                    var scratch = part.Split('=');
                    if(scratch.Length < 2)
                        continue;
                    
                    records.AddRange(GetSpfRecords(scratch.Skip(1).FirstOrDefault()));
                }
            }

            if (spfRecords.Any(an => an.Text.ToLower().Contains("include:")))
            {
                var tempRecs = spfRecords.Where(an => an.Text.ToLower().Contains("include:"));
                var tempPeices = tempRecs.SelectMany(rec => rec.Text.Split(' ')).Where(txt => txt.ToLower().Contains("include:"));

                foreach (var part in tempPeices)
                {
                    var scratch = part.Split(':');
                    if (scratch.Length < 2)
                        continue;

                    records.AddRange(GetSpfRecords(scratch.Skip(1).FirstOrDefault()));
                }
            }
            #endregion

            return records;
        }
    }
}