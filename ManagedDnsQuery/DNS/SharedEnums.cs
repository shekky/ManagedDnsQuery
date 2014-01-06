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

namespace ManagedDnsQuery.DNS
{
    public enum OpCode
    {
        Query = 0,
        Iquery = 1,
        Status = 2,
        Reserved3 = 3,
        Notify = 4,
        Update = 5,
        Reserved6 = 6,
        Reserved7 = 7,
        Reserved8 = 8,
        Reserved9 = 9,
        Reserved10 = 10,
        Reserved11 = 11,
        Reserved12 = 12,
        Reserved13 = 13,
        Reserved14 = 14,
        Reserved15 = 15,
    }

    public enum Qr
    {
        Query = 0,
        Response = 1,
    }

    public enum ResponseCode
    {
        //[RFC1035] - Domain Names Implementation and Specification - See Notes.txt
        NoError = 0,
        FormErr = 1,
        ServFail = 2,
        NxDomain = 3,
        NotImp = 4,
        Refused = 5,

        //[RFC2136] - DNS UPDATE - See Notes.txt
        YxDomain = 6,
        YxrrSet = 7,
        NxrrSet = 8,
        NotAuth = 9,
        NotZone = 10,

        // Reserved
        Reserved11 = 11,
        Reserved12 = 12,
        Reserved13 = 13,
        Reserved14 = 14,
        Reserved15 = 15,

        //These are implimented as extended Rcodes in OPT record.
        //// [RFC2671] - EDNS0 - DNS Extensions - See Notes.txt
        //BADVERSSIG = 16,

        ////[RFC2845] - DNS TSIG - See Notes.txt
        //BADKEY = 17,
        //BADTIME = 18,

        ////[RFC2930] - DNS TKEY RR - See Notes.txt
        //BADMODE = 19,
        //BADNAME = 20,
        //BADALG = 21,

        ////[RFC4635] - TSIG Algorithms - See Notes.txt
        //BADTRUNC = 22,
    }

    public enum RecordType
    {
        ARecord = 1,
        NsRecord = 2,
        CNameRecord = 5,
        SoaRecord = 6,
        PtrRecord = 12,
        HinfoRecord = 13, //Not implementing
        MxRecord = 15,
        TxtRecord = 16,
        AaaaRecord = 28,
        A6Record = 38,   //Moved to Historic status per RFC 6563 - http://tools.ietf.org/html/rfc6563
        OptRecord = 41,
        SpfRecord = 99,
    }

    public enum RecordClass
    {
        In = 1,
        Ch = 3,
        Hs = 4,
        Any = 255,
    }
}
