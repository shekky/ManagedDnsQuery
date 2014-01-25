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
using System.Net;
using ManagedDnsQuery.SPF.Concretes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test.SPF
{
    [TestClass]
    public class IpRangeTests : EqualityComparer
    {
        [TestMethod]
        public void TestParseIpv4Ranges()
        {
            var parser = new NetworkParser();
            var expected = new NetworkDetails
                               {
                                   BroadcastAddress = IPAddress.Parse("192.168.1.7"),
                                   NetworkAddress = IPAddress.Parse("192.168.1.0"),
                                   SubNetMask = IPAddress.Parse("255.255.255.248"),
                                   UsableStartAddress = IPAddress.Parse("192.168.1.1"),
                                   UsableEndAddress = IPAddress.Parse("192.168.1.6"),
                                   MaxHosts = 8,
                                   MaxUsableHosts = 6
                               };

            var actual = parser.ParseRange("192.168.1.5 /29");
            AssertEquality(expected, actual);

            Assert.IsTrue(actual.IsInRange(IPAddress.Parse("192.168.1.5")));
            Assert.IsTrue(actual.IsInRangeUsable(IPAddress.Parse("192.168.1.5")));
            Assert.IsTrue(actual.IsInRange(IPAddress.Parse("192.168.1.0")));
            Assert.IsFalse(actual.IsInRangeUsable(IPAddress.Parse("192.168.1.0")));
            Assert.IsFalse(actual.IsInRange(IPAddress.Parse("192.168.1.25")));
            Assert.IsFalse(actual.IsInRangeUsable(IPAddress.Parse("192.168.1.25")));
        }

        [TestMethod]
        public void TestParseIpv6Ranges()
        {
            var parser = new NetworkParser();

            var expected = new NetworkDetails
                               {
                                   BroadcastAddress = IPAddress.Parse("2001:db8:85a3::8a2e:370:733f"),
                                   NetworkAddress = IPAddress.Parse("2001:db8:85a3::8a2e:370:7330"),
                                   SubNetMask = IPAddress.Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:fff0"),
                                   UsableEndAddress = IPAddress.Parse("2001:db8:85a3::8a2e:370:733e"),
                                   UsableStartAddress = IPAddress.Parse("2001:db8:85a3::8a2e:370:7331"),
                                   MaxHosts = 16,
                                   MaxUsableHosts = 14,
                               };

            var actual = parser.ParseRange("2001:0db8:85a3::8a2e:0370:7334 /124");
            AssertEquality(expected, actual);

            Assert.IsTrue(actual.IsInRange(IPAddress.Parse("2001:db8:85a3::8a2e:370:733f")));
            Assert.IsFalse(actual.IsInRangeUsable(IPAddress.Parse("2001:db8:85a3::8a2e:370:733f")));
            Assert.IsTrue(actual.IsInRange(IPAddress.Parse("2001:db8:85a3::8a2e:370:733e")));
            Assert.IsTrue(actual.IsInRangeUsable(IPAddress.Parse("2001:db8:85a3::8a2e:370:733e")));
            Assert.IsFalse(actual.IsInRange(IPAddress.Parse("2001:db8:85a3::8a2e:370:7340")));
            Assert.IsFalse(actual.IsInRangeUsable(IPAddress.Parse("2001:db8:85a3::8a2e:370:7340")));
            Assert.IsTrue(actual.IsInRange(IPAddress.Parse("2001:db8:85a3::8a2e:370:733A"))); 
            Assert.IsTrue(actual.IsInRangeUsable(IPAddress.Parse("2001:db8:85a3::8a2e:370:733A")));
            Assert.IsFalse(actual.IsInRange(IPAddress.Parse("2001:db8:85a3::8a2e:370:732F")));
            Assert.IsFalse(actual.IsInRangeUsable(IPAddress.Parse("2001:db8:85a3::8a2e:370:732f")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadDataParseGiberish()
        {
            var parser = new NetworkParser();
            parser.ParseRange("Giberish");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadDataParseMaskLength()
        {
            var parser = new NetworkParser();
            parser.ParseRange("192.168.1.5 ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadDataParseMaskLength2()
        {
            var parser = new NetworkParser();
            parser.ParseRange("192.168.1.5 /");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadDataParseIp()
        {
            var parser = new NetworkParser();
            parser.ParseRange("192.168.1. /29");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadDataParseNullRange()
        {
            var parser = new NetworkParser();
            parser.ParseRange(null);
        }
    }
}
