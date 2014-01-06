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
    public class TxtRecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParseSoaRecordResponseTest()
        {
            var rawResponse = new byte[]
                                  {
                                        100, 124, 129, 128, 0, 1, 0, 1, 0, 0, 0, 0, 6, 103, 111, 111,
                                        103, 108, 101, 3, 99, 111, 109, 0, 0, 16, 0, 1, 192, 12, 0,
                                        16, 0, 1, 0, 0, 9, 225, 0, 76, 75, 118, 61, 115, 112, 102,
                                        49, 32, 105, 110, 99, 108, 117, 100, 101, 58, 95, 115, 112, 102, 46,
                                        103, 111, 111, 103, 108, 101, 46, 99, 111, 109, 32, 105, 112, 52, 58,
                                        50, 49, 54, 46, 55, 51, 46, 57, 51, 46, 55, 48, 47, 51, 49,
                                        32, 105, 112, 52, 58, 50, 49, 54, 46, 55, 51, 46, 57, 51, 46,
                                        55, 50, 47, 51, 49, 32, 126, 97, 108, 108
                                  };

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            rrs.Add(new ResourceRecord(reader));

            var expectedA = new[]
                            {
                                new TxtRecord(null)
                                    {
                                        Text = "v=spf1 include:_spf.google.com ip4:216.73.93.70/31 ip4:216.73.93.72/31 ~all",
                                    },
                            };

            AssertEquality(expectedA, rrs.Select(rr => rr.Record as TxtRecord));

            var expected = new[]
                            {
                                new DNS.ExternalConcretes.TxtRecord
                                    {
                                        Class = RecordClass.In,
                                        Name = "google.com.",
                                        Ttl = 2529,
                                        Type = RecordType.TxtRecord,
                                        Text = "v=spf1 include:_spf.google.com ip4:216.73.93.70/31 ip4:216.73.93.72/31 ~all",
                                    },
                            };

            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.TxtRecord));
        }
    }
}
