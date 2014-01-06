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

namespace ManagedDnsQuery.DNS
{
    internal static class BitArrayExtensions
    {
        internal static BitArray ToBits(this IEnumerable<byte> value)
        {
            return new BitArray(value.ToArray());
        }

        internal static BitArray ToBits(this ushort value)
        {
            return BitConverter.GetBytes(value).ToBits();
        }

        internal static ushort ToUShort(this IList<bool> value)
        {
            if (value == null || value.Count() > 8)
                throw new Exception("Invlid Bits, Cannont convert less than 1, or greater than 8 bits to a Byte.");

            var tempBits = new BitArray(8);
            for (var ndx = 0; ndx < value.Count(); ++ndx)
                tempBits.Set(ndx, value[ndx]);

            return tempBits.ToBytes().FirstOrDefault();
        }

        internal static ushort ToUShort(this BitArray values)
        {
            return BitConverter.ToUInt16(values.ToBytes().ToArray(), 0);
        }

        internal static IEnumerable<byte> ToBytes(this BitArray value)
        {
            var ret = new byte[value.Length / 8];
            value.CopyTo(ret, 0);

            return ret;
        }
    }
}
