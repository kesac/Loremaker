using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public static class ListExtensions
    {
        public static readonly Random Random = new Random();

        public static string GetRandom(this List<string> list)
        {
            return list[Random.Next(list.Count)];
        }

    }
}
