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
using System.Linq;
using System.Net;
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
            var start = IpAddressToUint(UsableStartAddress);
            var end = IpAddressToUint(UsableEndAddress);
            var current = IpAddressToUint(address);

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
            var start = IpAddressToUint(NetworkAddress);
            var end = IpAddressToUint(BroadcastAddress);
            var current = IpAddressToUint(address);

            return current >= start && current <= end;
        }

        private static uint IpAddressToUint(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            var ip = Convert.ToUInt32(bytes.Skip(0).Take(1).First()) << 24;
            ip += Convert.ToUInt32(bytes.Skip(1).Take(1).First()) << 16;
            ip += Convert.ToUInt32(bytes.Skip(2).Take(1).First()) << 8;
            ip += Convert.ToUInt32(bytes.Skip(3).Take(1).First());

            return ip;
        }
    }
}
