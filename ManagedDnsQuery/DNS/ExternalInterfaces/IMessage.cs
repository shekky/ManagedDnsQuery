﻿/**********************************************************************************
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

namespace ManagedDnsQuery.DNS.ExternalInterfaces
{
    public interface IMessage
    {
        IHeader Header { get; set; }
        IEnumerable<IQuestion> Questions { get; set; }
        IEnumerable<object> Answers { get; set; }
        IEnumerable<object> Authorities { get; set; }
        IEnumerable<object> Additionals { get; set; }
    }
}
