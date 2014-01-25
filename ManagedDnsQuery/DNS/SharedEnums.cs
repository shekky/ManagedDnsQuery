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
