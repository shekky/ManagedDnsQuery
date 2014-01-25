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
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.DNS.RDataConcretes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test.DNS_Tests.RecordTests
{
    [TestClass]
    public class PtrRecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParsePtrRecordResponseTest()
        {
            var rawResponse = new byte[]
                                  {
                                        198, 24, 133, 0, 0, 1, 0, 1, 0, 5, 0, 5, 2, 49, 54, 3,
                                        49, 51, 49, 3, 49, 56, 48, 2, 54, 56, 7, 105, 110, 45, 97,
                                        100, 100, 114, 4, 97, 114, 112, 97, 0, 0, 12, 0, 1, 192, 12,
                                        0, 12, 0, 1, 0, 0, 7, 8, 0, 15, 3, 110, 115, 49, 5,
                                        121, 97, 104, 111, 111, 3, 99, 111, 109, 0, 192, 15, 0, 2, 0,
                                        1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 51, 192, 60, 192, 15,
                                        0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 52, 192,
                                        60, 192, 15, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110,
                                        115, 53, 192, 60, 192, 15, 0, 2, 0, 1, 0, 2, 163, 0, 0,
                                        6, 3, 110, 115, 50, 192, 60, 192, 15, 0, 2, 0, 1, 0, 2,
                                        163, 0, 0, 2, 192, 56, 192, 56, 0, 1, 0, 1, 0, 18, 117,
                                        0, 0, 4, 68, 180, 131, 16, 192, 137, 0, 1, 0, 1, 0, 18,
                                        117, 0, 0, 4, 68, 142, 255, 16, 192, 83, 0, 1, 0, 1, 0,
                                        18, 117, 0, 0, 4, 203, 84, 221, 53, 192, 101, 0, 1, 0, 1,
                                        0, 18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 119, 0, 1, 0,
                                        1, 0, 18, 117, 0, 0, 4, 119, 160, 247, 124
                                  };

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            rrs.Add(new ResourceRecord(reader));

            var expectedA = new []
                            {
                                new PtrRecord(null)
                                    {
                                        DomainName = "ns1.yahoo.com.",
                                    },
                            };

            AssertEquality(expectedA, rrs.Select(rr => rr.Record as PtrRecord));

            var expected = new []
                            {
                                new DNS.ExternalConcretes.PtrRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "16.131.180.68.in-addr.arpa.",
                                        Ttl = 1800,
                                        Type = RecordType.PtrRecord,
                                        DomainName = "ns1.yahoo.com."
                                    },
                            };

            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.PtrRecord));
        }
    }
}
