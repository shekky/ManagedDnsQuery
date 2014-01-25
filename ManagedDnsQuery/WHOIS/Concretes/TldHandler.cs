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
using System.IO;
using System.Linq;
using System.Reflection;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery.WHOIS.Concretes
{
    internal class TldHandler : ITLDHandler
    {
        public ITLDParser Parser { get; private set; }
        public IDictionary<string, string> Map { get; private set; }

        public TldHandler()
        {
            Parser = new TldParser();
        }

        public TldHandler(ITLDParser parser)
        {
            Parser = parser;
        }

        public string GetTldServer(string domain)
        {
            if (Map == null || !Map.Any())
                Map = Parser.Parse(string.Format(@"{0}\TLD_List.txt", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));

            var peices = domain.ToLower().Trim().Split('.');
            if (!peices.Any())
                return null;

            var value = string.Empty;
            Map.TryGetValue(peices.LastOrDefault(), out value);

            return value;
        }

        public string GetWhoisServer(string firstResult)
        {
            return Parser.ParseWhoisServer(firstResult);
        }
    }
}
