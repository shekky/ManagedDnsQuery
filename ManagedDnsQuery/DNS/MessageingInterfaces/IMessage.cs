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

namespace ManagedDnsQuery.DNS.MessageingInterfaces
{
    public interface IMessage : IByteConverter
    {
        IHeader Header { get; set; }
        IEnumerable<IQuestion> Questions { get; set; }
        IEnumerable<IResourceRecord> Answers { get; set; }
        IEnumerable<IResourceRecord> Authorities { get; set; }
        IEnumerable<IResourceRecord> Additionals { get; set; }

        ExternalInterfaces.IMessage GetExternalAnswer();
    }
}
