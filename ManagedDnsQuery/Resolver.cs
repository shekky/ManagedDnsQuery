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
using ManagedDnsQuery.DNS.RDataConcretes;
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

        public DNS.ExternalInterfaces.IMessage Query(IEnumerable<IQuestion> questions, IPEndPoint dnsServer = null)
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
                    Questions = new List<IQuestion>(),
                };
            request.Questions = questions;


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

        public DNS.ExternalInterfaces.IMessage AuthoratativeQuery(string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In)
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

            //Support for IP6 when IP4 does not exist
            auth = Query(soaRecord.MName, RecordType.AaaaRecord, dnsServer, rClass);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var aaaarecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.AaaaRecord;

            return aaaarecord == null ? null : Query(domain, queryType, new IPEndPoint(aaaarecord.Address, 53), rClass);
        }

        public DNS.ExternalInterfaces.IMessage AuthoratativeQuery(IEnumerable<IQuestion> questions, IPEndPoint dnsServer = null)
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

            //Support for IP6 when IP4 does not exist
            auth = Query(soaRecord.MName, RecordType.AaaaRecord, dnsServer);
            if (auth.Answers == null || !auth.Answers.Any())
                return null;

            var aaaarecord = auth.Answers.FirstOrDefault() as DNS.ExternalConcretes.AaaaRecord;

            return aaaarecord == null ? null : Query(questions, new IPEndPoint(aaaarecord.Address, 53));
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

        public SpfResult VerifySpfRecord(string domain, string ip, IPEndPoint dnsServer = null)
        {
            IPAddress _ip = null;
            if(IPAddress.TryParse(ip, out _ip))
                return SpfResult.NoResult;

            if(string.IsNullOrEmpty(domain.TryTrim()))
                return SpfResult.NoResult;

            var spf = GetSpfRecords(domain, dnsServer); //Recursivly lookup including includes and redirects.
            if (spf == null || !spf.Any())
                return SpfResult.NoResult;

            var queryCount = 0;
            //foreach spf record fetched, loop till pass.
            var qualifiers = new[] { "+", "-", "~", "?" };
            var results = new List<SpfResult>();
            foreach (var rec in spf)
            {
                if(queryCount > 10) //Should not make more than 10 dns queries to fetch data needed to check spf.
                    return SpfResult.NoResult;

                //Test each mechanism individually in each record.
                foreach (var part in rec.Text.Split(' '))
                {
                    if (string.Equals("v=spf1", part.TryTrim(), StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    if (string.Equals("redirect=", part.TryTrim().TrySubstring(0, 9), StringComparison.CurrentCultureIgnoreCase)) 
                        continue;
                    if (string.Equals("include:", part.TryTrim().TrySubstring(0, 8), StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    //TODO: Verify qualifier....
                    #region IP Mechanism
                    if (string.Equals("ip", part.TrySubstring(0, 2), StringComparison.CurrentCultureIgnoreCase) || string.Equals("ip", part.TrySubstring(1, 2), StringComparison.CurrentCultureIgnoreCase))
                    {
                        var qualifier = qualifiers.Contains(part.TrySubstring(0, 1)) ? part.TrySubstring(0, 1) : null;
                        if (SpfChecker.VerifyIpMechanism(_ip, part.TryTrim()) == SpfResult.Pass)
                            return SpfResult.Pass;
                    }
                    #endregion

                    #region A Record Mechanism
                    if (string.Equals("a", part.TrySubstring(0, 1), StringComparison.CurrentCultureIgnoreCase) || string.Equals("a", part.TrySubstring(1, 1), StringComparison.CurrentCultureIgnoreCase))
                    {
                        var qualifier = qualifiers.Contains(part.TrySubstring(0, 1)) ? part.TrySubstring(0, 1) : null;
                        var range = part.Split('/').Skip(1).FirstOrDefault().TryTrim(); //Get range first.
                        var host = part.Split('/').FirstOrDefault().Split(':').FirstOrDefault().TryTrim(); //Get domain.
                        if (string.Equals("a", host, StringComparison.CurrentCultureIgnoreCase))
                            host = domain.TryTrim();

                        var addresses = new List<IPAddress>();
                        var aRecs = AuthoratativeQuery(host, RecordType.ARecord, dnsServer ?? _defaultServer1);
                        queryCount++;
                        if (aRecs.Answers != null && aRecs.Answers.Any())
                        {
                            addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.ARecord) != null)
                                                            .Select(an => (an as DNS.ExternalConcretes.ARecord).Address));
                        }
                        else
                        {
                            //Try Aaaa in the event no A records. 
                            aRecs = AuthoratativeQuery(host, RecordType.AaaaRecord, dnsServer ?? _defaultServer1);
                            queryCount++;
                            if (aRecs.Answers != null && aRecs.Answers.Any())
                                addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.AaaaRecord) != null)
                                                            .Select(an => (an as DNS.ExternalConcretes.AaaaRecord).Address));
                            else
                                continue; //Fail / error...
                        }

                        if (SpfChecker.VerifyAMechanism(_ip, addresses, (!string.IsNullOrEmpty(range) ? range.TryTrim() : null)) == SpfResult.Pass)
                            return SpfResult.Pass;
                    }
                    #endregion

                    #region MX Record Mechanism
                    if (string.Equals("mx", part.TrySubstring(0, 2), StringComparison.CurrentCultureIgnoreCase) || string.Equals("mx", part.TrySubstring(1, 2), StringComparison.CurrentCultureIgnoreCase))
                    {
                        var qualifier = qualifiers.Contains(part.TrySubstring(0, 1)) ? part.TrySubstring(0, 1) : null;
                        var range = part.Split('/').Skip(1).FirstOrDefault().TryTrim(); //Get range first.
                        var host = part.Split('/').FirstOrDefault().Split(':').FirstOrDefault().TryTrim(); //Get domain.
                        if (string.Equals("mx", host, StringComparison.CurrentCultureIgnoreCase))
                            host = domain.TryTrim();

                        var mxRecs = AuthoratativeQuery(host, RecordType.MxRecord, dnsServer ?? _defaultServer1);
                        queryCount++;
                        if(mxRecs.Answers == null || !mxRecs.Answers.Any())
                            continue; //No mxes 

                        var questions = (from IResourceRecord mx in mxRecs.Answers
                                         select (DNS.ExternalConcretes.MxRecord) mx.ConvertToExternalType()
                                         into temp select new Question(null)
                                            {
                                                QName = temp.Name, 
                                                QType = RecordType.ARecord, 
                                                QClass = RecordClass.In
                                            });

                        var addresses = new List<IPAddress>();
                        var aRecs = AuthoratativeQuery(questions, dnsServer ?? _defaultServer1);
                        queryCount++;
                        if (aRecs.Answers != null && aRecs.Answers.Any())
                        {
                            addresses.AddRange(aRecs.Answers.Where(an => (an as DNS.ExternalConcretes.ARecord) != null)
                                                            .Select(an => (an as DNS.ExternalConcretes.ARecord).Address));
                        }
                        else
                        {
                            //Try Aaaa in the event no A records. 
                            questions = (    from IResourceRecord mx in mxRecs.Answers
                                             select (DNS.ExternalConcretes.MxRecord)mx.ConvertToExternalType()
                                                 into temp
                                                 select new Question(null)
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
                            else
                                continue; //Fail / error...
                        }

                        if (SpfChecker.VerifyMxMechanism(_ip, addresses, (!string.IsNullOrEmpty(range) ? range.TryTrim() : null)) == SpfResult.Pass)
                            return SpfResult.Pass;
                    }
                    #endregion

                    #region PTR Record Mechanism
                    if (string.Equals("ptr", part.TrySubstring(0, 3), StringComparison.CurrentCultureIgnoreCase) || string.Equals("ptr", part.TrySubstring(0, 3), StringComparison.CurrentCultureIgnoreCase))
                    {

                    }
                    #endregion

                    //EXISTS - A record exists no matter the address.
                }
            }

            return SpfResult.Fail;
        }

        private IEnumerable<DNS.ExternalConcretes.TxtRecord> GetSpfRecords(string domain, IPEndPoint dnsServer = null)
        {
            var records = new List<DNS.ExternalConcretes.TxtRecord>();

            var spf = AuthoratativeQuery(domain, RecordType.TxtRecord, dnsServer??_defaultServer1); //Only query TXT records since SPF was obsoleted in aug 2013
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
                    
                    records.InsertRange(0, GetSpfRecords(scratch.Skip(1).FirstOrDefault()));
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

                    records.InsertRange(0, GetSpfRecords(scratch.Skip(1).FirstOrDefault()));
                }
            }
            #endregion

            //Verify record is not spf2 / sender id
            records.AddRange(spfRecords.Where(an => string.Equals(an.Text.TryTrim().TrySubstring(0, 6), "v=spf1", StringComparison.CurrentCultureIgnoreCase)));

            return records;
        }
    }
}