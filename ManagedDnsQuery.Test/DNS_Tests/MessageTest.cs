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
using System.Net;
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.RDataConcretes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    [TestClass]
    public class MessageTest : EqualityComparer
    {
        [TestMethod]
        public void TestQuestionMessageParse()
        {
            var request = new byte[] { 41, 163, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 5, 121, 97, 104, 111, 111, 3, 99, 111, 109, 0, 0, 15, 0, 1 };

            var expected = new Message
                               {
                                   Header = new Header(null)
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
                                                },
                                    Questions = new List<IQuestion>
                                                    {
                                                        new Question(null)
                                                                {
                                                                    QName = "yahoo.com.",
                                                                    QType = RecordType.MxRecord,
                                                                    QClass = RecordClass.In,
                                                                },
                                                    },
                                    Additionals = new List<IResourceRecord>(),
                                    Authorities = new List<IResourceRecord>(),
                                    Answers = new List<IResourceRecord>(),
                               };

            IMessage actual = new Message(request);
            AssertEquality(expected, actual);
        }

        [TestMethod]
        public void TestResponseMessageParse()
        {
            var response = new byte[]
                               {
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

            var expected = new Message
                               {
                                   Header = new Header(null)
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
                                                   },
                                   Questions = new List<IQuestion>
                                                   {
                                                       new Question(null)
                                                            {
                                                                QName = "yahoo.com.",
                                                                QType = RecordType.MxRecord,
                                                                QClass = RecordClass.In,
                                                            },
                                                   },
                                   Answers = new List<IResourceRecord>
                                                 {
                                                     new ResourceRecord(null)
                                                         {
                                                             Class = RecordClass.In,
                                                             Name = "yahoo.com.",
                                                             RdLength = 25,
                                                             Rdata = new byte[] { 0, 1, 4, 109, 116, 97, 55, 3, 97, 109, 48, 8, 121, 97, 104, 111, 111, 100, 110, 115, 3, 110, 101, 116, 0, },
                                                             Ttl = 1800,
                                                             Type = RecordType.MxRecord,
                                                             Record = new MxRecord(null)
                                                                          {
                                                                              Exchanger = "mta7.am0.yahoodns.net.",
                                                                              Preference = 1,
                                                                          },
                                                         },
                                                    new ResourceRecord(null)
                                                         {
                                                             Class = RecordClass.In,
                                                             Name = "yahoo.com.",
                                                             RdLength = 9,
                                                             Rdata = new byte[] { 0, 1, 4, 109, 116, 97, 54, 192, 46 },
                                                             Ttl = 1800,
                                                             Type = RecordType.MxRecord,
                                                             Record = new MxRecord(null)
                                                                          {
                                                                              Exchanger = "mta6.am0.yahoodns.net.",
                                                                              Preference = 1,
                                                                          },
                                                         },
                                                    new ResourceRecord(null)
                                                         {
                                                             Class = RecordClass.In,
                                                             Name = "yahoo.com.",
                                                             RdLength = 9,
                                                             Rdata = new byte[] { 0, 1, 4, 109, 116, 97, 53, 192, 46 },
                                                             Ttl = 1800,
                                                             Type = RecordType.MxRecord,
                                                             Record = new MxRecord(null)
                                                                          {
                                                                              Exchanger = "mta5.am0.yahoodns.net.",
                                                                              Preference = 1,
                                                                          },
                                                         },
                                                 },
                                   Authorities = new List<IResourceRecord>
                                                     {
                                                         new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] { 3, 110, 115, 50, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns2.yahoo.com.",
                                                                              },
                                                             },
                                                        new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] {3, 110, 115, 51, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns3.yahoo.com.",
                                                                              },
                                                             },
                                                        new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] {3, 110, 115, 52, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns4.yahoo.com.",
                                                                              },
                                                             },
                                                        new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] {3, 110, 115, 49, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns1.yahoo.com.",
                                                                              },
                                                             },
                                                        new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] {3, 110, 115, 54, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns6.yahoo.com.",
                                                                              },
                                                             },
                                                        new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] {3, 110, 115, 53, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns5.yahoo.com.",
                                                                              },
                                                             },
                                                        new ResourceRecord(null)
                                                             {
                                                                 Class = RecordClass.In,
                                                                 Name = "yahoo.com.",
                                                                 RdLength = 6,
                                                                 Rdata = new byte[] {3, 110, 115, 56, 192, 12 },
                                                                 Ttl = 172800,
                                                                 Type = RecordType.NsRecord,
                                                                 Record = new NsRecord(null)
                                                                              {
                                                                                  NsDomainName = "ns8.yahoo.com.",
                                                                              },
                                                             },
                                                     },
                                   Additionals = new List<IResourceRecord>
                                                        {
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns1.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 68, 180, 131, 16 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("68.180.131.16"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns2.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] {68, 142, 255, 16 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("68.142.255.16"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns3.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 203, 84, 221, 53 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("203.84.221.53"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns4.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 98, 138, 11, 157 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("98.138.11.157"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns5.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 119, 160, 247, 124 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("119.168.247.124"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns6.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 202, 43, 223, 170 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("202.43.223.170"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns8.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 202, 165, 104, 22 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("202.165.104.22"),
                                                                        }
                                                                },
                                                        },
                               };

            IMessage actual = new Message(response);
            AssertEquality(expected, actual);
        }

        [TestMethod]
        public void TestExternalMxConvert()
        {
            var response = new byte[]
                               {
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

            IMessage message = new Message(response);

            var expected = new []
                {
                    new DNS.ExternalConcretes.MxRecord
                        {
                            Class = RecordClass.In,
                            Name = "yahoo.com.",
                            Ttl = 1800,
                            Type = RecordType.MxRecord,
                            Preference = 1,
                            Exchanger = "mta7.am0.yahoodns.net.",
                        },
                    new DNS.ExternalConcretes.MxRecord
                        {
                            Class = RecordClass.In,
                            Name = "yahoo.com.",
                            Ttl = 1800,
                            Type = RecordType.MxRecord,
                            Preference = 1,
                            Exchanger = "mta6.am0.yahoodns.net.",
                        },
                    new DNS.ExternalConcretes.MxRecord
                        {
                            Class = RecordClass.In,
                            Name = "yahoo.com.",
                            Ttl = 1800,
                            Type = RecordType.MxRecord,
                            Preference = 1,
                            Exchanger = "mta5.am0.yahoodns.net.",
                        },
                };

            var actual = message.GetExternalAnswer().Answers.ToMxCollection();
            AssertEquality(expected, actual);
        }

        [TestMethod]
        public void TestARecordResponseParse()
        {
            var rawResponse = new byte[]
                               {
                                    215, 61, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                    111, 111, 3, 99, 111, 109, 0, 0, 1, 0, 1, 192, 12, 0, 1,
                                    0, 1, 0, 0, 7, 8, 0, 4, 206, 190, 36, 45, 192, 12, 0,
                                    1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 139, 183, 24, 192, 12,
                                    0, 1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 138, 253, 109, 192,
                                    12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 49,
                                    192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                    110, 115, 53, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0,
                                    0, 6, 3, 110, 115, 54, 192, 12, 192, 12, 0, 2, 0, 1, 0,
                                    2, 163, 0, 0, 6, 3, 110, 115, 51, 192, 12, 192, 12, 0, 2,
                                    0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 50, 192, 12, 192,
                                    12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 52,
                                    192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                    110, 115, 56, 192, 12, 192, 87, 0, 1, 0, 1, 0, 18, 117, 0,
                                    0, 4, 68, 180, 131, 16, 192, 159, 0, 1, 0, 1, 0, 18, 117,
                                    0, 0, 4, 68, 142, 255, 16, 192, 141, 0, 1, 0, 1, 0, 18,
                                    117, 0, 0, 4, 203, 84, 221, 53, 192, 177, 0, 1, 0, 1, 0,
                                    18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 105, 0, 1, 0, 1,
                                    0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 123, 0, 1, 0,
                                    1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 195, 0, 1,
                                    0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                               };

            var expected = new Message
                               {
                                   Header = new Header(null)
                                                {
                                                    Aa = true,
                                                    AnCount = 3,
                                                    ArCount = 7,
                                                    Id = 55101,
                                                    NsCount = 7,
                                                    OpCode = OpCode.Query,
                                                    QdCount = 1,
                                                    Qr = Qr.Response,
                                                    Ra = false,
                                                    RCode = ResponseCode.NoError,
                                                    Rd = true,
                                                    Tc = false,
                                                    Z = 0,
                                                },
                                   Questions = new List<IQuestion>
                                                   {
                                                       new Question(null)
                                                            {
                                                                QName = "yahoo.com.",
                                                                QType = RecordType.ARecord,
                                                                QClass = RecordClass.In,
                                                            },
                                                   },
                                    Answers = new List<IResourceRecord>
                                                 {
                                                     new ResourceRecord(null)
                                                         {
                                                             Class = RecordClass.In,
                                                             Name = "yahoo.com.",
                                                             RdLength = 4,
                                                             Rdata = new byte[] { 206, 190, 36, 45 },
                                                             Ttl = 1800,
                                                             Type = RecordType.ARecord,
                                                             Record = new ARecord(null)
                                                                          {
                                                                              Address = IPAddress.Parse("206.190.36.45"),
                                                                          },
                                                         },
                                                    new ResourceRecord(null)
                                                         {
                                                             Class = RecordClass.In,
                                                             Name = "yahoo.com.",
                                                             RdLength = 4,
                                                             Rdata = new byte[] { 98, 139, 183, 24 },
                                                             Ttl = 1800,
                                                             Type = RecordType.ARecord,
                                                             Record = new ARecord(null)
                                                                          {
                                                                              Address = IPAddress.Parse("98.139.183.24"),
                                                                          },
                                                         },
                                                    new ResourceRecord(null)
                                                         {
                                                             Class = RecordClass.In,
                                                             Name = "yahoo.com.",
                                                             RdLength = 4,
                                                             Rdata = new byte[] { 98, 138, 253, 109 },
                                                             Ttl = 1800,
                                                             Type = RecordType.ARecord,
                                                             Record = new ARecord(null)
                                                                          {
                                                                              Address = IPAddress.Parse("98.138.253.109"),
                                                                          },
                                                         },
                                                 },
                                   Additionals = new List<IResourceRecord>
                                                        {
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns1.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 68, 180, 131, 16 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("68.180.131.16"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns2.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] {68, 142, 255, 16 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("68.142.255.16"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns3.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 203, 84, 221, 53 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("203.84.221.53"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns4.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 98, 138, 11, 157 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("98.138.11.157"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns5.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 119, 160, 247, 124 },
                                                                    Ttl = 1209600,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("119.168.247.124"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns6.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 202, 43, 223, 170 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("202.43.223.170"),
                                                                        }
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "ns8.yahoo.com.",
                                                                    RdLength = 4,
                                                                    Rdata = new byte[] { 202, 165, 104, 22 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.ARecord,
                                                                    Record = new ARecord(null)
                                                                        {
                                                                            Address = IPAddress.Parse("202.165.104.22"),
                                                                        }
                                                                },
                                                        },
                                                     
                                   Authorities = new List<IResourceRecord>
                                                        {
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] {3, 110, 115, 49, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns1.yahoo.com.",
                                                                                },
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] {3, 110, 115, 53, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns5.yahoo.com.",
                                                                                },
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] {3, 110, 115, 54, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns6.yahoo.com.",
                                                                                },
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] {3, 110, 115, 51, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns3.yahoo.com.",
                                                                                },
                                                                },
                                                            new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] { 3, 110, 115, 50, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns2.yahoo.com.",
                                                                                },
                                                                },
                                                        new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] {3, 110, 115, 52, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns4.yahoo.com.",
                                                                                },
                                                                },
                                                        new ResourceRecord(null)
                                                                {
                                                                    Class = RecordClass.In,
                                                                    Name = "yahoo.com.",
                                                                    RdLength = 6,
                                                                    Rdata = new byte[] {3, 110, 115, 56, 192, 12 },
                                                                    Ttl = 172800,
                                                                    Type = RecordType.NsRecord,
                                                                    Record = new NsRecord(null)
                                                                                {
                                                                                    NsDomainName = "ns8.yahoo.com.",
                                                                                },
                                                                },
                                                        },
                               };

            IMessage actual = new Message(rawResponse);
            AssertEquality(expected, actual);
        }

        [TestMethod]
        public void TestExternalAConvert()
        {
            var rawResponse = new byte[]
                                  {
                                      215, 61, 133, 0, 0, 1, 0, 3, 0, 7, 0, 7, 5, 121, 97, 104,
                                      111, 111, 3, 99, 111, 109, 0, 0, 1, 0, 1, 192, 12, 0, 1,
                                      0, 1, 0, 0, 7, 8, 0, 4, 206, 190, 36, 45, 192, 12, 0,
                                      1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 139, 183, 24, 192, 12,
                                      0, 1, 0, 1, 0, 0, 7, 8, 0, 4, 98, 138, 253, 109, 192,
                                      12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 49,
                                      192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                      110, 115, 53, 192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0,
                                      0, 6, 3, 110, 115, 54, 192, 12, 192, 12, 0, 2, 0, 1, 0,
                                      2, 163, 0, 0, 6, 3, 110, 115, 51, 192, 12, 192, 12, 0, 2,
                                      0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 50, 192, 12, 192,
                                      12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3, 110, 115, 52,
                                      192, 12, 192, 12, 0, 2, 0, 1, 0, 2, 163, 0, 0, 6, 3,
                                      110, 115, 56, 192, 12, 192, 87, 0, 1, 0, 1, 0, 18, 117, 0,
                                      0, 4, 68, 180, 131, 16, 192, 159, 0, 1, 0, 1, 0, 18, 117,
                                      0, 0, 4, 68, 142, 255, 16, 192, 141, 0, 1, 0, 1, 0, 18,
                                      117, 0, 0, 4, 203, 84, 221, 53, 192, 177, 0, 1, 0, 1, 0,
                                      18, 117, 0, 0, 4, 98, 138, 11, 157, 192, 105, 0, 1, 0, 1,
                                      0, 18, 117, 0, 0, 4, 119, 160, 247, 124, 192, 123, 0, 1, 0,
                                      1, 0, 2, 163, 0, 0, 4, 202, 43, 223, 170, 192, 195, 0, 1,
                                      0, 1, 0, 2, 163, 0, 0, 4, 202, 165, 104, 22
                                  };

            var expected = new []
                            {
                                new DNS.ExternalConcretes.ARecord
                                    {
                                        Name = "yahoo.com.",
                                        Type = RecordType.ARecord,
                                        Class = RecordClass.In,
                                        Ttl = 1800,
                                        Address = IPAddress.Parse("206.190.36.45"),
                                    },
                                new DNS.ExternalConcretes.ARecord
                                    {
                                        Name = "yahoo.com.",
                                        Type = RecordType.ARecord,
                                        Class = RecordClass.In,
                                        Ttl = 1800,
                                        Address = IPAddress.Parse("98.139.183.24"),
                                    },
                                new DNS.ExternalConcretes.ARecord
                                    {
                                        Name = "yahoo.com.",
                                        Type = RecordType.ARecord,
                                        Class = RecordClass.In,
                                        Ttl = 1800,
                                        Address = IPAddress.Parse("98.138.253.109"),
                                    },
                            };

            IMessage message = new Message(rawResponse);

            AssertEquality(expected, message.GetExternalAnswer().Answers.ToACollection());
        }
    }
}
