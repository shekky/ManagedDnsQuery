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
using System.IO;
using System.Net.Sockets;
using System.Text;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery.WHOIS.Concretes
{
    internal class WhoisTcpTransport : IWhoisTransport
    {
        public string RunWhoisQuery(string domain, string whoisServer)
        {
            var sb = new StringBuilder();
            try
            {
                var tcpClient = new TcpClient(whoisServer.TryTrim(), 43);
                var netStream = tcpClient.GetStream();

                using (var bufferedStream = new BufferedStream(netStream))
                {
                    var sw = new StreamWriter(bufferedStream);
                    sw.WriteLine(domain);
                    sw.Flush();

                    var sr = new StreamReader(bufferedStream);
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        sb.AppendLine(line);
                    }

                }
            }
            catch (Exception)
            {
                sb = new StringBuilder();
            }

            return sb.ToString();
        }
    }
}
