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
using System.Text;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS.MessageingImplementations
{
    internal class ByteReader : IByteReader
    {
        public IList<byte> RawMessage { get; private set; }
        public int Position { get; private set; }

        internal ByteReader(IList<byte> rawMessage, int pos = 0)
        {
            RawMessage = rawMessage;
            Position = pos > 0 ? pos : 12;
        }

        //public byte NextByte
        //{
        //    get
        //    {
        //        var result = (byte)0;
        //        if (Position < RawMessage.Count)
        //        {
        //            result = RawMessage.Skip(Position)
        //                               .FirstOrDefault();
        //            ++Position;
        //        }
        //        return result;
        //    }
        //}

        public byte NextByte()
        {
            var result = (byte)0;
            if (Position < RawMessage.Count)
            {
                result = RawMessage.Skip(Position)
                                   .FirstOrDefault();
                ++Position;
            }
            return result;
        }

        //public string ReadLabels
        //{
        //    get
        //    {
        //        var sb = new StringBuilder();
        //        byte len;

        //        while ((len = NextByte) != 0)
        //        {
        //            if ((len & 0xc0) == 0xc0) //Compression
        //            {
        //                var subReader = new ByteReader(RawMessage, (len & 0x3f) | NextByte);
        //                sb.Append(subReader.ReadLabels);
        //                return sb.ToString();
        //            }

        //            for (var ndx = len; ndx > 0; --ndx)
        //                sb.Append((char)NextByte);
        //            sb.Append('.');
        //        }

        //        return sb.ToString();
        //    }
        //}

        public string ReadLabels()
        {
            var sb = new StringBuilder();
            byte len;

            while ((len = NextByte()) != 0)
            {
                if ((len & 0xc0) == 0xc0) //Compression
                {
                    var subReader = new ByteReader(RawMessage, (len & 0x3f) | NextByte());
                    sb.Append(subReader.ReadLabels());
                    return sb.ToString();
                }

                for (var ndx = len; ndx > 0; --ndx)
                    sb.Append((char)NextByte());
                sb.Append('.');
            }

            return sb.ToString();
        }

        //public string ReadText
        //{
        //    get
        //    {
        //        var len = NextByte;
        //        var sb = new StringBuilder();

        //        for (var ndx = 0; ndx < len; ++ndx)
        //            sb.Append((char) NextByte);

        //        return sb.ToString();
        //    }
        //}

        public string ReadText()
        {
            var len = NextByte();
            var sb = new StringBuilder();

            for (var ndx = 0; ndx < len; ++ndx)
                sb.Append((char)NextByte());

            return sb.ToString();
        }


        //public ushort ReadUShort
        //{
        //    get
        //    {
        //        var result = RawMessage.Skip(Position)
        //                                .Take(2)
        //                                .ToLeUShort();
        //        Position += 2;
        //        return result;
        //    }
        //}

        public ushort ReadUShort()
        {
            var result = RawMessage.Skip(Position)
                                        .Take(2)
                                        .ToLeUShort();
            Position += 2;
            return result;
        }

        //public uint ReadUInt
        //{
        //    get
        //    {
        //        var result = RawMessage.Skip(Position)
        //                               .Take(4)
        //                               .ToLeUInt();
        //        Position += 4;
        //        return result;
        //    }
        //}

        public uint ReadUInt()
        {
            var result = RawMessage.Skip(Position)
                                       .Take(4)
                                       .ToLeUInt();
            Position += 4;
            return result;
        }

        

        public IEnumerable<byte> GetRdata(ushort len)
        {
            return RawMessage.Skip(Position).Take(len);
        }
    }
}
