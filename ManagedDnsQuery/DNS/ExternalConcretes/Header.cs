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

using ManagedDnsQuery.DNS.ExternalInterfaces;

namespace ManagedDnsQuery.DNS.ExternalConcretes
{
    public sealed class Header : IHeader
    {
        public ushort Identifier { get; set; }
        public bool AuthoritativeAnswer { get; set; }
        public bool Truncated { get; set; }
        public bool RecursionDesired { get; set; }
        public bool RecrusionAvailable { get; set; }
        public ushort Z { get; set; }
        public ushort QuestionCount { get; set; }
        public ushort AnswerCount { get; set; }
        public ushort NameServerCount { get; set; }
        public ushort AdditionalCount { get; set; }
        public Qr QueryOrResponse { get; set; }
        public OpCode OperationCode { get; set; }
        public ResponseCode ResponseCode { get; set; }
    }
}
