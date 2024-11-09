using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public static class TextExtensions
    {
        public static bool CaseInsensitiveEquals(this string s, string target)
        {
            return String.Equals(s, target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Splits a line of text into individual words, but treats
        /// everything between quotes as a single word.
        /// </summary>
        public static List<string> Tokenize(this string s)
        {
            var result = new List<string>();
            var matches = Regex.Matches(s, "\"[^\"]*\"|[^ ]+");

            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var value = match.ToString();

                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        result.Add(value.Substring(1, value.Length - 2));
                    }
                    else
                    {
                        result.Add(value);
                    }

                }
            }
            else
            {
                result.Add(s);
            }

            return result;
        }
    }
}
