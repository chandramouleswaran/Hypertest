using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypertest.WebTest.Utils
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static string RemoveLineBreaks(this string theString)
        {
            return theString.Replace("\r", string.Empty).Replace("\n", " ").Replace("<strong>", string.Empty).Replace("</strong>", string.Empty);
        }
    }
}
