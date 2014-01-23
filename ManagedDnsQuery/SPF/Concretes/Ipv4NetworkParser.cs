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
using System.Net.Sockets;
using ManagedDnsQuery.SPF.Interfaces;

namespace ManagedDnsQuery.SPF.Concretes
{
    internal sealed class Ipv4NetworkParser : INetworkParser
    {
        public INetworkDetails ParseRange(string range)
        {
            if (string.IsNullOrEmpty(range))
                throw new ArgumentException("Range cannot be null or emtpy.");

            var addressPeices = range.Split('/');
            if (addressPeices.Length != 2)
                throw new ArgumentException(string.Format("Range not formatted correctly. Expecting \"127.0.0.1 /32\" format. '{0}' is invalid.", range));

            IPAddress ip = null;
            if (!IPAddress.TryParse(addressPeices.FirstOrDefault().Trim(), out ip))
                throw new ArgumentException(string.Format("Range not formatted correctly. Expecting \"127.0.0.1 /32\" format. '{0}' is not a valid Ip Address.", addressPeices.FirstOrDefault().Trim()));

            var maskLength = 0;
            if (!int.TryParse(addressPeices.Skip(1).FirstOrDefault().Trim(), out maskLength))
                throw new ArgumentException(string.Format("Range not formatted correctly. Expecting \"127.0.0.1 /32\" format. '{0}' is not a valid Subnet Mask Length.", addressPeices.Skip(1).FirstOrDefault().Trim()));

            if(ip.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException(string.Format("Invalid IPAddress type: {0}, was expecting IPV4.", ip.AddressFamily));

            return ParseIpv4Range(ip, maskLength);
        }

        private INetworkDetails ParseIpv4Range(IPAddress ip, int maskLength)
        {
            var rawIpBytes = ip.GetAddressBytes();
            var rawMaskBytes = GetIpv4SubnetMask(maskLength).ToArray();

            var rangeStart = new byte[4];
            var rangeEnd = new byte[4];
            for (var ndx = 0; ndx < 4; ++ndx)
            {
                rangeStart[ndx] = (byte)(rawIpBytes[ndx] & rawMaskBytes[ndx]);
                rangeEnd[ndx] = (byte)(rawIpBytes[ndx] | ~rawMaskBytes[ndx]);
            }

            return new NetworkDetails
                            {
                                NetworkAddress = new IPAddress(rangeStart),
                                BroadcastAddress = new IPAddress(rangeEnd),
                                SubNetMask = new IPAddress(rawMaskBytes),
                                MaxHosts = (int)Math.Pow(2, (32 - maskLength)),
                                MaxUsableHosts = (int)Math.Pow(2, (32 - maskLength)) - 2,

                                UsableStartAddress = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}",
                                                                         rangeStart.FirstOrDefault(),
                                                                         rangeStart.Skip(1).FirstOrDefault(),
                                                                         rangeStart.Skip(2).FirstOrDefault(),
                                                                         rangeStart.Skip(3).FirstOrDefault())),

                                UsableEndAddress = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}",
                                                                         rangeEnd.FirstOrDefault(),
                                                                         rangeEnd.Skip(1).FirstOrDefault(),
                                                                         rangeEnd.Skip(2).FirstOrDefault(),
                                                                         rangeEnd.Skip(3).FirstOrDefault())),
                            };
        }

        private IEnumerable<byte> GetIpv4SubnetMask(int ones)
        {
            var bits = new BitArray(32);
            bits.SetAll(true);

            var zeros = 32 - ones;
            for (var ndx = 0; ndx < zeros; ++ndx)
                bits[ndx] = false;

            var bytes = new byte[4];
            bits.CopyTo(bytes, 0);

            return BitConverter.IsLittleEndian ? bytes.Reverse() : bytes;
        }
    }
}
