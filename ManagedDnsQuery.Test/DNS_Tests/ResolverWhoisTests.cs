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
            var parser = GetMockedITLDParser();

            IResolver resolver = new Resolver(null, null, transport.Object, new TldHandler(parser.Object));
            var result = resolver.QueryWhois("google.com");

            Assert.IsFalse(string.IsNullOrEmpty(result), "Should have returned a string.");
            Assert.AreEqual(SecondPassString, result, "Should be equal");

            result = resolver.QueryWhois("yahoo.com");

            Assert.IsFalse(string.IsNullOrEmpty(result), "Should have returned a string.");
            Assert.AreEqual(FirstPassString, result, "Should be equal");
        }

        private Mock<IWhoisTransport> GetMockedWhoisTransport()
        {
            var transport = new Mock<IWhoisTransport>();

            transport.Setup(tr => tr.RunWhoisQuery(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string domain, string srv) => GetTestWhoisData(domain, srv));

            return transport;
        }

        private Mock<ITLDParser> GetMockedITLDParser()
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

        private string GetTestWhoisData(string domain, string srv)
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

        private string GetTestWhoisServer(string firstResult)
        {
            var pairs = new Dictionary<string, string>
                            {
                                { FirstPassString, "whois.verisign-grs.com" },
                            };

            return pairs[firstResult];
        }
    }
}
