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

    }
}
