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
using System.Threading.Tasks;
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

        public async Task<DNS.ExternalInterfaces.IMessage> QueryAsync(string name, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In)
        {
            return await Task.Factory.StartNew(() => Query(name, queryType, dnsServer, rClass));
        }

        public async Task<DNS.ExternalInterfaces.IMessage> AuthoratativeQueryAsync(string name, string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In)
        {
            return await Task.Factory.StartNew(() => AuthoratativeQuery(name, domain, queryType, dnsServer, rClass));
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

        public async Task<string> QueryWhoisAsync(string domainName)
        {
            return await Task.Factory.StartNew(() => QueryWhois(domainName));
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

        public async Task<SpfResult> VerifySpfRecordAsync(string domain, string ip)
        {
            return await Task.Factory.StartNew(() => VerifySpfRecord(domain, ip));
        }
    }
}

// This is the spf record fetch chain for gmail.com
//GMail.com
//"v=spf1 redirect=_spf.google.com"

//Google.com
//"v=spf1 include:_spf.google.com ip4:216.73.93.70/31 ip4:216.73.93.72/31 ~all"

//_spf.google.com
//"v=spf1 include:_netblocks.google.com include:_netblocks2.google.com include:_netblocks3.google.com ~all"

//_netblocks.google.com
//"v=spf1 ip4:216.239.32.0/19 ip4:64.233.160.0/19 ip4:66.249.80.0/20 ip4:72.14.192.0/18 ip4:209.85.128.0/17 ip4:66.102.0.0/20 ip4:74.125.0.0/16 ip4:64.18.0.0/20 ip4:207.126.144.0/20 ip4:173.194.0.0/16 ~all"

//_netblocks2.google.com
//"v=spf1 ip6:2001:4860:4000::/36 ip6:2404:6800:4000::/36 ip6:2607:f8b0:4000::/36 ip6:2800:3f0:4000::/36 ip6:2a00:1450:4000::/36 ip6:2c0f:fb50:4000::/36 ~all"

//_netblocks3.google.com
//"v=spf1 ~all"


// This is the spf record fetch chain for outlook.com
//outlook.com
//"v=spf1 include:spf-a.outlook.com include:spf-b.outlook.com ip4:157.55.9.128/25 include:spfa.bigfish.com include:spfb.bigfish.com include:spfc.bigfish.com include:spf-a.hotmail.com include:_spf-ssg-b.microsoft.com include:_spf-ssg-c.microsoft.com ~all"

//spf-a.outlook.com
//"v=spf1 ip4:157.56.232.0/21 ip4:157.56.240.0/20 ip4:207.46.198.0/25 ip4:207.46.4.128/25 ip4:157.56.24.0/25 ip4:157.55.157.128/25 ip4:157.55.61.0/24 ip4:157.55.49.0/25 ip4:65.55.174.0/25 ip4:65.55.126.0/25 ip4:65.55.113.64/26 ip4:65.55.94.0/25 -all"

//spf-b.outlook.com
//"v=spf1 ip4:65.55.78.128/25 ip4:111.221.112.0/21 ip4:207.46.58.128/25 ip4:111.221.69.128/25 ip4:111.221.66.0/25 ip4:111.221.23.128/25 ip4:70.37.151.128/25 ip4:157.56.248.0/21 ip4:213.199.177.0/26 ip4:157.55.225.0/25 ip4:157.55.11.0/25 -all"

//spfa.bigfish.com
//"v=spf1 ip4:157.55.116.128/26 ip4:157.55.133.0/24 ip4:157.55.158.0/23 ip4:157.55.234.0/24 ip4:157.56.120.0/25 ip4:207.46.100.0/24 ip4:207.46.108.0/25 ip4:207.46.163.0/24 ip4:134.170.140.0/24 ip4:157.56.96.0/19 -all"

//spfb.bigfish.com
//"v=spf1 ip4:207.46.51.64/26 ip4:213.199.154.0/24 ip4:213.199.180.128/26 ip4:216.32.180.0/23 ip4:64.4.22.64/26 ip4:65.55.83.128/27 ip4:65.55.169.0/24 ip4:65.55.88.0/24 ip4:131.107.0.0/16 ip4:157.56.73.0/24 ip4:134.170.132.0/24 -all"

//spfc.bigfish.com
//"v=spf1 ip4:207.46.101.128/26 ip6:2a01:111:f400:7c00::/54 ip6:2a01:111:f400:fc00::/54 ip4:157.56.87.192/26 ip4:157.55.40.32/27 ip4:157.56.123.0/27 ip4:157.56.91.0/27 ip4:157.55.206.0/23 ip4:157.56.192.0/19 -all"

//spf-a.hotmail.com
//"v=spf1 ip4:157.55.0.192/26 ip4:157.55.1.128/26 ip4:157.55.2.0/25 ip4:65.54.190.0/24 ip4:65.54.51.64/26 ip4:65.54.61.64/26 ip4:65.55.111.0/24 ip4:65.55.116.0/25 ip4:65.55.34.0/24 ip4:65.55.90.0/24 ip4:65.54.241.0/24 ip4:207.46.117.0/24 ~all"

//_spf-ssg-b.microsoft.com
//"v=spf1 ip4:207.68.169.173/30 ip4:207.68.176.1/26 ip4:207.46.132.129/27 ip4:207.68.176.97/27 ip4:65.55.238.129/26 ip4:207.46.222.193/26 ip4:207.46.116.135/29 ip4:65.55.178.129/27 ip4:213.199.161.129/27 ip4:65.55.33.70/28 ~all"

//_spf-ssg-c.microsoft.com
//"v=spf1 ip4:65.54.121.123/29 ip4:65.55.81.53/28 ip4:65.55.234.192/26 ip4:207.46.200.0/27 ip4:65.55.52.224/27 ip4:94.245.112.10/31 ip4:94.245.112.0/27 ip4:111.221.26.0/27 ip4:207.46.50.221/26 ip4:207.46.50.224 ~all"