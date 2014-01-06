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
    public class CNameRecordTests : EqualityComparer
    {
        [TestMethod]
        public void ParseCNameRecordResponseTest()
        {
            var rawResponse = new byte[]
                                  {
                                        122, 65, 133, 0, 0, 1, 0, 1, 0, 7, 0, 7, 3, 119, 119, 119,
                                        5, 121, 97, 104, 111, 111, 3, 99, 111, 109, 0, 0, 5, 0, 1,
                                        192, 12, 0, 5, 0, 1, 0, 0, 1, 44, 0, 15, 6, 102, 100,
                                        45, 102, 112, 51, 3, 119, 103, 49, 1, 98, 192, 16, 192, 16, 0,
                                        2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 56, 192, 16,
                                        192, 16, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                        54, 192, 16, 192, 16, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                        3, 110, 115, 51, 192, 16, 192, 16, 0, 2, 0, 1, 0, 2, 163,
                                        0, 0, 6, 3, 110, 115, 50, 192, 16, 192, 16, 0, 2, 0, 1,
                                        0, 2, 163, 0, 0, 6, 3, 110, 115, 52, 192, 16, 192, 16, 0,
                                        2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 53, 192, 16,
                                        192, 16, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                        49, 192, 16, 192, 178, 0, 1, 0, 1, 0, 18, 117, 0, 0, 4,
                                        68, 180, 131, 16, 192, 124, 0, 1, 0, 1, 0, 18, 117, 0, 0,
                                        4, 68, 142, 255, 16, 192, 106, 0, 1, 0, 1, 0, 18, 117, 0,
                                        0, 4, 203, 84, 221, 53, 192, 142, 0, 1, 0, 1, 0, 18, 117,
                                        0, 0, 4, 98, 138, 11, 157, 192, 160, 0, 1, 0, 1, 0, 18,
                                        117, 0, 0, 4, 119, 160, 247, 124, 192, 88, 0, 1, 0, 1, 0,
                                        2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 70, 0, 1, 0, 1,
                                        0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                                  };

            var reader = new ByteReader(rawResponse);
            new Question(reader); //Advance position

            var rrs = new List<IResourceRecord>();
            rrs.Add(new ResourceRecord(reader));

            var expectedA = new[]
                            {
                                new CNameRecord(null)
                                    {
                                        CName = "fd-fp3.wg1.b.yahoo.com.",
                                    },
                            };

            AssertEquality(expectedA, rrs.Select(rr => rr.Record as CNameRecord));

            var expected = new[]
                               {
                                   new DNS.ExternalConcretes.CNameRecord
                                       {
                                            Class = RecordClass.In,
                                            Name = "www.yahoo.com.",
                                            Ttl = 300,
                                            Type = RecordType.CNameRecord,
                                            CName = "fd-fp3.wg1.b.yahoo.com.",
                                       },
                               };

            AssertEquality(expected, rrs.Select(rr => rr.ConvertToExternalType() as DNS.ExternalConcretes.CNameRecord));
        }
    }
}
