using System;

namespace ManagedDnsQuery
{
    /// <summary>
    /// Extensions that wont blow up on null or empty strings.
    /// </summary>
    internal static class StringExtensions
    {
        public static string TryTrim(this string value)
        {
            return (string.IsNullOrEmpty(value) ? value : value.Trim());
        }

        public static string TrySubstring(this string value, int start, int chars)
        {
            return ((string.IsNullOrEmpty(value) || value.Length < (start + chars)) ? value : value.Substring(start, chars));
        }

        public static string TrySubstring(this string value, int start)
        {
            return ((string.IsNullOrEmpty(value) || value.Length < (start)) ? value : value.Substring(start));
        }

        public static string TryToLower(this string value)
        {
            return (string.IsNullOrEmpty(value) ? value : value.ToLower());
        }

        public static string TryToUpper(this string value)
        {
            return (string.IsNullOrEmpty(value) ? value : value.ToUpper());
        }

        public static bool AreEqual(this string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
