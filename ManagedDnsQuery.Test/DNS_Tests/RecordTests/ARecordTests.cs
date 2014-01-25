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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.DNS.RDataConcretes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test.DNS_Tests.RecordTests
{
    [TestClass]
    public class ARecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParseARecordResponseTest()
        {
            var rawResponse = new byte[]
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

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            for (var ndx = 0; ndx < 3; ++ndx)
                rrs.Add(new ResourceRecord(reader));

            var expectedA = new []
                            {
                                new ARecord(null)
                                    {
                                        Address = IPAddress.Parse("206.190.36.45"),
                                    },
                                new ARecord(null)
                                    {
                                        Address = IPAddress.Parse("98.139.183.24"),
                                    },
                                new ARecord(null)
                                    {
                                        Address = IPAddress.Parse("98.138.253.109"),
                                    },
                            };

            AssertEquality(expectedA, rrs.Select(rr => rr.Record as ARecord));

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

            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.ARecord));
        }
    }
}
