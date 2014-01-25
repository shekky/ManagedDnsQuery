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
    public class NsRecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParseNsResponseTest()
        {
            var rawResponse = new byte[]
                                  {
                                        98, 228, 133, 0, 0, 1, 0, 4, 0, 0, 0, 4, 12, 52, 115, 105,
                                        116, 101, 100, 105, 103, 105, 116, 97, 108, 3, 99, 111, 109, 0, 0,
                                        2, 0, 1, 192, 12, 0, 2, 0, 1, 0, 1, 81, 128, 0, 6,
                                        3, 110, 115, 49, 192, 12, 192, 12, 0, 2, 0, 1, 0, 1, 81,
                                        128, 0, 6, 3, 110, 115, 52, 192, 12, 192, 12, 0, 2, 0, 1,
                                        0, 1, 81, 128, 0, 6, 3, 110, 115, 51, 192, 12, 192, 12, 0,
                                        2, 0, 1, 0, 1, 81, 128, 0, 6, 3, 110, 115, 50, 192, 12,
                                        192, 46, 0, 1, 0, 1, 0, 1, 81, 128, 0, 4, 208, 109, 106,
                                        113, 192, 64, 0, 1, 0, 1, 0, 1, 81, 128, 0, 4, 192, 138,
                                        21, 145, 192, 82, 0, 1, 0, 1, 0, 1, 81, 128, 0, 4, 192,
                                        138, 21, 144, 192, 100, 0, 1, 0, 1, 0, 1, 81, 128, 0, 4,
                                        208, 109, 106, 82
                                  };

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            for (var ndx = 0; ndx < 3; ++ndx)
                rrs.Add(new ResourceRecord(reader));

            var expectedNs = new []
                            {
                                new NsRecord(null)
                                    {
                                        NsDomainName = "ns1.4sitedigital.com.",
                                    },
                                new NsRecord(null)
                                    {
                                        NsDomainName = "ns4.4sitedigital.com.",
                                    },
                                new NsRecord(null)
                                    {
                                        NsDomainName = "ns3.4sitedigital.com.",
                                    },
                            };
            AssertEquality(expectedNs, rrs.Select(rr => rr.Record as NsRecord));

            var expected = new []
                            {
                                new DNS.ExternalConcretes.NsRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "4sitedigital.com.",
                                        Ttl = 86400,
                                        Type = RecordType.NsRecord,
                                        NsDomainName = "ns1.4sitedigital.com."
                                    },
                                new DNS.ExternalConcretes.NsRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "4sitedigital.com.",
                                        Ttl = 86400,
                                        Type = RecordType.NsRecord,
                                        NsDomainName = "ns4.4sitedigital.com."
                                    },
                                new DNS.ExternalConcretes.NsRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "4sitedigital.com.",
                                        Ttl = 86400,
                                        Type = RecordType.NsRecord,
                                        NsDomainName = "ns3.4sitedigital.com."
                                    },
                            };
            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.NsRecord));
        }
    }
}
