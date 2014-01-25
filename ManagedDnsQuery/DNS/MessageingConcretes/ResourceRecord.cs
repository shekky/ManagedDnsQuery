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
