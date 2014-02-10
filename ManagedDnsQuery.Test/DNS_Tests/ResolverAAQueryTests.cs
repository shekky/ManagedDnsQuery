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
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.ExternalConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    [TestClass]
    public class ResolverAAQueryTests : EqualityComparer
    {
        private IQueryCache testCache;

        [TestInitialize]
        public void initTests()
        {
            if(testCache == null)
                testCache = new QueryCache();
        }

        [TestMethod]
        public void TestAuthQueryARecord()
        {
            var mockedTransport = GetMockedTransport();
            IResolver resolver = new Resolver(mockedTransport.Object, testCache);

            var expected = new []
                               {
                                   new ARecord
                                      {
                                          Name = "yahoo.com.",
                                          Class = RecordClass.In,
                                          Ttl = 1800,
                                          Type = RecordType.ARecord,
                                          Address = IPAddress.Parse("206.190.36.45"),
                                      },
                                   new ARecord
                                      {
                                          Name = "yahoo.com.",
                                          Class = RecordClass.In,
                                          Ttl = 1800,
                                          Type = RecordType.ARecord,
                                          Address = IPAddress.Parse("98.139.183.24"),
                                      },
                                   new ARecord
                                      {
                                          Name = "yahoo.com.",
                                          Class = RecordClass.In,
                                          Ttl = 1800,
                                          Type = RecordType.ARecord,
                                          Address = IPAddress.Parse("98.138.253.109"),
                                      },
                               };

            var actual = resolver.AuthoratativeQuery("yahoo.com", RecordType.ARecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(true, actual.Header.AuthoritativeAnswer, "Should have returned from SOA");
            AssertEquality(expected, actual.Answers.ToACollection());

            //Test Caching
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new DNS.MessageingConcretes.Message());

            expected = actual.Answers.ToACollection().ToArray();
            actual = resolver.AuthoratativeQuery("yahoo.com", RecordType.ARecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(false, actual.Header.AuthoritativeAnswer, "Should have returned from Cache");
            AssertEquality(expected, actual.Answers.ToACollection());
        }

        [TestMethod]
        public void TestAuthQueryMxRecord()
        {
            var mockedTransport = GetMockedTransport();
            IResolver resolver = new Resolver(mockedTransport.Object, testCache);

            var expected = new []
                               {
                                   new MxRecord
                                       {
                                           Name = "yahoo.com.",
                                           Class = RecordClass.In,
                                           Ttl = 1800,
                                           Type = RecordType.MxRecord,
                                           Preference = 1,
                                           Exchanger = "mta6.am0.yahoodns.net.",
                                       },
                                    new MxRecord
                                       {
                                           Name = "yahoo.com.",
                                           Class = RecordClass.In,
                                           Ttl = 1800,
                                           Type = RecordType.MxRecord,
                                           Preference = 1,
                                           Exchanger = "mta7.am0.yahoodns.net.",
                                       },
                                    new MxRecord
                                       {
                                           Name = "yahoo.com.",
                                           Class = RecordClass.In,
                                           Ttl = 1800,
                                           Type = RecordType.MxRecord,
                                           Preference = 1,
                                           Exchanger = "mta5.am0.yahoodns.net.",
                                       },
                               };

            var actual = resolver.AuthoratativeQuery("yahoo.com", RecordType.MxRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(true, actual.Header.AuthoritativeAnswer, "Should have returned from SOA");
            AssertEquality(expected, actual.Answers.ToMxCollection());

            //Test Caching
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new DNS.MessageingConcretes.Message());

            expected = actual.Answers.ToMxCollection().ToArray();
            actual = resolver.AuthoratativeQuery("yahoo.com", RecordType.MxRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(false, actual.Header.AuthoritativeAnswer, "Should have returned from Cache");
            AssertEquality(expected, actual.Answers.ToMxCollection());
        }

        [TestMethod]
        public void TestAuthCnameQuery()
        {
            var mockedTransport = GetMockedTransport();
            IResolver resolver = new Resolver(mockedTransport.Object, testCache);

            var expected = new []
                               {
                                   new CNameRecord
                                       {
                                           Class = RecordClass.In,
                                           CName = "fd-fp3.wg1.b.yahoo.com.",
                                           Name = "www.yahoo.com.",
                                           Ttl = 300,
                                           Type = RecordType.CNameRecord,
                                       },
                               };

            var actual = resolver.AuthoratativeQuery("www.yahoo.com", RecordType.CNameRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(true, actual.Header.AuthoritativeAnswer, "Should have returned from SOA");
            AssertEquality(expected, actual.Answers.ToCNameCollection());

            //Test Caching
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new DNS.MessageingConcretes.Message());

            expected = actual.Answers.ToCNameCollection().ToArray();
            actual = resolver.AuthoratativeQuery("www.yahoo.com", RecordType.CNameRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(false, actual.Header.AuthoritativeAnswer, "Should have returned from Cache");
            AssertEquality(expected, actual.Answers.ToCNameCollection());
        }

        [TestMethod]
        public void TestAuthNsQuery()
        {
            var mockedTransport = GetMockedTransport();
            IResolver resolver = new Resolver(mockedTransport.Object, testCache);

            var expected = new[]
                               {
                                   new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns4.yahoo.com."
                                       },
                                    new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns2.yahoo.com."
                                       },
                                    new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns3.yahoo.com."
                                       },
                                    new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns8.yahoo.com."
                                       },
                                    new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns6.yahoo.com."
                                       },
                                    new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns1.yahoo.com."
                                       },
                                    new NsRecord
                                       {
                                           Class = RecordClass.In,
                                           Name = "yahoo.com.",
                                           Ttl = 172800,
                                           Type = RecordType.NsRecord,
                                           NsDomainName = "ns5.yahoo.com."
                                       },
                               };

            var actual = resolver.AuthoratativeQuery("yahoo.com", RecordType.NsRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(true, actual.Header.AuthoritativeAnswer, "Should have returned from SOA");
            AssertEquality(expected, actual.Answers.ToNsCollection());

            //Test Caching
            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns(new DNS.MessageingConcretes.Message());

            expected = actual.Answers.ToNsCollection().ToArray();
            actual = resolver.AuthoratativeQuery("yahoo.com", RecordType.NsRecord, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53));
            Assert.AreEqual(false, actual.Header.AuthoritativeAnswer, "Should have returned from Cache");
            AssertEquality(expected, actual.Answers.ToNsCollection());
        }

        private Mock<IDnsTransport> GetMockedTransport()
        {
            var mockedTransport = new Mock<IDnsTransport>();

            mockedTransport.Setup(tr => tr.SendRequest(It.IsAny<IMessage>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
                .Returns((IMessage mes, IPEndPoint srv, int timeout) => new DNS.MessageingConcretes.Message(dnsData[string.Format("{0}{1}{2}", mes.Questions.FirstOrDefault().QName.ToLower(), mes.Questions.FirstOrDefault().QClass, mes.Questions.FirstOrDefault().QType)]));

            return mockedTransport;
        }

        private readonly IDictionary<string, IEnumerable<byte>> dnsData
            = new Dictionary<string, IEnumerable<byte>>
                  {
                      {
                          "yahoo.com.InSoaRecord", //Yahoo SOA from OpenDns 208.67.222.222 and 208.67.220.220
                          new byte []
                              {
                                  91, 33, 129, 128, 0, 1, 0, 1, 0, 0, 0, 0, 5, 121, 97, 104,
                                  111, 111, 3, 99, 111, 109, 0, 0, 6, 0, 1, 192, 12, 0, 6,
                                  0, 1, 0, 0, 2, 76, 0, 49, 3, 110, 115, 49, 192, 12, 10,
                                  104, 111, 115, 116, 109, 97, 115, 116, 101, 114, 9, 121, 97, 104, 111,
                                  111, 45, 105, 110, 99, 192, 18, 120, 11, 95, 66, 0, 0, 14, 16,
                                  0, 0, 1, 44, 0, 27, 175, 128, 0, 0, 2, 88
                              }
                       },
                       {
                           "ns1.yahoo.com.InARecord", //A record for ns1.yahoo.com from OpenDns 208.67.222.222 and 208.67.220.220
                           new byte []
                               {
                                    91, 35, 129, 128, 0, 1, 0, 1, 0, 0, 0, 0, 3, 110, 115, 49,
                                    5, 121, 97, 104, 111, 111, 3, 99, 111, 109, 0, 0, 1, 0, 1,
                                    192, 12, 0, 1, 0, 1, 0, 9, 58, 118, 0, 4, 68, 180, 131, 16
                               }
                       },
                       {
                           "yahoo.com.InARecord", //A record for yahoo.com from ns1.yahoo.com / 68.180.131.16
                           new byte []
                               {
                                    91, 36, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                    111, 111, 3, 99, 111, 109, 0, 0, 1, 0, 1, 192, 12, 0, 1,
                                    0, 1, 0, 0, 7, 8, 0, 4, 206, 190, 36, 45, 192, 12, 0,
                                    1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 139, 183, 24, 192, 12,
                                    0, 1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 138, 253, 109, 192,
                                    12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 50,
                                    192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                    110, 115, 54, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0,
                                    0, 6, 3, 110, 115, 56, 192, 12, 192, 12, 0, 2, 0, 1, 0,
                                    2, 163, 0, 0, 6, 3, 110, 115, 52, 192, 12, 192, 12, 0, 2,
                                    0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 49, 192, 12, 192,
                                    12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 51,
                                    192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                    110, 115, 53, 192, 12, 192, 159, 0, 1, 0, 1, 0, 18, 117, 0,
                                    0, 4, 68, 180, 131, 16, 192, 87, 0, 1, 0, 1, 0, 18, 117,
                                    0, 0, 4, 68, 142, 255, 16, 192, 177, 0, 1, 0, 1, 0, 18,
                                    117, 0, 0, 4, 203, 84, 221, 53, 192, 141, 0, 1, 0, 1, 0,
                                    18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 195, 0, 1, 0, 1,
                                    0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 105, 0, 1, 0,
                                    1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 123, 0, 1,
                                    0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                               }
                       },
                       {
                           "yahoo.com.InMxRecord", //Mx records for yahoo.com from ns1.yahoo.com / 68.180.131.16
                           new byte []
                               {
                                    91, 37, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                    111, 111, 3, 99, 111, 109, 0, 0, 15, 0, 1, 192, 12, 0, 15,
                                    0, 1, 0, 0, 7, 8, 0, 25, 0, 1, 4, 109, 116, 97, 54,
                                    3, 97, 109, 48, 8, 121, 97, 104, 111, 111, 100, 110, 115, 3, 110,
                                    101, 116, 0, 192, 12, 0, 15, 0, 1, 0, 0, 7, 8, 0, 9,
                                    0, 1, 4, 109, 116, 97, 55, 192, 46, 192, 12, 0, 15, 0, 1,
                                    0, 0, 7, 8, 0, 9, 0, 1, 4, 109, 116, 97, 53, 192, 46,
                                    192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                    56, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                    3, 110, 115, 51, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163,
                                    0, 0, 6, 3, 110, 115, 49, 192, 12, 192, 12, 0, 2, 0, 1,
                                    0, 2, 163, 0, 0, 6, 3, 110, 115, 52, 192, 12, 192, 12, 0,
                                    2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 54, 192, 12,
                                    192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                    50, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                    3, 110, 115, 53, 192, 12, 192, 154, 0, 1, 0, 1, 0, 18, 117,
                                    0, 0, 4, 68, 180, 131, 16, 192, 208, 0, 1, 0, 1, 0, 18,
                                    117, 0, 0, 4, 68, 142, 255, 16, 192, 136, 0, 1, 0, 1, 0,
                                    18, 117, 0, 0, 4, 203, 84, 221, 53, 192, 172, 0, 1, 0, 1,
                                    0, 18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 226, 0, 1, 0,
                                    1, 0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 190, 0, 1,
                                    0, 1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 118, 0,
                                    1, 0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                               }
                       },
                       {
                           "www.yahoo.com.InCNameRecord", //CName record for ns1.yahoo.com from ns1.yahoo.com / 68.180.131.16
                           new byte []
                               {
                                    91, 38, 133, 0, 0, 1, 0, 1, 0, 7, 0, 7, 3, 119, 119, 119,
                                    5, 121, 97, 104, 111, 111, 3, 99, 111, 109, 0, 0, 5, 0, 1,
                                    192, 12, 0, 5, 0, 1, 0, 0, 1, 44, 0, 15, 6, 102, 100,
                                    45, 102, 112, 51, 3, 119, 103, 49, 1, 98, 192, 16, 192, 16, 0,
                                    2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 50, 192, 16,
                                    192, 16, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                    52, 192, 16, 192, 16, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                    3, 110, 115, 49, 192, 16, 192, 16, 0, 2, 0, 1, 0, 2, 163,
                                    0, 0, 6, 3, 110, 115, 56, 192, 16, 192, 16, 0, 2, 0, 1,
                                    0, 2, 163, 0, 0, 6, 3, 110, 115, 53, 192, 16, 192, 16, 0,
                                    2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 51, 192, 16,
                                    192, 16, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                    54, 192, 16, 192, 106, 0, 1, 0, 1, 0, 18, 117, 0, 0, 4,
                                    68, 180, 131, 16, 192, 70, 0, 1, 0, 1, 0, 18, 117, 0, 0,
                                    4, 68, 142, 255, 16, 192, 160, 0, 1, 0, 1, 0, 18, 117, 0,
                                    0, 4, 203, 84, 221, 53, 192, 88, 0, 1, 0, 1, 0, 18, 117,
                                    0, 0, 4, 98, 138, 11, 157, 192, 142, 0, 1, 0, 1, 0, 18,
                                    117, 0, 0, 4, 119, 160, 247, 124, 192, 178, 0, 1, 0, 1, 0,
                                    2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 124, 0, 1, 0, 1,
                                    0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                               }
                       },
                       {
                           "yahoo.com.InNsRecord", // record for ns1.yahoo.com from ns1.yahoo.com / 68.180.131.16
                           new byte []
                               {
                                    91, 39, 133, 0, 0, 1, 0, 7, 0, 0, 0, 7, 5, 121, 97, 104,
                                    111, 111, 3, 99, 111, 109, 0, 0, 2, 0, 1, 192, 12, 0, 2,
                                    0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 52, 192, 12, 192,
                                    12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 50,
                                    192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                    110, 115, 51, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0,
                                    0, 6, 3, 110, 115, 56, 192, 12, 192, 12, 0, 2, 0, 1, 0,
                                    2, 163, 0, 0, 6, 3, 110, 115, 54, 192, 12, 192, 12, 0, 2,
                                    0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 49, 192, 12, 192,
                                    12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 53,
                                    192, 12, 192, 129, 0, 1, 0, 1, 0, 18, 117, 0, 0, 4, 68,
                                    180, 131, 16, 192, 57, 0, 1, 0, 1, 0, 18, 117, 0, 0, 4,
                                    68, 142, 255, 16, 192, 75, 0, 1, 0, 1, 0, 18, 117, 0, 0,
                                    4, 203, 84, 221, 53, 192, 39, 0, 1, 0, 1, 0, 18, 117, 0,
                                    0, 4, 98, 138, 11, 157, 192, 147, 0, 1, 0, 1, 0, 18, 117,
                                    0, 0, 4, 119, 160, 247, 124, 192, 111, 0, 1, 0, 1, 0, 2,
                                    163, 0, 0, 4, 202, 43, 223, 170, 192, 93, 0, 1, 0, 1, 0,
                                    2, 163, 0, 0, 4, 202, 165, 104, 22
                               }
                       },
                  };
    }
}