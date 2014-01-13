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
