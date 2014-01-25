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
