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
using System.Net;

namespace ManagedDnsQuery.DNS
{
    internal static class ToBigEndianExtensions
    {
        internal static IEnumerable<byte> ToBeBytes(this ushort value)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
        }

        internal static IEnumerable<byte> ToBeBytes(this uint value)
        {
            var temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        }

        internal static ushort ToBeUshort(this ushort value)
        {
            var temp = (ushort)IPAddress.HostToNetworkOrder((short)value);
            return (ushort)IPAddress.HostToNetworkOrder((short)value);
        }
    }
}
