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
using System.IO;
using System.Linq;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery.WHOIS.Concretes
{
    internal class TldParser : ITLDParser
    {
        public IDictionary<string, string> Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("No TLD File Found at path: '{0}'", filePath));

            var results = new Dictionary<string, string>();
            using (var file = new FileStream(filePath, FileMode.Open))
            using (var sr = new StreamReader(file))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if(line.TryTrim().TrySubstring(0,1) == ";")
                        continue; //Skip Comments

                    var peices = line.Split('=');
                    if(peices == null || !peices.Any() || peices.Length != 2)
                        continue; //Bad Line

                    results.Add(peices.FirstOrDefault(), peices.Skip(1).FirstOrDefault());
                }
            }
            return results;
        }

        public string ParseWhoisServer(string firstResult)
        {
            var server = string.Empty;

            var lines = firstResult.TryTrim().Replace("\r", "").Split('\n');
            if (lines.Any() && string.Equals(lines.FirstOrDefault().TryTrim(), "whois server version 2.0", StringComparison.CurrentCultureIgnoreCase))
                foreach (var line in lines.Where(line => line.TryTrim().TryToLower().Contains("whois server:")))
                {
                    server = line.TryToLower().TryTrim().Replace("whois server:", "");
                    break;
                }

            return server;
        }
    }
}
