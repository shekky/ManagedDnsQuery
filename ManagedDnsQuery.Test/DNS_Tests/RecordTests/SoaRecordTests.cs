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
