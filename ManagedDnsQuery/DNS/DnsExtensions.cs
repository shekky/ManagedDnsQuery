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
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS
{
    internal static class DnsExtensions
    {
        internal static readonly int HeaderSize = 12;

        internal static IEnumerable<byte> ToLabelBytes(this string value)
        {
            var temp = new List<byte>();
            value = value.TryTrim().TrimEnd(new[] {'.'});

            foreach (var peice in value.Split('.'))
            {
                temp.Add((byte)peice.Length);
                temp.AddRange(Encoding.ASCII.GetBytes(peice.TryTrim()));
            }

            temp.Add(0);
            return temp;
        }

        internal static bool IsExpired(this IResourceRecord value)
        {
            var expired = true;

            if (value != null)
                expired = (value.TimeStamp.AddSeconds(value.Ttl).CompareTo(DateTime.Now) < 0);
            
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

        internal static IEnumerable<IQuestion> ToQuestions(this IEnumerable<ExternalInterfaces.IQuestion> values)
        {
            return values.Select(val => new Question(null) {QName = val.QName, QType = val.QType, QClass = val.QClass}).ToArray();
        }
    }
}
