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
using ManagedDnsQuery.WHOIS.Concretes;
using ManagedDnsQuery.WHOIS.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    [TestClass]
    public class ResolverWhoisTests
    {
        private const string FirstPassString = "TestFirstResultPass: This is Fake / mocked whois data";
        private const string SecondPassString = "TestSecondResultPass: This is Fake / mocked whois data";

        [TestMethod]
        public void TestWhoisQuery()
        {
            var transport = GetMockedWhoisTransport();
            var parser = GetMockedItldParser();

            IResolver resolver = new Resolver(null, null, transport.Object, new TldHandler(parser.Object));
            var result = resolver.QueryWhois("google.com");

            Assert.IsFalse(string.IsNullOrEmpty(result), "Should have returned a string.");
            Assert.AreEqual(SecondPassString, result, "Should be equal");

            result = resolver.QueryWhois("yahoo.com");

            Assert.IsFalse(string.IsNullOrEmpty(result), "Should have returned a string.");
            Assert.AreEqual(FirstPassString, result, "Should be equal");
        }

        private static Mock<IWhoisTransport> GetMockedWhoisTransport()
        {
            var transport = new Mock<IWhoisTransport>();

            transport.Setup(tr => tr.RunWhoisQuery(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string domain, string srv) => GetTestWhoisData(domain, srv));

            return transport;
        }

        private static Mock<ITLDParser> GetMockedItldParser()
        {
            var parser = new Mock<ITLDParser>();

            parser.Setup(prs => prs.Parse(It.IsAny<string>()))
                .Returns(new Dictionary<string, string>
                             {
                                 { "com", "whois.crsnic.net" },
                             });

            parser.Setup(prs => prs.ParseWhoisServer(It.IsAny<string>()))
                .Returns((string firstResult) => GetTestWhoisServer(firstResult));

            return parser;
        }

        private static string GetTestWhoisData(string domain, string srv)
        {
            var datas = new []
                {
                    new
                    {
                        domain = "google.com",
                        server = "whois.crsnic.net",
                        data = FirstPassString,
                    },
                    new
                    {
                        domain = "google.com",
                        server = "whois.verisign-grs.com",
                        data = SecondPassString,
                    },
                    new
                    {
                        domain = "yahoo.com",
                        server = "whois.crsnic.net",
                        data = FirstPassString,
                    },
                };

            var temp = datas.FirstOrDefault(dt => dt.domain == domain && dt.server == srv);
            return temp != null ? temp.data : null;
        }

        private static string GetTestWhoisServer(string firstResult)
        {
            var pairs = new Dictionary<string, string>
                            {
                                { FirstPassString, "whois.verisign-grs.com" },
                            };

            return pairs[firstResult];
        }
    }
}
