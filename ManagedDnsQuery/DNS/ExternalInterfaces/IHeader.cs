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

namespace ManagedDnsQuery.DNS.ExternalInterfaces
{
    public interface IHeader
    {
        ushort Identifier { get; set; }
        bool AuthoritativeAnswer { get; set; }
        bool Truncated { get; set; }
        bool RecursionDesired { get; set; }
        bool RecrusionAvailable { get; set; }
        ushort Z { get; set; }
        ushort QuestionCount { get; set; }
        ushort AnswerCount { get; set; }
        ushort NameServerCount { get; set; }
        ushort AdditionalCount { get; set; }
        Qr QueryOrResponse { get; set; }
        OpCode OperationCode { get; set; }
        ResponseCode ResponseCode { get; set; }
    }
}
