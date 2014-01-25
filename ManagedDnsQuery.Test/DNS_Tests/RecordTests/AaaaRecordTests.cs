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
    public class AaaaRecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParseAaaaRecordResponseTest()
        {
            var rawResponse = new byte[]
                                  {
                                        134, 17, 129, 128, 0, 1, 0, 1, 0, 0, 0, 0, 6, 103, 111, 111,
                                        103, 108, 101, 3, 99, 111, 109, 0, 0, 28, 0, 1, 192, 12, 0,
                                        28, 0, 1, 0, 0, 0, 178, 0, 16, 38, 7, 248, 176, 64, 9,
                                        8, 4, 0, 0, 0, 0, 0, 0, 16, 14
                                  };

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            rrs.Add(new ResourceRecord(reader));

            var expectedA = new[]
                            {
                                new AaaaRecord(null)
                                    {
                                        Address = IPAddress.Parse("2607:f8b0:4009:804::100e"),
                                    },
                            };

            AssertEquality(expectedA, rrs.Select(rr => rr.Record as AaaaRecord));

            var expected = new[]
                               {
                                   new DNS.ExternalConcretes.AaaaRecord
                                       {
                                            Class = RecordClass.In,
                                            Name = "google.com.",
                                            Ttl = 178,
                                            Type = RecordType.AaaaRecord,
                                            Address = IPAddress.Parse("2607:f8b0:4009:804::100e"),
                                       },
                               };

            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.AaaaRecord));
        }
    }
}
