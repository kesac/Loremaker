using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public static class CollectionExtensions
    {
        public static void AddRange(this Dictionary<string, string> to, Dictionary<string, string> from)
        {
            foreach(var fromKey in from.Keys)
            {
                to[fromKey] = from[fromKey];
            }
        }

        public static void AddRange<T>(this HashSet<T> set, List<T> values)
        {
            foreach(var value in values)
            {
                set.Add(value);
            }
        }

    }
}
