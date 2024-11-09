using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public static class TextExtensionsOld
    {
        public static bool HasContextClues(this string s)
        {
            return Regex.IsMatch(s, @"(\[[^\]]+\])");
        }

        public static string RemoveCurlyBrackets(this string s)
        {
            return s.Replace("{", string.Empty).Replace("}", string.Empty);
        }

        public static string RemoveSquareBrackets(this string s)
        {
            return s.Replace("[", string.Empty).Replace("]", string.Empty);
        }

        public static List<string> GetContextClues(this string s)
        {
            var result = new List<string>();
            if (s.HasContextClues())
            {
                foreach (var m in Regex.Matches(s, @"(\[[^\]]+\])"))
                {
                    var capture = m.ToString();
                    var value = capture.RemoveSquareBrackets();
                    result.Add(value.ToLower());
                }
            }
            return result;
        }
    }
}
