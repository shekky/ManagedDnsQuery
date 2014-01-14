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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManagedDnsQuery.WHOIS.Interfaces;

namespace ManagedDnsQuery.WHOIS.Concretes
{
    internal class TldParser : ITLDParser
    {
        public IDictionary<string, string> Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("No TLD File Found at path: '{0}'", filePath));

            var results = new Dictionary<string, string>();
            using (var file = new FileStream(filePath, FileMode.Open))
            using (var sr = new StreamReader(file))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if(line.Trim().Substring(0,1) == ";")
                        continue; //Skip Comments

                    var peices = line.Split('=');
                    if(peices == null || !peices.Any() || peices.Length != 2)
                        continue; //Bad Line

                    results.Add(peices.FirstOrDefault(), peices.Skip(1).FirstOrDefault());
                }
            }
            return results;
        }

        public string ParseWhoisServer(string firstResult)
        {
            var server = string.Empty;

            var lines = firstResult.Trim().Replace("\r", "").Split('\n');
            if (lines.Any() && string.Equals(lines.FirstOrDefault().Trim(), "whois server version 2.0", StringComparison.CurrentCultureIgnoreCase))
                foreach (var line in lines.Where(line => line.Trim().ToLower().Contains("whois server:")))
                {
                    server = line.ToLower().Trim().Replace("whois server:", "");
                    break;
                }

            return server;
        }
    }
}
