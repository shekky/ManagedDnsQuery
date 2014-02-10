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
using ManagedDnsQuery.DNS.MessageingConcretes;

namespace ManagedDnsQuery
{
    public sealed class QueryCache : IQueryCache
    {
        private IDictionary<string, IEnumerable<IResourceRecord>> Cache { get; set; }
        private readonly object _lock = new object();

        public IMessage CheckCache(IEnumerable<IQuestion> questions)
        {
            var result = new List<IResourceRecord>();

            lock (_lock)
            {
                if (Cache == null)
                    Cache = new Dictionary<string, IEnumerable<IResourceRecord>>();

                foreach (var question in questions)
                {
                    var key = string.Format("{0}{1}{2}", question.QName.ToLower(), question.QClass, question.QType);

                    if (Cache.ContainsKey(key))
                    {
                        if (Cache[key].Any(an => an.IsExpired()))
                            Cache.Remove(key);
                        else
                            result.AddRange(Cache[key]);
                    }
                }
            }

            return result.Any() ? new Message { Answers = result } : null;
        }

        public bool AddCache(IMessage message)
        {
            if (message == null)
                return false;

            var added = false;
            lock(_lock)
            {
                #region Add Answers
                if (message.Answers != null && message.Answers.Any())
                {
                    foreach (var ans in message.Answers)
                    {
                        var key = string.Format("{0}{1}{2}", ans.Name.ToLower(), ans.Class, ans.Type);

                        if (!Cache.ContainsKey(key))
                        {
                            Cache.Add(key, new List<IResourceRecord> { ans });
                            added = true;
                        }
                        else
                        {
                            var existing = Cache[key].Where(an => !an.IsExpired()).ToList();
                            if (!existing.Any(an => Exists(an, ans)))
                            {
                                existing.Add(ans);
                                Cache[key] = existing;
                                added = true;
                            }
                        }
                    }
                }
                #endregion

                #region Add Additionals
                if (message.Additionals != null && message.Additionals.Any())
                {
                    foreach (var add in message.Additionals)
                    {
                        var key = string.Format("{0}{1}{2}", add.Name.ToLower(), add.Class, add.Type);

                        if (!Cache.ContainsKey(key))
                        {
                            Cache.Add(key, new List<IResourceRecord> { add });
                            added = true;
                        }
                        else
                        {
                            var existing = Cache[key].Where(an => !an.IsExpired()).ToList();
                            if (!existing.Any(an => Exists(an, add)))
                            {
                                existing.Add(add);
                                Cache[key] = existing;
                                added = true;
                            }
                        }
                    }
                }
                #endregion

                #region Add Authorities
                if (message.Authorities != null && message.Authorities.Any())
                {
                    foreach (var auth in message.Authorities)
                    {
                        var key = string.Format("{0}{1}{2}", auth.Name.ToLower(), auth.Class, auth.Type);

                        if (!Cache.ContainsKey(key))
                        {
                            Cache.Add(key, new List<IResourceRecord> { auth });
                            added = true;
                        }
                        else
                        {
                            var existing = Cache[key].Where(an => !an.IsExpired()).ToList();
                            if (!existing.Any(an => Exists(an, auth)))
                            {
                                existing.Add(auth);
                                Cache[key] = existing;
                                added = true;
                            }
                        }
                    }
                }
                #endregion
            }

            return added;
        }

        /// <summary>
        /// Will Compare two IResourceRecord
        /// </summary>
        /// <param name="first">Existing Entry in Cache</param>
        /// <param name="attempted">Entry to be inserted into Cache</param>
        /// <returns></returns>
        private bool Exists(IResourceRecord first, IResourceRecord attempted)
        {
            if (first.Name.TryTrim() != attempted.Name.TryTrim())
                return false;
            if (first.Type != attempted.Type)
                return false;
            if (first.Class != attempted.Class)
                return false;
            if (first.Ttl != attempted.Ttl)
                return false;
            if (first.RdLength != attempted.RdLength)
                return false;
            if (!AreEqual(first.Rdata.ToArray(), attempted.Rdata.ToArray()))
                return false;

            return true;
        }

        /// <summary>
        /// Will compare Rdata of two IResourceRecords
        /// </summary>
        /// <param name="first">Existing Entry in Cache</param>
        /// <param name="attempted">Entry to be inserted into Cache</param>
        /// <returns></returns>
        private bool AreEqual(IList<byte> first, IList<byte> attempted)
        {
            if (first.Count != attempted.Count)
                return false;

            for (var ndx = 0; ndx < first.Count; ++ndx)
                if (first[ndx] != attempted[ndx])
                    return false;

            return true;
        }
    }
}
