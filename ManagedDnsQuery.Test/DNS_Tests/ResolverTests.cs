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

using System.Collections.Generic;
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingImplementations;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    [TestClass]
    public class ResolverTests : EqualityComparer
    {
        [TestMethod]
        public void TestQueryARecord()
        {
            var mockedTransport = new Mock<IDnsTransport>();
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new Message(GetRawTestData(RecordType.ARecord)));

            IResolver resolver = new Resolver(mockedTransport.Object);

            var expected = new []
                            {
                                new DNS.ExternalConcretes.ARecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "yahoo.com.",
                                        Ttl = 1800,
                                        Type = RecordType.ARecord,
                                        Address = IPAddress.Parse("206.190.36.45"),
                                    },
                                new DNS.ExternalConcretes.ARecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "yahoo.com.",
                                        Ttl = 1800,
                                        Type = RecordType.ARecord,
                                        Address = IPAddress.Parse("98.139.183.24"),
                                    },
                                new DNS.ExternalConcretes.ARecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "yahoo.com.",
                                        Ttl = 1800,
                                        Type = RecordType.ARecord,
                                        Address = IPAddress.Parse("98.138.253.109"),
                                    },
                            };
            
            var actual = resolver.Query("yahoo.com", RecordType.ARecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53)).Answers.ToACollection();
            AssertEquality(expected, actual);


            //Test Cache
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new Message());

            var expected1 = actual;
            actual = resolver.Query("yahoo.com", RecordType.ARecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53)).Answers.ToACollection();
            AssertEquality(expected1, actual);
        }

        [TestMethod]
        public void TestQueryMxRecord()
        {
            var mockedTransport = new Mock<IDnsTransport>();
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new Message(GetRawTestData(RecordType.MxRecord)));

            IResolver resolver = new Resolver(mockedTransport.Object);

            var expected = new[]
                            {
                                new DNS.ExternalConcretes.MxRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "yahoo.com.",
                                        Ttl = 1800,
                                        Type = RecordType.MxRecord,
                                        Preference = 1,
                                        Exchanger = "mta7.am0.yahoodns.net.",
                                    },
                                new DNS.ExternalConcretes.MxRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "yahoo.com.",
                                        Ttl = 1800,
                                        Type = RecordType.MxRecord,
                                        Preference = 1,
                                        Exchanger = "mta6.am0.yahoodns.net.",
                                    },
                                new DNS.ExternalConcretes.MxRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "yahoo.com.",
                                        Ttl = 1800,
                                        Type = RecordType.MxRecord,
                                        Preference = 1,
                                        Exchanger = "mta5.am0.yahoodns.net.",
                                    },
                            };

            var actual = resolver.Query("yahoo.com", RecordType.MxRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53)).Answers.ToMxCollection();
            AssertEquality(expected, actual);


            //Test Cache
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new Message());

            var expected1 = actual;
            actual = resolver.Query("yahoo.com", RecordType.MxRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53)).Answers.ToMxCollection();
            AssertEquality(expected1, actual);
        }

        private IEnumerable<byte> GetRawTestData(RecordType type)
        {
            if(type == RecordType.ARecord)
                return new byte[]
                           {
                                215, 61, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                111, 111, 3, 99, 111, 109, 0, 0, 1, 0, 1, 192, 12, 0, 1,
                                0, 1, 0, 0, 7, 8, 0, 4, 206, 190, 36, 45, 192, 12, 0,
                                1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 139, 183, 24, 192, 12,
                                0, 1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 138, 253, 109, 192,
                                12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 49,
                                192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                110, 115, 53, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0,
                                0, 6, 3, 110, 115, 54, 192, 12, 192, 12, 0, 2, 0, 1, 0,
                                2, 163, 0, 0, 6, 3, 110, 115, 51, 192, 12, 192, 12, 0, 2,
                                0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 50, 192, 12, 192,
                                12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 52,
                                192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                110, 115, 56, 192, 12, 192, 87, 0, 1, 0, 1, 0, 18, 117, 0,
                                0, 4, 68, 180, 131, 16, 192, 159, 0, 1, 0, 1, 0, 18, 117,
                                0, 0, 4, 68, 142, 255, 16, 192, 141, 0, 1, 0, 1, 0, 18,
                                117, 0, 0, 4, 203, 84, 221, 53, 192, 177, 0, 1, 0, 1, 0,
                                18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 105, 0, 1, 0, 1,
                                0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 123, 0, 1, 0,
                                1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 195, 0, 1,
                                0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                           };

            if (type == RecordType.MxRecord)
                return new byte []
                           {
                                41, 163, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                111, 111, 3, 99, 111, 109, 0, 0, 15, 0, 1, 192, 12, 0, 15,
                                0, 1, 0, 0, 7, 8, 0, 25, 0, 1, 4, 109, 116, 97, 55,
                                3, 97, 109, 48, 8, 121, 97, 104, 111, 111, 100, 110, 115, 3, 110,
                                101, 116, 0, 192, 12, 0, 15, 0, 1, 0, 0, 7, 8, 0, 9,
                                0, 1, 4, 109, 116, 97, 54, 192, 46, 192, 12, 0, 15, 0, 1,
                                0, 0, 7, 8, 0, 9, 0, 1, 4, 109, 116, 97, 53, 192, 46,
                                192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                50, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                3, 110, 115, 51, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163,
                                0, 0, 6, 3, 110, 115, 52, 192, 12, 192, 12, 0, 2, 0, 1,
                                0, 2, 163, 0, 0, 6, 3, 110, 115, 49, 192, 12, 192, 12, 0,
                                2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 54, 192, 12,
                                192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                53, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                3, 110, 115, 56, 192, 12, 192, 172, 0, 1, 0, 1, 0, 18, 117,
                                0, 0, 4, 68, 180, 131, 16, 192, 118, 0, 1, 0, 1, 0, 18,
                                117, 0, 0, 4, 68, 142, 255, 16, 192, 136, 0, 1, 0, 1, 0,
                                18, 117, 0, 0, 4, 203, 84, 221, 53, 192, 154, 0, 1, 0, 1,
                                0, 18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 208, 0, 1, 0,
                                1, 0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 190, 0, 1,
                                0, 1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 226, 0,
                                1, 0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                           };

            return null;
        }
    }
}
