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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ManagedDnsQuery.DNS.MessageingConcretes;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS
{
    public sealed class UdpDnsTransport : IDnsTransport
    {
        public IMessage SendRequest(IMessage request, IPEndPoint dnsServer, int timeOut = 60)
        {
            byte[] rawResponse;

            try
            {
                using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeOut * 1000);
                    sock.SendTo(request.ToBytes().ToArray(), dnsServer);

                    var buffer = new byte[512];
                    var recieved = sock.Receive(buffer);

                    rawResponse = new byte[recieved];
                    Array.Copy(buffer, rawResponse, recieved);
                } 
            }
            catch(SocketException)
            {
                rawResponse = null;
                throw;
            }
            catch (Exception)
            {
                rawResponse = null;
                throw;
            }

            return new Message(rawResponse);
        }
    }
}
