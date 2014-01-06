﻿/**********************************************************************************
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
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingImplementations;
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
