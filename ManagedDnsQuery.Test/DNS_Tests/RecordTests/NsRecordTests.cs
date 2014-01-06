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
