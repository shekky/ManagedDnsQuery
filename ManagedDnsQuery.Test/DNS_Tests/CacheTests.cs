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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.DNS.RDataConcretes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    [TestClass]
    public class CacheTests : EqualityComparer
    {
        [TestMethod]
        public void TestCache()
        {
            IQueryCache cache = new QueryCache();
            var testMessage = GetTestMessage();

            var message = cache.CheckCache(testMessage.Questions.FirstOrDefault());
            Assert.IsNull(message, "Should be nothing in the cache.");

            Assert.IsTrue(cache.AddCache(testMessage), "Should not fail to add to cache");
            Assert.IsTrue(cache.AddCache(GetTestMessage(false)), "Should not fail to add to cache");

            message = cache.CheckCache(testMessage.Questions.FirstOrDefault());
            Assert.IsNotNull(message, "Should be found, and should not have expired.");
            AssertEquality(testMessage, message);

            testMessage = GetTestMessage(false);
            message = cache.CheckCache(testMessage.Questions.FirstOrDefault());
            Assert.IsNull(message, "Message should have expired and been removed from cache.");
        }

        private IMessage GetTestMessage(bool expired = true)
        {
            return expired
                       ? new Message
                             {
                                 Header = new Header(null),
                                 Questions = new List<IQuestion>
                                                 {
                                                     new Question(null)
                                                         {
                                                             QName = "yahoo.com",
                                                             QType = RecordType.ARecord,
                                                             QClass = RecordClass.In,
                                                         },
                                                 },
                                 Answers = new List<IResourceRecord>
                                               {
                                                   new ResourceRecord(null)
                                                       {
                                                           TimeStamp = DateTime.Now,
                                                           Class = RecordClass.In,
                                                           Name = "yahoo.com.",
                                                           Ttl = 1800,
                                                           Type = RecordType.ARecord,
                                                           RdLength = 3,
                                                           Rdata = new byte[] { 0, 0, 0 },
                                                           Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("206.190.36.45"),
                                                                        },
                                                       },
                                                   new ResourceRecord(null)
                                                       {
                                                           TimeStamp = DateTime.Now,
                                                           Class = RecordClass.In,
                                                           Name = "yahoo.com.",
                                                           Ttl = 1800,
                                                           Type = RecordType.ARecord,
                                                           RdLength = 3,
                                                           Rdata = new byte[] { 0, 0, 0 },
                                                           Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("98.139.183.24"),
                                                                        },
                                                       },
                                               },
                             }
                       : new Message
                             {
                                 Header = new Header(null),
                                 Questions = new List<IQuestion>
                                                 {
                                                     new Question(null)
                                                         {
                                                             QName = "sample.com",
                                                             QType = RecordType.ARecord,
                                                             QClass = RecordClass.In,
                                                         },
                                                 },
                                 Answers = new List<IResourceRecord>
                                               {
                                                   new ResourceRecord(null)
                                                       {
                                                           TimeStamp = DateTime.Now.AddDays(-1),
                                                           Class = RecordClass.In,
                                                           Name = "sample.com.",
                                                           Ttl = 1800,
                                                           Type = RecordType.ARecord,
                                                           RdLength = 3,
                                                           Rdata = new byte[] { 0, 0, 0 },
                                                           Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("206.190.36.45"),
                                                                        },
                                                       },
                                                   new ResourceRecord(null)
                                                       {
                                                           TimeStamp = DateTime.Now.AddDays(-1),
                                                           Class = RecordClass.In,
                                                           Name = "sample.com.",
                                                           Ttl = 1800,
                                                           Type = RecordType.ARecord,
                                                           RdLength = 3,
                                                           Rdata = new byte[] { 0, 0, 0 },
                                                           Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("98.139.183.24"),
                                                                        },
                                                       },
                                               },
                             };
        }
    }
}
