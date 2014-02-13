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

        public Resolver(IDnsTransport transport, IQueryCache cache = null, IWhoisTransport wtransport = null, ITLDHandler tldHandler = null, ISpfChecker checker = null, int retrys = 3, int timeout = 60)
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

        public DNS.ExternalInterfaces.IMessage Query(string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In)
        {
            if (!string.IsNullOrEmpty(domain) && domain.TrySubstring(domain.Length - 1) != ".")
                domain = string.Format("{0}.", domain);

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
                                                                    QName = (queryType == RecordType.PtrRecord ? domain.ToArpa() : domain),
                                                                    QType = queryType,
                                                                    QClass = rClass,
                                                                },
                                                        },
                                   };

            var cached = Cache.CheckCache(request.Questions);
            if (cached != null && cached.Answers != null && cached.Answers.Any())
            {
                cached.Header = request.Header;
                cached.Questions = request.Questions;
                return cached.GetExternalAnswer();
            }
                

            var result = Transport.SendRequest(request, dnsServer??_defaultServer1, TimeOut);

            if (result == null || result.Header == null || result.Header.RCode == ResponseCode.FormErr || result.Header.RCode == ResponseCode.ServFail)
                for (var ndx = 1; ndx < Retries; ++ndx)
                {
                    result = Transport.SendRequest(request, dnsServer??_defaultServer1, TimeOut);
                    if (result != null && result.Header != null && (result.Header.RCode != ResponseCode.FormErr && result.Header.RCode != ResponseCode.ServFail))
                        ndx = Retries;
                }

            if (result != null)
                Cache.AddCache(result);

            return result != null ? result.GetExternalAnswer() : null;
        }

        public DNS.ExternalInterfaces.IMessage Query(IEnumerable<DNS.ExternalInterfaces.IQuestion> questions, IPEndPoint dnsServer = null)
        {
            IMessage request = new Message
                {
                    Header = new Header(null)
                        {
                            OpCode = OpCode.Query,
                            QdCount = 1,
                            Id = (ushort)(new Random()).Next(),
                            Rd = UseRecursion,
                        },
                };
            request.Questions = questions.ToQuestions();

            var cached = Cache.CheckCache(request.Questions);
            if (cached != null && cached.Answers != null && cached.Answers.Any())
            {
                cached.Header = request.Header;
                cached.Questions = request.Questions;
                return cached.GetExternalAnswer();
            }

            var result = Transport.SendRequest(request, dnsServer ?? _defaultServer1, TimeOut);

            if (result == null || result.Header == null || result.Header.RCode == ResponseCode.FormErr || result.Header.RCode == ResponseCode.ServFail)
                for (var ndx = 1; ndx < Retries; ++ndx)
                {
                    result = Transport.SendRequest(request, dnsServer ?? _defaultServer1, TimeOut);
                    if (result != null && result.Header != null && (result.Header.RCode != ResponseCode.FormErr && result.Header.RCode != ResponseCode.ServFail))
                        ndx = Retries;
                }

            if (result != null)
                Cache.AddCache(result);

            return result != null ? result.GetExternalAnswer() : null;
        }

        public DNS.ExternalInterfaces.IMessage AuthoratativeQuery(string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In, bool ipv6Failover = false)
        {
            var index = domain.LastIndexOf('.');
            var last = 3;
            while (index > 0 && index >= last - 3)
            {
                last = index;
                index = domain.LastIndexOf('.', last - 1);
            }
            var topLevelDomain = domain.Substring(index + 1);

            DNS.ExternalConcretes.SoaRecord soaRecord = null;
            var auth = Query(topLevelDomain, RecordType.SoaRecord, dnsServer ?? _defaultServer1, rClass);
            if (auth.Authorities != null && auth.Authorities.Any())
                soaRecord = auth.Authorities.FirstOrDefault() as DNS.ExternalConcretes.SoaRecord;
            else if (auth.Answers != null && auth.Answers.Any())
                soaRecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.SoaRecord;

            if (soaRecord == null)
                return null;

            auth = Query(soaRecord.MName, RecordType.ARecord, dnsServer, rClass);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var arecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.ARecord;
            if (arecord != null)
                return Query(domain, queryType, new IPEndPoint(arecord.Address, 53), rClass);

            if (ipv6Failover)
            {
                //Support for IP6 when IP4 does not exist
                auth = Query(soaRecord.MName, RecordType.AaaaRecord, dnsServer, rClass);
                if (auth.Answers == null || !auth.Answers.Any())
                    return null;

                var aaaarecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.AaaaRecord;

                return aaaarecord == null ? null : Query(domain, queryType, new IPEndPoint(aaaarecord.Address, 53), rClass);
            }

            return null;
        }

        public DNS.ExternalInterfaces.IMessage AuthoratativeQuery(IEnumerable<DNS.ExternalInterfaces.IQuestion> questions, IPEndPoint dnsServer = null, bool ipv6Failover = false)
        {
            var domain = questions.FirstOrDefault().QName;
            var index = domain.LastIndexOf('.');
            var last = 3;
            while (index > 0 && index >= last - 3)
            {
                last = index;
                index = domain.LastIndexOf('.', last - 1);
            }
            var topLevelDomain = domain.Substring(index + 1);

            DNS.ExternalConcretes.SoaRecord soaRecord = null;
            var auth = Query(topLevelDomain, RecordType.SoaRecord, dnsServer ?? _defaultServer1);
            if (auth.Authorities != null && auth.Authorities.Any())
                soaRecord = auth.Authorities.FirstOrDefault() as DNS.ExternalConcretes.SoaRecord;
            else if (auth.Answers != null && auth.Answers.Any())
                soaRecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.SoaRecord;

            if (soaRecord == null)
                return null;

            auth = Query(soaRecord.MName, RecordType.ARecord, dnsServer);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var arecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.ARecord;
            if (arecord != null)
                return Query(questions, new IPEndPoint(arecord.Address, 53));

            if (ipv6Failover)
            {
                //Support for IP6 when IP4 does not exist
                auth = Query(soaRecord.MName, RecordType.AaaaRecord, dnsServer);
                if (auth.Answers == null || !auth.Answers.Any())
                    return null;

                var aaaarecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.AaaaRecord;

                return aaaarecord == null ? null : Query(questions, new IPEndPoint(aaaarecord.Address, 53));
            }

            return null;
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

        public SpfResult VerifySpfRecord(string domain, string ip, IPEndPoint dnsServer = null, bool ipv6Failover = false)
        {
            IPAddress _ip = null;
            if(IPAddress.TryParse(ip, out _ip))
                return SpfResult.NoResult;

            if(string.IsNullOrEmpty(domain.TryTrim()))
                return SpfResult.NoResult;

            var spf = GetSpfRecords(domain, dnsServer ?? _defaultServer1, ipv6Failover); //Recursivly lookup including includes and redirects.
            if (spf == null || !spf.Any())
                return SpfResult.NoResult;

            var queryCount = 0;
            //foreach spf record fetched, loop till pass.
            var results = new List<SpfResult>();
            foreach (var rec in spf)
            {
                if(queryCount > 10) //Should not make more than 10 dns queries to fetch data needed to check spf.
                    return SpfResult.NoResult;

                //Test each mechanism individually in each record.
                foreach (var part in rec.Text.Split(' '))
                {
                    if(Skip(part))
                        continue;

                    var parts = ParseSpfPart(part);

                    if (part.TrySubstring(0, 2).AreEqual("ip") || part.TrySubstring(1, 2).AreEqual("ip"))
                    {
                        //var qualifier = parts["qualifier"];
                        if (SpfChecker.VerifyIpMechanism(_ip, part.TryTrim()) == SpfResult.Pass)
                            return SpfResult.Pass;
                    }

                    if (part.TrySubstring(0, 1).AreEqual("a") || part.TrySubstring(1, 1).AreEqual("a"))
                    {
                        //var qualifier = parts["qualifier"];
                        if (!string.Equals("a", parts["host"], StringComparison.CurrentCultureIgnoreCase))
                            continue; //Somthing went wrong...

                        var addresses = GetARecordsForSpf(ref queryCount, domain.TryTrim(), dnsServer ?? _defaultServer1, ipv6Failover);

                        if (SpfChecker.VerifyAMechanism(_ip, addresses, (!string.IsNullOrEmpty(parts["range"]) ? parts["range"] : null)) == SpfResult.Pass)
                            return SpfResult.Pass;
                    }

                    if (part.TrySubstring(0, 2).AreEqual("mx") || part.TrySubstring(1, 2).AreEqual("mx"))
                    {
                        //var qualifier = parts["qualifier"];
                        if (string.Equals("mx", parts["host"], StringComparison.CurrentCultureIgnoreCase))
                            continue; //Somthing went wrong...

                        var addresses = GetMxRecordsForSpf(ref queryCount, domain.TryTrim(), dnsServer ?? _defaultServer1, ipv6Failover);

                        if (SpfChecker.VerifyMxMechanism(_ip, addresses, (!string.IsNullOrEmpty(parts["range"]) ? parts["range"] : null)) == SpfResult.Pass)
                            return SpfResult.Pass;
                    }

                    if (part.TrySubstring(0, 3).AreEqual("ptr") || part.TrySubstring(1, 3).AreEqual("ptr"))
                    {

                    }

                    //EXISTS - A record exists no matter the address.
                }
            }

            return SpfResult.Fail;
        }

        private bool Skip(string part)
        {
            if (string.IsNullOrEmpty(part.TryTrim()))
                return true;
            if (string.Equals("v=spf1", part.TryTrim(), StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (string.Equals("redirect=", part.TryTrim().TrySubstring(0, 9), StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (string.Equals("include:", part.TryTrim().TrySubstring(0, 8), StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }

        private IDictionary<string, string> ParseSpfPart(string part)
        {
            var qualifiers = new[] { "+", "-", "~", "?" };

            return new Dictionary<string, string>
                       {
                           { "qualifier", qualifiers.Contains(part.TrySubstring(0, 1)) ? part.TrySubstring(0, 1).TryTrim() : null },
                           { "range", part.Split('/').Skip(1).FirstOrDefault().TryTrim() },
                           { "host", part.Split('/').FirstOrDefault().Split(':').FirstOrDefault().TryTrim() }
                       };
        }

        private IEnumerable<DNS.ExternalConcretes.TxtRecord> GetSpfRecords(string domain, IPEndPoint dnsServer, bool ipv6Failover)
        {
            var records = new List<DNS.ExternalConcretes.TxtRecord>();

            //Only query TXT records since SPF was obsoleted in aug 2013
            var spf = AuthoratativeQuery(domain, RecordType.TxtRecord, dnsServer, RecordClass.In, ipv6Failover); 
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
                    
                    records.InsertRange(0, GetSpfRecords(scratch.Skip(1).FirstOrDefault(), dnsServer, ipv6Failover));
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

                    records.InsertRange(0, GetSpfRecords(scratch.Skip(1).FirstOrDefault(), dnsServer, ipv6Failover));
                }
            }
            #endregion

            //Verify record is not spf2 / sender id
            records.AddRange(spfRecords.Where(an => string.Equals(an.Text.TryTrim().TrySubstring(0, 6), "v=spf1", StringComparison.CurrentCultureIgnoreCase)));

            return records;
        }

        private IEnumerable<IPAddress> GetMxRecordsForSpf(ref int queryCount, string domain, IPEndPoint dnsServer, bool ipv6Failover)
        {
            var addresses = new List<IPAddress>();

            var mxRecs = AuthoratativeQuery(domain, RecordType.MxRecord, dnsServer ?? _defaultServer1);
            queryCount++;
            if (mxRecs.Answers == null || !mxRecs.Answers.Any())
                return null; //No mxes 

            var questions = (from IResourceRecord mx in mxRecs.Answers
                             select (DNS.ExternalConcretes.MxRecord)mx.ConvertToExternalType()
                                 into temp
                                 select new DNS.ExternalConcretes.Question
                                 {
                                     QName = temp.Name,
                                     QType = RecordType.ARecord,
                                     QClass = RecordClass.In
                                 });

            var aRecs = AuthoratativeQuery(questions, dnsServer, ipv6Failover);
            queryCount++;

            if (aRecs.Answers != null && aRecs.Answers.Any())
            {
                addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.ARecord) != null)
                                                .Select(an => (an as DNS.ExternalConcretes.ARecord).Address));
            }
            else if (ipv6Failover)
            {
                //Try Aaaa in the event no A records. 
                questions = (from IResourceRecord mx in mxRecs.Answers
                             select (DNS.ExternalConcretes.MxRecord)mx.ConvertToExternalType()
                                 into temp
                                 select new DNS.ExternalConcretes.Question
                                 {
                                     QName = temp.Name,
                                     QType = RecordType.AaaaRecord,
                                     QClass = RecordClass.In
                                 });

                aRecs = AuthoratativeQuery(questions, dnsServer ?? _defaultServer1);
                queryCount++;

                if (aRecs.Answers != null && aRecs.Answers.Any())
                    addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.AaaaRecord) != null)
                                                .Select(an => (an as DNS.ExternalConcretes.AaaaRecord).Address));
            }

            return addresses;
        }

        private IEnumerable<IPAddress> GetARecordsForSpf(ref int queryCount, string domain, IPEndPoint dnsServer, bool ipv6Failover)
        {
            var addresses = new List<IPAddress>();

            var aRecs = AuthoratativeQuery(domain, RecordType.ARecord, dnsServer ?? _defaultServer1);
            queryCount++;

            if (aRecs.Answers != null && aRecs.Answers.Any())
            {
                addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.ARecord) != null)
                                                .Select(an => (an as DNS.ExternalConcretes.ARecord).Address));
            }
            else if (ipv6Failover)
            {
                //Try Aaaa in the event no A records. 
                aRecs = AuthoratativeQuery(domain, RecordType.AaaaRecord, dnsServer ?? _defaultServer1);
                queryCount++;

                if (aRecs.Answers != null && aRecs.Answers.Any())
                    addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.AaaaRecord) != null)
                                                .Select(an => (an as DNS.ExternalConcretes.AaaaRecord).Address));
            }

            return addresses;
        }

         
    }
}