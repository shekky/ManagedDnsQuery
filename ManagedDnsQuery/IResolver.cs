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

using System.Net;
using System.Threading.Tasks;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.ExternalInterfaces;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery
{
    public interface IResolver
    {
        bool UseRecursion { get; set; }
        int TimeOut { get; set; }
        int Retries { get; set; }
        IDnsTransport Transport { get; }
        IWhoisTransport WhoisTransport { get; }
        ITLDHandler TldHandler { get; }
        IQueryCache Cache { get; }

        IMessage Query(string name, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In);
        Task<IMessage> QueryAsync(string name, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In);
        IMessage AuthoratativeQuery(string name, string domain, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In);
        Task<IMessage> AuthoratativeQueryAsync(string name, string domain, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In);
        string QueryWhois(string domainName);
        Task<string> QueryWhoisAsync(string domainName);
    }
}
