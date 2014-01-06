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
using System.Linq;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingImplementations;
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
