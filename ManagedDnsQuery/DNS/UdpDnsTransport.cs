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

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ManagedDnsQuery.DNS.MessageingImplementations;
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
