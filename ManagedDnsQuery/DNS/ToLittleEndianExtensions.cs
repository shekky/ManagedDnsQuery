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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ManagedDnsQuery.DNS
{
    internal static class ToLittleEndianExtensions
    {
        internal static ushort ToLeUShort(this IEnumerable<byte> value)
        {
            return (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(value.ToArray(), 0));
        }

        internal static uint ToLeUInt(this IEnumerable<byte> value)
        {
            return (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(value.ToArray(), 0));
        }

        internal static BitArray ToLeBits(this IEnumerable<byte> value)
        {
            return value.ToLeUShort().ToBits();
        }
    }
}
