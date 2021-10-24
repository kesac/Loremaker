using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public static class CollectionExtensions
    {
        public static readonly Random Random = new Random();

        public static T GetRandom<T>(this List<T> list)
        {
            return list[Random.Next(list.Count)];
        }

        public static void AddRange(this Dictionary<string, string> to, Dictionary<string, string> from)
        {
            foreach(var fromKey in from.Keys)
            {
                to[fromKey] = from[fromKey];
            }
        }

    }
}
