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
using System.IO;
using System.Linq;
using System.Reflection;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery.WHOIS.Concretes
{
    internal class TldHandler : ITLDHandler
    {
        public ITLDParser Parser { get; private set; }
        public IDictionary<string, string> Map { get; private set; }

        public TldHandler()
        {
            Parser = new TldParser();
        }

        public TldHandler(ITLDParser parser)
        {
            Parser = parser;
        }

        public string GetTldServer(string domain)
        {
            if (Map == null || !Map.Any())
                Map = Parser.Parse(string.Format(@"{0}\TLD_List.txt", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));

            var peices = domain.Trim().Split('.');
            if (!peices.Any())
                return null;

            var value = string.Empty;
            Map.TryGetValue(peices.LastOrDefault(), out value);

            return value;
        }

        public string GetWhoisServer(string firstResult)
        {
            return Parser.ParseWhoisServer(firstResult);
        }
    }
}
