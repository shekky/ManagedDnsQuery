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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS.MessageingConcretes
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

        public string ReadText()
        {
            var len = NextByte();
            var sb = new StringBuilder();

            for (var ndx = 0; ndx < len; ++ndx)
                sb.Append((char)NextByte());

            return sb.ToString();
        }

        public ushort ReadUShort()
        {
            var result = RawMessage.Skip(Position)
                                        .Take(2)
                                        .ToLeUShort();
            Position += 2;
            return result;
        }

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
