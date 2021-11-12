using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public static class Chance
    {
        private static Random Random = new Random();

        public static bool Roll(double chance)
        {
            return Chance.Random.NextDouble() < chance;
        }

        public static int Between(int min, int max)
        {
            return Chance.Random.Next(max - min + 1) + min;
        }

        public static T FindRandom<T>(this List<T> list)
        {
            return list[Chance.Random.Next(list.Count)];
        }

        public static T RemoveRandom<T>(this List<T> list)
        {
            var result = list[Chance.Random.Next(list.Count)];

            if(!list.Remove(result))
            {
                throw new InvalidOperationException("Object does not exist in specified list");
            }

            return result;
        }

    }
}
