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

    }
}
