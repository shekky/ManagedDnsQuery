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

namespace ManagedDnsQuery.DNS.MessageingInterfaces
{
    internal enum HeaderBitPosition
    {
        RCode = 0,
        Z = 4,
        Ra = 7,
        Rd = 8,
        Tc = 9,
        Aa = 10,
        OpCode = 11,
        Qr = 15,
    }

    internal enum HeaderBytePosition
        {
            Id = 0,
            Flags = 2,
            QdCount = 4,
            AnCount = 6,
            NsCount = 8,
            ArCount = 10,
        }

    public interface IHeader : IByteConverter
    {
        ushort Id { get; set; }
        bool Aa { get; set; }
        bool Tc { get; set; }
        bool Rd { get; set; }
        bool Ra { get; set; }
        ushort Z { get; set; }
        ushort QdCount { get; set; }
        ushort AnCount { get; set; }
        ushort NsCount { get; set; }
        ushort ArCount { get; set; }
        Qr Qr { get; set; }
        OpCode OpCode { get; set; }
        ResponseCode RCode { get; set; }

        ExternalInterfaces.IHeader ToExternal();
    }
}
