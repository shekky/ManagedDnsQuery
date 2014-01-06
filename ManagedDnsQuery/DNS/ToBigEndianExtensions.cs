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
