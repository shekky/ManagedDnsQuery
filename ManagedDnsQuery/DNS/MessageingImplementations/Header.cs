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

using System.Collections.Generic;
using System.Linq;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery.DNS.MessageingImplementations
{
    internal sealed class Header : IHeader
    {
        public ushort Id { get; set; }
        public Qr Qr { get; set; }
        public OpCode OpCode { get; set; }
        public bool Aa { get; set; }
        public bool Tc { get; set; }
        public bool Rd { get; set; }
        public bool Ra { get; set; }
        public ushort Z { get; set; }
        public ResponseCode RCode { get; set; }
        public ushort QdCount { get; set; }
        public ushort AnCount { get; set; }
        public ushort NsCount { get; set; }
        public ushort ArCount { get; set; }

        public Header(IEnumerable<byte> rawHeader)
        {
            if(rawHeader == null || rawHeader.Count() < 6) 
                return;

            Id = rawHeader.Skip((int) HeaderBytePosition.Id)
                          .Take(2)
                          .ToLeUShort();

            var bits = rawHeader.Skip((int) HeaderBytePosition.Flags)
                                 .Take(2)
                                 .ToLeUShort();

            RCode = (ResponseCode)((bits >> (int)HeaderBitPosition.RCode) & 15);
            Z = (ushort)((bits >> (int)HeaderBitPosition.Z) & 7);
            Ra = ((bits >> (int) HeaderBitPosition.Ra) & 1) == 1;
            Rd = ((bits >> (int) HeaderBitPosition.Rd) & 1) == 1;
            Tc = ((bits >> (int) HeaderBitPosition.Tc) & 1) == 1;
            Aa = ((bits >> (int) HeaderBitPosition.Aa) & 1) == 1;
            OpCode = (OpCode)((bits >> (int) HeaderBitPosition.OpCode) & 15);
            Qr = (Qr)((bits >> (int) HeaderBitPosition.Qr) & 1);

            QdCount = rawHeader.Skip((int) HeaderBytePosition.QdCount)
                               .Take(2)
                               .ToLeUShort();

            AnCount = rawHeader.Skip((int) HeaderBytePosition.AnCount)
                                .Take(2)
                                .ToLeUShort();

            ArCount = rawHeader.Skip((int) HeaderBytePosition.ArCount)
                                .Take(2)
                                .ToLeUShort();
        }

        public IEnumerable<byte> ToBytes()
        {
            ushort bits = 0;
            bits |= (ushort) (((ushort) RCode & 15) << (int) HeaderBitPosition.RCode);
            bits |= (ushort) ((Z & 7) << (int) HeaderBitPosition.Z);
            bits |= (ushort) (((Ra ? 1 : 0) & 1) << (int) HeaderBitPosition.Ra);
            bits |= (ushort) (((Rd ? 1 : 0) & 1) << (int) HeaderBitPosition.Rd);
            bits |= (ushort) (((Tc ? 1 : 0) & 1) << (int) HeaderBitPosition.Tc);
            bits |= (ushort) (((Aa ? 1 : 0) & 1) << (int) HeaderBitPosition.Aa);
            bits |= (ushort) (((ushort) OpCode & 15) << (int) HeaderBitPosition.OpCode);
            bits |= (ushort) (((ushort) Qr & 1) << (int) HeaderBitPosition.Qr);

            var bytes = new List<byte>();
            bytes.AddRange(Id.ToBeBytes());
            bytes.AddRange(bits.ToBeBytes());
            bytes.AddRange(QdCount.ToBeBytes());
            bytes.AddRange(AnCount.ToBeBytes());
            bytes.AddRange(NsCount.ToBeBytes());
            bytes.AddRange(ArCount.ToBeBytes());

            return bytes;
        }
    }
}
//
/*  4.1.1. Header section format
 * 
	The header contains the following fields:

										1  1  1  1  1  1
		  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                      ID                       |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                    QDCOUNT                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                    ANCOUNT                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                    NSCOUNT                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                    ARCOUNT                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

		where:

		ID              A 16 bit identifier assigned by the program that
						generates any kind of query.  This identifier is copied
						the corresponding reply and can be used by the requester
						to match up replies to outstanding queries.

		QR              A one bit field that specifies whether this message is a
						query (0), or a response (1).

		OPCODE          A four bit field that specifies kind of query in this
						message.  This value is set by the originator of a query
						and copied into the response.  The values are:

						0               a standard query (QUERY)

						1               an inverse query (IQUERY)

						2               a server status request (STATUS)

						3-15            reserved for future use

		AA              Authoritative Answer - this bit is valid in responses,
						and specifies that the responding name server is an
						authority for the domain name in question section.

						Note that the contents of the answer section may have
						multiple owner names because of aliases.  The AA bit
						corresponds to the name which matches the query name, or
						the first owner name in the answer section.

		TC              TrunCation - specifies that this message was truncated
						due to length greater than that permitted on the
						transmission channel.

		RD              Recursion Desired - this bit may be set in a query and
						is copied into the response.  If RD is set, it directs
						the name server to pursue the query recursively.
						Recursive query support is optional.

		RA              Recursion Available - this be is set or cleared in a
						response, and denotes whether recursive query support is
						available in the name server.

		Z               Reserved for future use.  Must be zero in all queries
						and responses.

		RCODE           Response code - this 4 bit field is set as part of
						responses.  The values have the following
						interpretation:

						0               No error condition

						1               Format error - The name server was
										unable to interpret the query.

						2               Server failure - The name server was
										unable to process this query due to a
										problem with the name server.

						3               Name Error - Meaningful only for
										responses from an authoritative name
										server, this code signifies that the
										domain name referenced in the query does
										not exist.

						4               Not Implemented - The name server does
										not support the requested kind of query.

						5               Refused - The name server refuses to
										perform the specified operation for
										policy reasons.  For example, a name
										server may not wish to provide the
										information to the particular requester,
										or a name server may not wish to perform
										a particular operation (e.g., zone
										transfer) for particular data.

						6-15            Reserved for future use.

		QDCOUNT         an unsigned 16 bit integer specifying the number of
						entries in the question section.

		ANCOUNT         an unsigned 16 bit integer specifying the number of
						resource records in the answer section.

		NSCOUNT         an unsigned 16 bit integer specifying the number of name
						server resource records in the authority records
						section.

		ARCOUNT         an unsigned 16 bit integer specifying the number of
						resource records in the additional records section.  
 */
//