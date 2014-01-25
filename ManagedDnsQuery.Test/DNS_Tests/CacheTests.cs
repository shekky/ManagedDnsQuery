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
