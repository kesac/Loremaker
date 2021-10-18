using Loremaker.Names;
using Loremaker.Text;
using Syllabore;
using System;
using System.IO;
using System.Text.Json;

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
                // Basic substitution
                var text = new TextTemplate("{subject} {verb} to {place}.")
                    .Define("subject", x => x.As("Alice", "Brian", "Cam"))
                    .Define("verb", x => x.As("ran", "walked", "hopped"))
                    .Define("place", x => x.As("store", "park", "house").UsingDeterminers("a", "the"));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(text.Next());
                }
                Console.WriteLine();
            }
            {
                // More complicated
                var text = new TextTemplate("{subject} {verb} to {place}.")
                    .CapitalizeFirstWord()
                    .Define("subject", x => x
                        .As("Alice", "Bob", "Chris")
                        .UsingAdjectives("busy", "impatient", "energetic")
                        .UsingDeterminers("the",""))
                    .Define("verb", x => x
                        .As("ran", "walked", "hopped"))
                    .Define("place", x => x
                        .As("store", "park", "house")
                        .UsingAdjectives("green", "busy", "bustling")
                        .UsingDeterminers("a", "the"));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(text.Next());
                }
                Console.WriteLine();
            }
            {
                // More complicated using randomly generated names for {place}
                var text = new TextTemplate("{subject} {raised} in {birthplace} near {place}.")
                                .Define("subject", x => x
                                    .As("Alice", "Brian", "Cam"))
                                .Define("raised", x => x
                                    .As("grew up", "was raised", "was brought up"))
                                .Define("birthplace", x => x
                                    .As("[village]", "[town]")
                                    .UsingAdjectives("small", "modest", "poor", "large", "busy", "remote", "trade", "coastal", "underground")
                                    .UsingDeterminers("a"))
                                .Define("place", x => x
                                    .As("River", "Mountain", "Mountains", "Mountain Range", "Forest", "Ruins", "Canyon", "Sea", "Lake", "Plains")
                                    .UsingDeterminers("", "the")
                                    .UsingNamesFrom(new NameGenerator()
                                        .UsingProvider(x => x
                                            .WithVowels("aeo")
                                            .WithLeadingConsonants("tvr"))));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(text.Next());
                }
                Console.WriteLine();
            }
            {
                // Complex example multiple templates using context from each other

                var worldNames = new NameGenerator()
                   .LimitSyllableCount(3)
                    .UsingProvider(x => x
                        .WithVowels("aeo")
                        .WithLeadingConsonants("tvr"));

                var locationNames = new NameGenerator()
                    .LimitSyllableCount(3)
                    .UsingProvider(x => x
                        .WithVowels("aieou")
                        .WithLeadingConsonants("strvbc")
                        .WithTrailingConsonants("strvgfc"));

                var shipNames = new NameGenerator()
                    .LimitSyllableCount(2,3)
                    .UsingProvider(x => new DefaultSyllableProvider()
                        .WithProbability(x => x
                            .TrailingConsonantExists(0)
                            .VowelBecomesSequence(0)))
                    .UsingValidator(x => x
                        .DoNotAllowPattern("[wyz]")
                        .DoNotAllowPattern("[^aeiou]{3,}"));

                var chain = new TextChain()
                    .Append("{subject} {raised} in {birthplace} near {place}.", x => x
                        .Define("raised", x => x.As("grew up", "was raised", "was brought up"))
                        .Define("birthplace", x => x
                            .As("[village]", "[town]") // square brackets tells text generator to record these as context tags
                            .UsingAdjectives("small", "modest", "poor", "large", "busy", "remote", "trade", "coastal", "underground")
                            .UsingDeterminers("a"))
                        .Define("place", x => x
                            .As("[Mountain]", "[Mountain] Range", "[Ocean]")
                            .UsingDeterminers("", "the")
                            .UsingNamesFrom(locationNames)))
                    .Append("Growing up, {pronoun} {waswere} always drawn to {passion}.", x => x
                        .Define("waswere", x => x.As("was"))
                        .Define("passion", x => x.As("the beauty and power of [fire]", "the vastness of the [ocean]"))
                        .WhenContextHas("mountain")) // this line of text won't be used unless "mountain" was used in previous lines of text
                    .Append("{pronoun} {waswere} banished from the community after accidentally setting the [house] of the local [priest] on fire.", x => x
                        .CapitalizeFirstWord()
                        .WhenContextHas("fire"))
                    .Append("With the priest trapped inside, to everyone's horror.", x => x
                        .WhenContextHas("fire", "priest"))
                    .Append("{pronoun} became a [sailor] at the age of {age}.", x => x
                        .CapitalizeFirstWord()
                        .Define("age", x => x.As("16", "17", "18"))
                        .WhenContextHas("ocean")
                        .AvoidWhenContextHas("mountain"))
                    .Append("At the age of {olderAge}, {pronoun} became captain of the {shipname}.", x => x
                        .Define("olderAge", x => x.As("29", "30", "31"))
                        .Define("shipname", x => x.UsingNamesFrom(shipNames))
                        .WhenContextHas("sailor"))
                    .Append("{subject} now sails {place}.", x => x
                        .WhenContextHas("sailor"))
                    .Append("{subject} now travels {world}.", x => x
                        .AvoidWhenContextHas("sailor"))
                    .DefineGlobally("world", x => x.UsingNamesFrom(worldNames))
                    .DefineGlobally("subject", x => x.As("Alice", "Brian", "Cam"))
                    .DefineGlobally("pronoun", x => x.As("he", "she"));

                var options = new JsonSerializerOptions() { WriteIndented = true };
                string result = JsonSerializer.Serialize<TextChain>(chain, options);
                File.WriteAllText("test.json.txt", result);

                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine(chain.Next());
                }
            }

            Console.ReadLine();
        }
    }
}
