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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS
{
    internal static class DnsExtensions
    {
        internal static readonly int HeaderSize = 12;

        internal static IEnumerable<byte> ToLabelBytes(this string value)
        {
            var temp = new List<byte>();
            value = value.Trim().TrimEnd(new[] {'.'});

            foreach (var peice in value.Split('.'))
            {
                temp.Add((byte)peice.Length);
                temp.AddRange(Encoding.ASCII.GetBytes(peice.Trim()));
            }

            temp.Add(0);
            return temp;
        }

        internal static string ToByteString(this IEnumerable<byte> value)
        {
            var sb = new StringBuilder();
            foreach (var b in value)
                sb.Append(b);

            return sb.ToString();
        }

        internal static bool IsExpired(this IMessage value)
        {
            var expired = true;

            if (value != null && value.Answers != null && value.Answers.Any())
                expired = value.Answers.Select(ans => ans.TimeStamp.AddSeconds(ans.Ttl)).Any(dt => dt <= DateTime.Now);

            return expired;
        }

        internal static string ToArpa(this string ip)
        {
            IPAddress scratch = null;
            if (!IPAddress.TryParse(ip.TrimEnd(new [] {'.'}), out scratch))
                return string.Empty;

            var sb = new StringBuilder();
            if(scratch.AddressFamily == AddressFamily.InterNetwork)
            {
                sb.Append("in-addr.arpa.");
                foreach (var block in scratch.GetAddressBytes())
                    sb.Insert(0, string.Format("{0}.", block));
            }

            if (scratch.AddressFamily == AddressFamily.InterNetworkV6)
            {
                sb.Append("ip6.arpa.");
                foreach (var block in scratch.GetAddressBytes())
                {
                    sb.Insert(0, string.Format("{0:x}.", (block >> 4) & 0xf));
                    sb.Insert(0, string.Format("{0:x}.", (block >> 0) & 0xf));
                }
            }

            return sb.ToString();
        }
    }
}
