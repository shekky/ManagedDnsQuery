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
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery
{
    public sealed class QueryCache : IQueryCache
    {
        private IDictionary<string, IMessage> Cache { get; set; }
        private readonly object Lock = new object();

        public IMessage CheckCache(IQuestion question)
        {
            IMessage result = null;

            lock (Lock)
            {
                if (Cache == null)
                    Cache = new Dictionary<string, IMessage>();

                var key = question
                            .ToBytes()
                            .ToByteString();

                if (Cache.ContainsKey(key))
                {
                    if (Cache[key].IsExpired())
                    {
                        Cache.Remove(key);
                        return result;
                    }
                    result = Cache[key];
                }
            }

            return result;
        }

        public bool AddCache(IMessage message)
        {
            if (message == null || message.Questions == null || !message.Questions.Any())
                return false;

            var added = false;
            lock(Lock)
            {
                if (!Cache.ContainsKey(message.Questions.FirstOrDefault().ToBytes().ToByteString()))
                {
                    Cache.Add(message.Questions.FirstOrDefault().ToBytes().ToByteString(), message);
                    added = true;
                }
            }

            return added;
        }
    }
}
