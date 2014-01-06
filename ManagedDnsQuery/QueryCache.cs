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
using ManagedDnsQuery.DNS;
using ManagedDnsQuery.DNS.MessageingInterfaces;

namespace ManagedDnsQuery
{
    public sealed class QueryCache : IQueryCache
    {
        private IDictionary<string, IMessage> Cache { get; set; }

        public IMessage CheckCache(IQuestion question)
        {
            IMessage result = null;

            if(Cache == null)
                Cache = new Dictionary<string, IMessage>();

            var key = question
                        .ToBytes()
                        .ToByteString();

            if(Cache.ContainsKey(key))
            {
                if (Cache[key].IsExpired())
                {
                    Cache.Remove(key);
                    return result;
                }
                result = Cache[key];
            }

            return result;
        }

        public bool AddCache(IMessage message)
        {
            if (message == null || message.Questions == null || !message.Questions.Any())
                return false;

            var added = false;

            if (!Cache.ContainsKey(message.Questions.FirstOrDefault().ToBytes().ToByteString()))
            {
                Cache.Add(message.Questions.FirstOrDefault().ToBytes().ToByteString(), message);
                added = true;
            }

            return added;
        }
    }
}
