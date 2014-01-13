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

using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.ExternalInterfaces;

namespace ManagedDnsQuery
{
    public interface IResolver
    {
        bool UseRecursion { get; set; }
        int TimeOut { get; set; }
        int Retries { get; set; }
        IDnsTransport Transport { get; } 

        IQueryCache Cache { get; }
        IMessage Query(string name, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In);
        IMessage AuthoratativeQuery(string name, string domain, RecordType queryType, IPEndPoint dnsServer, RecordClass rClass = RecordClass.In);
    }
}
