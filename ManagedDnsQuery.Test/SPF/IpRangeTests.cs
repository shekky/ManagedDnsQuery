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
