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

namespace ManagedDnsQuery.DNS.MessageingInterfaces
{
    internal enum HeaderBitPosition
    {
        RCode = 0,
        Z = 4,
        Ra = 7,
        Rd = 8,
        Tc = 9,
        Aa = 10,
        OpCode = 11,
        Qr = 15,
    }

    internal enum HeaderBytePosition
        {
            Id = 0,
            Flags = 2,
            QdCount = 4,
            AnCount = 6,
            NsCount = 8,
            ArCount = 10,
        }

    public  interface IHeader : IByteConverter
    {
        ushort Id { get; set; }
        bool Aa { get; set; }
        bool Tc { get; set; }
        bool Rd { get; set; }
        bool Ra { get; set; }
        ushort Z { get; set; }
        ushort QdCount { get; set; }
        ushort AnCount { get; set; }
        ushort NsCount { get; set; }
        ushort ArCount { get; set; }
        Qr Qr { get; set; }
        OpCode OpCode { get; set; }
        ResponseCode RCode { get; set; }
    }
}
