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
