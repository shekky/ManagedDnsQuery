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
using System.Net;
using ManagedDnsQuery.SPF.Interfaces;

namespace ManagedDnsQuery.SPF.Concretes
{
    internal class SpfChecker : ISpfChecker
    {
        private INetworkParser Parser { get; set; }

        public SpfChecker()
        {
            Parser = new NetworkParser();
        }

        public SpfChecker(INetworkParser parser)
        {
            Parser = parser;
        }

        public SpfResult VerifyIpMechanism(IPAddress sender, string ipSpfText)
        {
            if (ipSpfText.Contains("/"))
            {
                var range = ipSpfText.Split(':').Skip(1).FirstOrDefault();
                INetworkDetails details = Parser.ParseRange(range);

                if (!details.IsInRange(sender))
                    return SpfResult.Fail;
            }
            else
            {
                if (!sender.Equals(IPAddress.Parse(ipSpfText.Split(':').Skip(1).FirstOrDefault())))
                    return SpfResult.Fail;
            }

            return SpfResult.Pass;
        }

        public SpfResult VerifyAMechanism(IPAddress sender, IEnumerable<IPAddress> aRecordAddresses, string range = null)
        {
            return SpfResult.Pass;
        }

        public SpfResult VerifyMxMechanism(IPAddress sender, IEnumerable<IPAddress> mxRecordAddresses)
        {
            throw new System.NotImplementedException();
        }

        public SpfResult VerifyPtrMechanism(IPAddress sender, IEnumerable<IPAddress> ptrRecordAddresses)
        {
            throw new System.NotImplementedException();
        }
    }
}
