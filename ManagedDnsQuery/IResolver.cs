﻿/**********************************************************************************
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

using System.Collections.Generic;
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.ExternalInterfaces;
using ManagedDnsQuery.SPF.Interfaces;

namespace ManagedDnsQuery
{
    public interface IResolver
    {
        bool UseRecursion { get; set; }
        int TimeOut { get; set; }
        int Retries { get; set; }

        IMessage Query(string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In);
        IMessage Query(IEnumerable<IQuestion> questions, IPEndPoint dnsServer = null);
        IMessage AuthoratativeQuery(string domain, RecordType queryType, IPEndPoint dnsServer = null, RecordClass rClass = RecordClass.In, bool ipv6Failover = false);
        IMessage AuthoratativeQuery(IEnumerable<IQuestion> questions, IPEndPoint dnsServer = null, bool ipv6Failover = false);
        string QueryWhois(string domainName);
        SpfResult VerifySpfRecord(string domain, string ip, IPEndPoint dnsServer = null, bool ipv6Failover = false);
    }
}
