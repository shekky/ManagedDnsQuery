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
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS.MessageingConcretes
{
    internal class ResourceRecord : IResourceRecord
    {
        public string Name { get; set; }
        public RecordType Type { get; set; }
        public RecordClass Class { get; set; }
        public uint Ttl { get; set; }
        public ushort RdLength { get; set; }
        public IEnumerable<byte> Rdata { get; set; }
        public object Record { get; set; }
        public DateTime TimeStamp { get; internal set; }

        internal ResourceRecord(IByteReader reader)
        {
            TimeStamp = DateTime.Now;

            if(reader == null)
                return;

            Name = reader.ReadLabels();
            Type = (RecordType) reader.ReadUShort();
            Class = (RecordClass) reader.ReadUShort();
            Ttl = reader.ReadUInt();
            RdLength = reader.ReadUShort();
            Rdata = reader.GetRdata(RdLength);

            var type = System.Type.GetType(string.Format("ManagedDnsQuery.DNS.RDataConcretes.{0}, {1}", Type, "ManagedDnsQuery"));
            if(type != null)
                Record = Activator.CreateInstance(type, new object[] { reader });
        }

        public object ConvertToExternalType()
        {
            var type = System.Type.GetType(string.Format("ManagedDnsQuery.DNS.ExternalConcretes.{0}, {1}", Type, "ManagedDnsQuery"));

            if (type != null && Record != null)
            {
                IEnumerable<object> propVals = Record.GetType()
                                                     .GetProperties()
                                                     .Select(prop => prop.GetValue(Record, null))
                                                     .ToArray();

                return Activator.CreateInstance(type, new object[] { Name, Type, Class, Ttl, propVals});
            }

            return null;
        }

        public IEnumerable<byte> ToBytes()
        {
            var bytes = new List<byte>();

            bytes.AddRange(Name.ToLabelBytes());
            bytes.AddRange(((ushort) Type).ToBeBytes());
            bytes.AddRange(((ushort) Class).ToBeBytes());
            bytes.AddRange(Ttl.ToBeBytes());
            bytes.AddRange(RdLength.ToBeBytes());
            bytes.AddRange(Rdata);

            return bytes;
        }
    }
}
