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
            {
                var generator = new WorldGenerator();
                var world = generator.Next();
            }
            {
                var names = new NameGenerator();
                var text = new TextTemplate("{subject} {raised} in {birthplace} near {place}.")
                                .Define("subject", x => x
                                    .UsingNamesFrom(names))
                                .Define("raised", x => x
                                    .As("grew up", "was raised", "was brought up"))
                                .Define("birthplace", x => x
                                    .As("[village]", "[town]")
                                    .UsingAdjectives("small", "modest", "poor", "large", "busy", "remote", "trade", "coastal", "underground")
                                    .UsingDeterminers("a"))
                                .Define("place", x => x
                                    .As("River", "Mountain", "Mountains", "Mountain Range", "Forest", "Ruins", "Canyon", "Sea", "Lake", "Plains")
                                    .UsingDeterminers("", "the")
                                    .UsingNamesFrom(names));
            }
            {
                var names = new NameGenerator();
                var chain = new TextChain()
                    .Initialize("world", x => x.UsingNamesFrom(names))
                    .Append("{subject} {raised} in {birthplace} near {place}.", x => x
                        .Define("subject", x => x.UsingNamesFrom(names))
                        .Define("raised", x => x.As("grew up", "was raised", "was brought up"))
                        .Define("birthplace", x => x
                            .As("[village]", "[town]")
                            .UsingAdjectives("small", "modest", "poor", "large", "busy", "remote", "trade", "coastal", "underground")
                            .UsingDeterminers("a"))
                        .Define("place", x => x
                            .As("River", "[Mountain]", "[Mountain]s", "[Mountain] Range", "Forest", "Ruins", "Canyon", "Sea", "Lake", "Plains")
                            .UsingDeterminers("", "the")
                            .UsingNamesFrom(names)))
                    .Append("Growing up, {pronoun} {waswere} always drawn to {passion}.", x => x
                        .Define("pronoun", x => x.As("he", "she"))
                        .Define("waswere", x => x.As("was"))
                        .Define("passion", x => x.As("the beauty and power of [fire]", "the vastness of the [ocean]"))
                        .RequiresContext("mountain"))
                    .Append("{pronoun} {waswere} banished from the community after accidentally setting the [house] of the local [priest] on fire.", x => x
                        .RequiresContext("fire"))
                    .Append("With the priest trapped inside, to everyone's horror.", x => x
                        .RequiresContext("fire", "priest"))
                    .Append("{pronoun} became a sailor at the age of {age}.", x => x
                        .Define("age", x => x.As("16","17","18","19","20"))
                        .RequiresContext("ocean"))
                    .Append("{subject} now travels {world}.");

                for (int i = 0; i < 20; i++)
                {
                    Console.WriteLine(chain.Next());
                }
            }

            Console.ReadLine();
        }
    }
}
