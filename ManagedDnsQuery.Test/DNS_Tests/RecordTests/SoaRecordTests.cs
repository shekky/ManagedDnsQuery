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
    public class SoaRecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParseSoaRecordResponseTest()
        {
            var rawResponse = new byte[]
                                  {
                                        145, 23, 129, 128, 0, 1, 0, 1, 0, 0, 0, 0, 6, 103, 111, 111,
                                        103, 108, 101, 3, 99, 111, 109, 0, 0, 6, 0, 1, 192, 12, 0,
                                        6, 0, 1, 0, 1, 81, 128, 0, 38, 3, 110, 115, 49, 192, 12,
                                        9, 100, 110, 115, 45, 97, 100, 109, 105, 110, 192, 12, 119, 253, 203,
                                        20, 0, 0, 28, 32, 0, 0, 7, 8, 0, 18, 117, 0, 0, 0, 1, 44
                                  };

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            rrs.Add(new ResourceRecord(reader));

            var expectedA = new[]
                            {
                                new SoaRecord(null)
                                    {
                                        Expire = 1209600,
                                        Minimum = 300,
                                        MName = "ns1.google.com.",
                                        Refresh = 7200,
                                        Retry = 1800,
                                        RName = "dns-admin.google.com.",
                                        Serial = 2013121300,
                                    },
                            };

            AssertEquality(expectedA, rrs.Select(rr => rr.Record as SoaRecord));

            var expected = new[]
                            {
                                new DNS.ExternalConcretes.SoaRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "google.com.",
                                        Ttl = 86400,
                                        Type = RecordType.SoaRecord,
                                        Expire = 1209600,
                                        Minimum = 300,
                                        MName = "ns1.google.com.",
                                        Refresh = 7200,
                                        Retry = 1800,
                                        RName = "dns-admin.google.com.",
                                        Serial = 2013121300,
                                    },
                            };

            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.SoaRecord));
        }
    }
}
