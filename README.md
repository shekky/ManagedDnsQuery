ManagedDnsQuery
===============

library to do native DNS and WHOIS queries in 100% managed .NET code.

This library was inspired by two older code project articles "C# .NET DNS query component" ("http://www.codeproject.com/Articles/12072/C-NET-DNS-query-component") that is no longer supported, and the "DNS.NET Resolver (C#)" ("http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C") that seemed to update the previous and included many experimental and obsolete record types.

The goal of this library was the first step to creating my own managed .NET SMTP server to send and recieve e-mail. Unlike the other two libraries I have focused on rfc1035 ("http://www.ietf.org/rfc/rfc1035.txt") and rfc1886 ("http://www.ietf.org/rfc/rfc1886.txt") Since I am only needing basic A (AAAA for IPv6), MX, NS, PTR, and TXT records. 
The library will also contain SPF Checking, and WHOIS quering as they are common tasks. 