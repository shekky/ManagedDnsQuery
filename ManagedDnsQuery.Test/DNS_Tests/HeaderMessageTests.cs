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

using System.Linq;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.DNS.MessageingConcretes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    [TestClass]
    public class HeaderMessageTests : EqualityComparer
    {
        [TestMethod]
        public void ParseHeaderToBytesTest()
        {
            var expected = new byte []
                            {
                                70, 17, 129, 128, 0, 1, 0, 1, 0, 0, 0, 0
                            };

            var actual = new Header(expected);
            AssertEquality(expected, actual.ToBytes());
        }

        [TestMethod]
        public void ParseHeaderFromMxRequest()
        {
            var request = new byte[] { 41, 163, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 5, 121, 97, 104, 111, 111, 3, 99, 111, 109, 0, 0, 15, 0, 1 };

            var expected = new Header(null)
                               {
                                   Aa = false,
                                   AnCount = 0,
                                   ArCount = 0,
                                   Id = 10659,
                                   NsCount = 0,
                                   OpCode = OpCode.Query,
                                   QdCount = 1,
                                   Qr = Qr.Query,
                                   Ra = false,
                                   RCode = ResponseCode.NoError,
                                   Rd = true,
                                   Tc = false,
                                   Z = 0,
                               };

            IHeader actual = new Header(request.Take(12));
            AssertEquality(expected, actual);
        }

        [TestMethod]
        public void ParseHeaderFromMxResponse()
        {
            var response = new byte[] { 
                                        41, 163, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                        111, 111, 3, 99, 111, 109, 0, 0, 15, 0, 1, 192, 12, 0, 15,
                                        0, 1, 0, 0, 7, 8, 0, 25, 0, 1, 4, 109, 116, 97, 55,
                                        3, 97, 109, 48, 8, 121, 97, 104, 111, 111, 100, 110, 115, 3, 110,
                                        101, 116, 0, 192, 12, 0, 15, 0, 1, 0, 0, 7, 8, 0, 9,
                                        0, 1, 4, 109, 116, 97, 54, 192, 46, 192, 12, 0, 15, 0, 1,
                                        0, 0, 7, 8, 0, 9, 0, 1, 4, 109, 116, 97, 53, 192, 46,
                                        192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                        50, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                        3, 110, 115, 51, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163,
                                        0, 0, 6, 3, 110, 115, 52, 192, 12, 192, 12, 0, 2, 0, 1,
                                        0, 2, 163, 0, 0, 6, 3, 110, 115, 49, 192, 12, 192, 12, 0,
                                        2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 54, 192, 12,
                                        192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115,
                                        53, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6,
                                        3, 110, 115, 56, 192, 12, 192, 172, 0, 1, 0, 1, 0, 18, 117,
                                        0, 0, 4, 68, 180, 131, 16, 192, 118, 0, 1, 0, 1, 0, 18,
                                        117, 0, 0, 4, 68, 142, 255, 16, 192, 136, 0, 1, 0, 1, 0,
                                        18, 117, 0, 0, 4, 203, 84, 221, 53, 192, 154, 0, 1, 0, 1,
                                        0, 18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 208, 0, 1, 0,
                                        1, 0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 190, 0, 1,
                                        0, 1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 226, 0,
                                        1, 0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                                      };

            var expected = new Header(null)
                               {
                                   Aa = true,
                                   AnCount = 3,
                                   ArCount = 7,
                                   Id = 10659,
                                   NsCount = 7,
                                   OpCode = OpCode.Query,
                                   QdCount = 1,
                                   Qr = Qr.Response,
                                   Ra = false,
                                   RCode = ResponseCode.NoError,
                                   Rd = true,
                                   Tc = false,
                                   Z = 0,
                               };

            IHeader actual = new Header(response.Take(12));
            AssertEquality(expected, actual);
        }
    }
}
