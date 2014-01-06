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

using System.Collections.Generic;
using System.Linq;
using ManagedDnsQuery.DNS.ExternalInterfaces;

namespace ManagedDnsQuery.DNS.ExternalConcretes
{
    public sealed class NsRecord : INsRecord
    {
        public string Name { get; set; }
        public RecordType Type { get; set; }
        public RecordClass Class { get; set; }
        public uint Ttl { get; set; }
        public string NsDomainName { get; set; }

        public NsRecord() { }

        public NsRecord(string name, RecordType type, RecordClass cl, uint ttl, IEnumerable<object> vals)
        {
            Name = name;
            Type = type;
            Class = cl;
            Ttl = ttl;

            if (vals == null || !vals.Any())
                return;

            NsDomainName = (string) vals.FirstOrDefault();
        }

        public string AsString
        {
            get
            {
                return string.Format("{0}     {1}     {2}     {3}     {4}",
                                        Name, 
                                        Ttl, 
                                        Class.ToString().ToUpper(),
                                        Type.ToString().ToUpper().Replace("RECORD", ""),
                                        NsDomainName);
            }
        }
    }
}
