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
    internal sealed class NetworkParser : INetworkParser
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

            var maskLength = -1;
            if (!int.TryParse(addressPeices.Skip(1).FirstOrDefault().Trim(), out maskLength))
                throw new ArgumentException(string.Format("Range not formatted correctly. Expecting \"127.0.0.1 /32\" format. '{0}' is not a valid Subnet Mask Length.", addressPeices.Skip(1).FirstOrDefault().Trim()));

            if(ip.AddressFamily != AddressFamily.InterNetwork && ip.AddressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException(string.Format("Invalid IPAddress type: {0}, was expecting IPV4 or IPV6", ip.AddressFamily));

            var rawIpBytes = ip.GetAddressBytes();
            var rawMaskBytes = GetSubnetMask(maskLength, ip.AddressFamily == AddressFamily.InterNetwork).ToArray();

            var rangeStart = new byte[rawIpBytes.Length];
            var rangeEnd = new byte[rawIpBytes.Length];

            for (var ndx = 0; ndx < rawIpBytes.Length; ++ndx)
            {
                rangeStart[ndx] = (byte)(rawIpBytes[ndx] & rawMaskBytes[ndx]);
                rangeEnd[ndx] = (byte)(rawIpBytes[ndx] | ~rawMaskBytes[ndx]);
            }

            var hostsCount = Math.Pow(2, ((rawIpBytes.Length == 4 ? 32 : 128) - maskLength));
            return new NetworkDetails
                        {
                            NetworkAddress = new IPAddress(rangeStart),
                            BroadcastAddress = new IPAddress(rangeEnd),
                            SubNetMask = new IPAddress(rawMaskBytes),
                            MaxHosts = (int)hostsCount,
                            MaxUsableHosts = (int)hostsCount - 2,
                            UsableStartAddress = IncrementByOne(new IPAddress(rangeStart)),
                            UsableEndAddress = DecrementByOne(new IPAddress(rangeEnd)),
                        };
        }

        private static IEnumerable<byte> GetSubnetMask(int ones, bool ipv4 = true)
        {
            var zeros = 0;
            BitArray bits = null;

            if(ipv4)
            {
                zeros = 32 - ones;
                bits = new BitArray(32);
            }
            else
            {
                zeros = 128 - ones;
                bits = new BitArray(128);
            }

            bits.SetAll(true);
            for (var ndx = 0; ndx < zeros; ++ndx)
                bits[ndx] = false;

            var bytes = new byte[bits.Length / 8];
            bits.CopyTo(bytes, 0);

            return BitConverter.IsLittleEndian ? bytes.Reverse() : bytes;
        }

        private static IPAddress IncrementByOne(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            if(bytes.Length != 4 && bytes.Length != 16)
                throw new ArgumentException("Not a valid IP Address");
            
            for (var ndx = (bytes.Length -1); ndx > -1; --ndx)
            {
                if (bytes[ndx] < 255)
                {
                    bytes[ndx] += 1;
                    break;
                }
            }
            
            return new IPAddress(bytes);
        }

        private static IPAddress DecrementByOne(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            if (bytes.Length != 4 && bytes.Length != 16)
                throw new ArgumentException("Not a valid IP Address");

            for (var ndx = (bytes.Length - 1); ndx > -1; --ndx)
            {
                if (bytes[ndx] > 0)
                {
                    bytes[ndx] -= 1;
                    break;
                }
            }

            return new IPAddress(bytes);
        }
    }
}
