using Loremaker.Names;
using Loremaker.Text;
using Syllabore;
using System;

namespace Loremaker.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var generator = new WorldGenerator();
            var world = generator.Next();

            var names = new NameGenerator();

            var text = new TextGenerator("{subject} {raised} in {birthplace} near {place}.")
                            .Define("subject", x => x
                                .UsingNamesFrom(names))
                            .Define("raised", x => x
                                .As("grew up", "was raised", "was brought up", "grew up alone"))
                            .Define("birthplace", x => x
                                .As("village", "town")
                                .UsingAdjectives("small", "modest", "poor", "large", "busy", "remote", "trade", "coastal", "underground")
                                .UsingDeterminers("a"))
                            .Define("place", x => x
                                .As("River", "Mountain", "Mountains", "Mountain Range", "Forest", "Ruins", "Canyon", "Sea", "Lake", "Plains")
                                .UsingDeterminers("", "the")
                                .UsingNamesFrom(names));

            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(text.Next());
            }

            Console.ReadLine();
        }
    }
}
