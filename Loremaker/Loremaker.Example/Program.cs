using Loremaker.Names;
using System;

namespace Loremaker.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var generator = new WorldGenerator(new DefaultNameGenerator());

            var world = generator.Next();
        }
    }
}
