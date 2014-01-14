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
using System.Runtime.CompilerServices;
using ManagedDnsQuery.DNS.MessageingInterfaces;


#if DEBUG
[assembly: InternalsVisibleTo("ManagedDnsQuery.Test")]
#endif

namespace ManagedDnsQuery.DNS.MessageingConcretes
{
    internal class Message : IMessage
    {
        public IHeader Header { get; set; }
        public IEnumerable<IQuestion> Questions { get; set; }
        public IEnumerable<IResourceRecord> Answers { get; set; }
        public IEnumerable<IResourceRecord> Authorities { get; set; }
        public IEnumerable<IResourceRecord> Additionals { get; set; }

        internal Message()
        {
            Answers = new List<IResourceRecord>();
            Authorities = new List<IResourceRecord>();
            Additionals = new List<IResourceRecord>();
        }

        internal Message(IEnumerable<byte> rawMessage)
        {
            if(rawMessage == null || !rawMessage.Any())
                return;
            
            Header = new Header(rawMessage.Take(12));

            IByteReader reader = new ByteReader(rawMessage.ToArray());

            #region Questions
            var qe = new List<IQuestion>();
            for (var ndx = 0; ndx < Header.QdCount; ++ndx)
                qe.Add(new Question(reader));

            Questions = qe;
            #endregion

            #region Answers
            var temp = new List<IResourceRecord>();
            for(var ndx = 0; ndx < Header.AnCount; ++ndx)
                temp.Add(new ResourceRecord(reader));

            Answers = temp;
            #endregion

            #region Authorities
            temp = new List<IResourceRecord>();
            for (var ndx = 0; ndx < Header.NsCount; ++ndx)
                temp.Add(new ResourceRecord(reader));

            Authorities = temp;
            #endregion

            #region Additionals
            temp = new List<IResourceRecord>();
            for (var ndx = 0; ndx < Header.ArCount; ++ndx)
                temp.Add(new ResourceRecord(reader));

            Additionals = temp;
            #endregion
        }

        public IEnumerable<byte> ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Header.ToBytes());

            if(Questions != null && Questions.Any())
                bytes.AddRange(Questions.SelectMany(qu => qu.ToBytes()));
            if(Answers != null && Answers.Any()) 
                bytes.AddRange(Answers.SelectMany(ans => ans.ToBytes()));
            if(Authorities != null && Authorities.Any())
                bytes.AddRange(Authorities.SelectMany(auth => auth.ToBytes()));
            if(Additionals != null && Additionals.Any())
                bytes.AddRange(Additionals.SelectMany(ads => ads.ToBytes()));

            return bytes;
        }

        public ExternalInterfaces.IMessage GetExternalAnswer()
        {
            return new ExternalConcretes.Message
                       {
                           Header = Header.ToExternal(),
                           Questions = Questions.Select(qu => qu.ToExternal()),
                           Answers = Answers.Select(ans => ans.ConvertToExternalType()),
                           Authorities = Authorities.Select(auth => auth.ConvertToExternalType()),
                           Additionals = Additionals.Select(adds => adds.ConvertToExternalType()),
                       };
        }
    }
}
