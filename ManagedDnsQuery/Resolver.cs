/**********************************************************************************
 ==================================================================================
    Copyright 2013 Tim Burnett 
    
    This file is part of ManagedDnsQuery.

    ManagedDnsQuery is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    ManagedDnsQuery is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with ManagedDnsQuery.  If not, see <http://www.gnu.org/licenses/>.
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
using ManagedDnsQuery.WHOIS.Concretes;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery
{
    public sealed class Resolver : IResolver
    {
        public bool UseRecursion { get; set; }
        public int TimeOut { get; set; }
        public int Retries { get; set; }
        public IDnsTransport Transport { get; private set; }
        public IWhoisTransport WhoisTransport { get; private set; }
        public ITLDHandler TldHandler { get; private set; }
        public IQueryCache Cache { get; private set; }
        
        public Resolver()
        {
            Transport = new UdpDnsTransport();
            Cache = new QueryCache();
            Retries = 3;
            TimeOut = 60;
            UseRecursion = true;
            WhoisTransport = new WhoisTcpTransport();
            TldHandler = new TldHandler();
        }

        public Resolver(IDnsTransport transport, IQueryCache cache = null, IWhoisTransport wtransport = null, ITLDHandler tldHandler = null, int retrys = 3, int timeout = 60, bool useRecursion = true)
        {
            Transport = transport;
            Retries = retrys;
            TimeOut = timeout;
            UseRecursion = useRecursion;
            Cache = (cache ?? new QueryCache());
            WhoisTransport = (wtransport?? new WhoisTcpTransport());
            TldHandler = (tldHandler?? new TldHandler());
        }

        public DNS.ExternalInterfaces.IMessage Query(string name, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In)
        {
            if (!string.IsNullOrEmpty(name) && name.Substring(name.Length - 1) != ".")
                name = string.Format("{0}.", name);

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

        public DNS.ExternalInterfaces.IMessage AuthoratativeQuery(string name, string domain, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In)
        {
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
            return arecord == null ? null : Query(name, queryType, new IPEndPoint(arecord.Address, 53), rClass);
        }

        public async Task<DNS.ExternalInterfaces.IMessage> QueryAsync(string name, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In)
        {
            return await Task.Factory.StartNew(() => Query(name, queryType, dnsServer, rClass));
        }

        public async Task<DNS.ExternalInterfaces.IMessage> AuthoratativeQueryAsync(string name, string domain, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In)
        {
            return await Task.Factory.StartNew(() => AuthoratativeQuery(name, domain, queryType, dnsServer, rClass));
        }

        public string QueryWhois(string domainName)
        {
            domainName = domainName.Trim().Replace("http://", "").Replace("https://", "");
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
    }
}
