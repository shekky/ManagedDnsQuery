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
using System.Linq;
using ManagedDnsQuery.DNS.MessageingInterfaces;
using ManagedDnsQuery.DNS.RDataConcretes;
using ManagedDnsQuery.SPF.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedDnsQuery.Test
{
    public abstract class EqualityComparer
    {
        internal virtual void AssertEquality(IList<byte> expected, IList<byte> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for(var ndx = 0; ndx < actual.Count; ++ndx)
                Assert.AreEqual(expected[ndx], actual[ndx], "Should Be equal");
        }

        internal virtual void AssertEquality(IEnumerable<byte> expected, IEnumerable<byte> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IHeader expected, IHeader actual)
        {
            Assert.AreEqual(expected.Id, actual.Id, "Should be equal");
            Assert.AreEqual(expected.Aa, actual.Aa, "Should be equal");
            Assert.AreEqual(expected.Tc, actual.Tc, "Should be equal");
            Assert.AreEqual(expected.Rd, actual.Rd, "Should be equal");
            Assert.AreEqual(expected.Ra, actual.Ra, "Should be equal");
            Assert.AreEqual(expected.Z, actual.Z, "Should be equal");
            Assert.AreEqual(expected.QdCount, actual.QdCount, "Should be equal");
            Assert.AreEqual(expected.AnCount, actual.AnCount, "Should be equal");
            Assert.AreEqual(expected.NsCount, actual.NsCount, "Should be equal");
            Assert.AreEqual(expected.ArCount, actual.ArCount, "Should be equal");
            Assert.AreEqual(expected.Qr, actual.Qr, "Should be equal");
            Assert.AreEqual(expected.OpCode, actual.OpCode, "Should be equal");
            Assert.AreEqual(expected.RCode, actual.RCode, "Should be equal");
            AssertEquality(expected.ToBytes(), actual.ToBytes());
        }

        internal virtual void AssertEquality(IEnumerable<IQuestion> expected, IEnumerable<IQuestion> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<IQuestion> expected, IList<IQuestion> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(IQuestion expected, IQuestion actual)
        {
            Assert.AreEqual(expected.QName, actual.QName, "Should be equal");
            Assert.AreEqual(expected.QType, actual.QType, "Should be equal");
            Assert.AreEqual(expected.QClass, actual.QClass, "Should be equal");
            AssertEquality(expected.ToBytes(), actual.ToBytes());
        }

        internal virtual void AssertEquality(IMessage expected, IMessage actual)
        {
            AssertEquality(expected.Header, actual.Header);
            AssertEquality(expected.Questions, actual.Questions);
            AssertEquality(expected.Answers, actual.Answers);
            AssertEquality(expected.Authorities, actual.Authorities);
            AssertEquality(expected.Additionals, actual.Additionals);
            AssertEquality(expected.ToBytes(), actual.ToBytes());
        }

        internal virtual void AssertEquality(IEnumerable<IResourceRecord> expected, IEnumerable<IResourceRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<IResourceRecord> expected, IList<IResourceRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(IResourceRecord expected, IResourceRecord actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.RdLength, actual.RdLength, "Should be equal");
            Assert.AreEqual(expected.Record.ToString(), actual.Record.ToString(), "Should be equal");
            AssertEquality(expected.Rdata, actual.Rdata);
            AssertEquality(expected.ToBytes(), actual.ToBytes());
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.MxRecord> expected, IEnumerable<DNS.ExternalConcretes.MxRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.MxRecord> expected, IList<DNS.ExternalConcretes.MxRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.MxRecord expected, DNS.ExternalConcretes.MxRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.Preference, actual.Preference, "Should be equal");
            Assert.AreEqual(expected.Exchanger, actual.Exchanger, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<MxRecord> expected, IEnumerable<MxRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<MxRecord> expected, IList<MxRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(MxRecord expected, MxRecord actual)
        {
            Assert.AreEqual(expected.Preference, actual.Preference, "Should be equal");
            Assert.AreEqual(expected.Exchanger, actual.Exchanger, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<NsRecord> expected, IEnumerable<NsRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<NsRecord> expected, IList<NsRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(NsRecord expected, NsRecord actual)
        {
            Assert.AreEqual(expected.NsDomainName, actual.NsDomainName, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.NsRecord> expected, IEnumerable<DNS.ExternalConcretes.NsRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.NsRecord> expected, IList<DNS.ExternalConcretes.NsRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.NsRecord expected, DNS.ExternalConcretes.NsRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.NsDomainName, actual.NsDomainName, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<ARecord> expected, IEnumerable<ARecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<ARecord> expected, IList<ARecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(ARecord expected, ARecord actual)
        {
            Assert.AreEqual(expected.Address, actual.Address, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.ARecord> expected, IEnumerable<DNS.ExternalConcretes.ARecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.ARecord> expected, IList<DNS.ExternalConcretes.ARecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.ARecord expected, DNS.ExternalConcretes.ARecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.Address, actual.Address, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<AaaaRecord> expected, IEnumerable<AaaaRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<AaaaRecord> expected, IList<AaaaRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(AaaaRecord expected, AaaaRecord actual)
        {
            Assert.AreEqual(expected.Address, actual.Address, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.AaaaRecord> expected, IEnumerable<DNS.ExternalConcretes.AaaaRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.AaaaRecord> expected, IList<DNS.ExternalConcretes.AaaaRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.AaaaRecord expected, DNS.ExternalConcretes.AaaaRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.Address, actual.Address, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<CNameRecord> expected, IEnumerable<CNameRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<CNameRecord> expected, IList<CNameRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(CNameRecord expected, CNameRecord actual)
        {
            Assert.AreEqual(expected.CName, actual.CName, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.CNameRecord> expected, IEnumerable<DNS.ExternalConcretes.CNameRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.CNameRecord> expected, IList<DNS.ExternalConcretes.CNameRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.CNameRecord expected, DNS.ExternalConcretes.CNameRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.CName, actual.CName, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<PtrRecord> expected, IEnumerable<PtrRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<PtrRecord> expected, IList<PtrRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(PtrRecord expected, PtrRecord actual)
        {
            Assert.AreEqual(expected.DomainName, actual.DomainName, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.PtrRecord> expected, IEnumerable<DNS.ExternalConcretes.PtrRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.PtrRecord> expected, IList<DNS.ExternalConcretes.PtrRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.PtrRecord expected, DNS.ExternalConcretes.PtrRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");
            Assert.AreEqual(expected.DomainName, actual.DomainName, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<SoaRecord> expected, IEnumerable<SoaRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<SoaRecord> expected, IList<SoaRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(SoaRecord expected, SoaRecord actual)
        {
            Assert.AreEqual(expected.MName, actual.MName, "Should be equal");
            Assert.AreEqual(expected.RName, actual.RName, "Should be equal");
            Assert.AreEqual(expected.Serial, actual.Serial, "Should be equal");
            Assert.AreEqual(expected.Refresh, actual.Refresh, "Should be equal");
            Assert.AreEqual(expected.Retry, actual.Retry, "Should be equal");
            Assert.AreEqual(expected.Expire, actual.Expire, "Should be equal");
            Assert.AreEqual(expected.Minimum, actual.Minimum, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.SoaRecord> expected, IEnumerable<DNS.ExternalConcretes.SoaRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.SoaRecord> expected, IList<DNS.ExternalConcretes.SoaRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.SoaRecord expected, DNS.ExternalConcretes.SoaRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.MName, actual.MName, "Should be equal");
            Assert.AreEqual(expected.RName, actual.RName, "Should be equal");
            Assert.AreEqual(expected.Serial, actual.Serial, "Should be equal");
            Assert.AreEqual(expected.Refresh, actual.Refresh, "Should be equal");
            Assert.AreEqual(expected.Retry, actual.Retry, "Should be equal");
            Assert.AreEqual(expected.Expire, actual.Expire, "Should be equal");
            Assert.AreEqual(expected.Minimum, actual.Minimum, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<TxtRecord> expected, IEnumerable<TxtRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<TxtRecord> expected, IList<TxtRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(TxtRecord expected, TxtRecord actual)
        {
            Assert.AreEqual(expected.Text, actual.Text, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<DNS.ExternalConcretes.TxtRecord> expected, IEnumerable<DNS.ExternalConcretes.TxtRecord> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<DNS.ExternalConcretes.TxtRecord> expected, IList<DNS.ExternalConcretes.TxtRecord> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(DNS.ExternalConcretes.TxtRecord expected, DNS.ExternalConcretes.TxtRecord actual)
        {
            Assert.AreEqual(expected.Class, actual.Class, "Should be equal");
            Assert.AreEqual(expected.Name, actual.Name, "Should be equal");
            Assert.AreEqual(expected.Ttl, actual.Ttl, "Should be equal");
            Assert.AreEqual(expected.Type, actual.Type, "Should be equal");
            Assert.AreEqual(expected.AsString, actual.AsString, "Should be equal");

            Assert.AreEqual(expected.Text, actual.Text, "Should be equal");
        }

        internal virtual void AssertEquality(IEnumerable<INetworkDetails> expected, IEnumerable<INetworkDetails> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<INetworkDetails> expected, IList<INetworkDetails> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality(INetworkDetails expected, INetworkDetails actual)
        {
            Assert.AreEqual(expected.BroadcastAddress, actual.BroadcastAddress, "Should be equal");
            Assert.AreEqual(expected.NetworkAddress, actual.NetworkAddress, "Should be equal");
            Assert.AreEqual(expected.SubNetMask, actual.SubNetMask, "Should be equal");
            Assert.AreEqual(expected.UsableEndAddress, actual.UsableEndAddress, "Should be equal");
            Assert.AreEqual(expected.UsableStartAddress, actual.UsableStartAddress, "Should be equal");
            Assert.AreEqual(expected.MaxHosts, actual.MaxHosts, "Should be equal");
            Assert.AreEqual(expected.MaxUsableHosts, actual.MaxUsableHosts, "Should be equal");
        }
    }
}
/*
        internal virtual void AssertEquality(IEnumerable<> expected, IEnumerable<> actual)
        {
            AssertEquality(expected.ToArray(), actual.ToArray());
        }

        internal virtual void AssertEquality(IList<> expected, IList<> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Should be equal");
            for (var ndx = 0; ndx < actual.Count; ++ndx)
                AssertEquality(expected[ndx], actual[ndx]);
        }

        internal virtual void AssertEquality( expected,  actual)
        {
            Assert.AreEqual(expected, actual., "Should be equal");
            Assert.AreEqual(expected, actual., "Should be equal");
        }
*/