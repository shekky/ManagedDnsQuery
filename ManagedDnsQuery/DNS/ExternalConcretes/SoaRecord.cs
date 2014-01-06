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
    public sealed class SoaRecord : ISoaRecord
    {
        public string Name { get; set; }
        public RecordType Type { get; set; }
        public RecordClass Class { get; set; }
        public uint Ttl { get; set; }
        
        public string MName { get; set; }
        public string RName { get; set; }
        public uint? Serial { get; set; }
        public uint? Refresh { get; set; }
        public uint? Retry { get; set; }
        public uint? Expire { get; set; }
        public uint? Minimum { get; set; }

        public SoaRecord() { }

        public SoaRecord(string name, RecordType type, RecordClass cl, uint ttl, IEnumerable<object> vals)
        {
            Name = name;
            Type = type;
            Class = cl;
            Ttl = ttl;

            if (vals == null || !vals.Any() || vals.Count() < 7)
                return;

            MName = (string) vals.FirstOrDefault();
            RName = (string) vals.Skip(1).FirstOrDefault();
            Serial = (uint?) vals.Skip(2).FirstOrDefault();
            Refresh = (uint?) vals.Skip(3).FirstOrDefault();
            Retry = (uint?) vals.Skip(4).FirstOrDefault();
            Expire = (uint?) vals.Skip(5).FirstOrDefault();
            Minimum = (uint?) vals.Skip(6).FirstOrDefault();
        }

        public string AsString
        {
            get
            {
                return string.Format("{0}     {1}     {2}     {3}     {4}   {5}  {6}  {7}  {8}  {9}  {10}",
                                        Name,
                                        Ttl,
                                        Class.ToString().ToUpper(),
                                        Type.ToString().ToUpper().Replace("RECORD", ""),
                                        MName,
                                        RName,
                                        Serial,
                                        Refresh,
                                        Retry,
                                        Expire,
                                        Minimum);
            }
        }
    }
}
