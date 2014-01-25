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

using System.Collections.Generic;
using ManagedDnsQuery.DNS.ExternalConcretes;

namespace ManagedDnsQuery.Test.DNS_Tests
{
    internal static class ConversionExtensions 
    {
        internal static IEnumerable<MxRecord> ToMxCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<MxRecord>.ToCollection(values);
        }

        internal static IEnumerable<ARecord> ToACollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<ARecord>.ToCollection(values);
        }

        internal static IEnumerable<CNameRecord> ToCNameCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<CNameRecord>.ToCollection(values);
        }

        internal static IEnumerable<NsRecord> ToNsCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<NsRecord>.ToCollection(values);
        }

        internal static IEnumerable<SoaRecord> ToSoaCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<SoaRecord>.ToCollection(values);
        }

        internal static IEnumerable<TxtRecord> ToTxtCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<TxtRecord>.ToCollection(values);
        }

        internal static IEnumerable<PtrRecord> ToPtrCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<PtrRecord>.ToCollection(values);
        }

        internal static IEnumerable<AaaaRecord> ToAaaaCollection(this IEnumerable<object> values)
        {
            return GenericCollectionConverter<AaaaRecord>.ToCollection(values);
        }
    }
}
