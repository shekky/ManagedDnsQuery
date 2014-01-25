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
using System.Linq;
using System.Net;
using System.Numerics;
using ManagedDnsQuery.SPF.Interfaces;

namespace ManagedDnsQuery.SPF.Concretes
{
    public sealed class NetworkDetails : INetworkDetails
    {
        public IPAddress NetworkAddress { get; set; }
        public IPAddress BroadcastAddress { get; set; }
        public IPAddress SubNetMask { get; set; }
        public IPAddress UsableStartAddress { get; set; }
        public IPAddress UsableEndAddress { get; set; }
        public int MaxHosts { get; set; }
        public int MaxUsableHosts { get; set; }

        /// <summary>
        /// Will Test is an address is in the usable range of a network
        /// excluding the Network Address and the Broadcast Address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>bool Is In Range</returns>
        public bool IsInRangeUsable(IPAddress address)
        {
            var start = IpAddressToBigInt(UsableStartAddress);
            var end = IpAddressToBigInt(UsableEndAddress);
            var current = IpAddressToBigInt(address);

            return current >= start && current <= end;
        }

        /// <summary>
        /// Will Test is an address is in the range of a network
        /// including the Network Address and the Broadcast Address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>bool Is In Range</returns>
        public bool IsInRange(IPAddress address)
        {
            var start = IpAddressToBigInt(NetworkAddress);
            var end = IpAddressToBigInt(BroadcastAddress);
            var current = IpAddressToBigInt(address);

            return current >= start && current <= end;
        }

        private static BigInteger IpAddressToBigInt(IPAddress address)
        {
            var bytes = address.GetAddressBytes().Reverse().ToArray();
            var uintBytes = new byte[bytes.Length + 1];
            Array.Copy(bytes, uintBytes, bytes.Length);

            return new BigInteger(uintBytes);
        }
    }
}
