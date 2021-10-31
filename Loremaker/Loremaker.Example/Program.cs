using Archigen;
using Loremaker.Maps;
using Loremaker.Names;
using Loremaker.Text;
using Markov;
using Syllabore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                            .Define("subject", "Alice", "Brian", "Cam")
                            .Define("verb", "ran", "walked", "hopped" )  
                            .Define("place", x => x
                                .As("store", "park", "house")
                                .ApplyDeterminersToAll("a", "the"));

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
                                .ApplyAdjectivesToAll("outgoing", "impatient", "energetic")
                                .ApplyDeterminersToAll("an",""))
                            .Define("verb", "ran", "walked", "hopped")
                            .Define("place", x => x
                                .As("lake")
                                .ApplyAdjectives("placid", "still", "crystal-clear") // This only applies to "lake"
                                .As("store", "park", "house")
                                .ApplyAdjectives("green", "busy", "bustling")       // This only applies to "store", "park", and "house"
                                .ApplyDeterminersToAll("a", "the"));                // This applies to everything

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
                                    .ApplyAdjectives("small", "modest", "poor", "large", "busy", "remote", "trade", "coastal", "underground")
                                    .ApplyDeterminers("a"))
                                .Define("place", x => x
                                    .As("River", "Mountain", "Mountains", "Mountain Range", "Forest", "Ruins", "Canyon", "Sea", "Lake", "Plains")
                                    .ApplyDeterminers("", "the")
                                    .ApplyNameGenerator(x => x
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
                   .UsingSyllableCount(3)
                    .UsingProvider(x => x
                        .WithVowels("aeo")
                        .WithLeadingConsonants("tvr"));

                var locationNames = new NameGenerator()
                    .UsingSyllableCount(3)
                    .UsingProvider(x => x
                        .WithVowels("aieou")
                        .WithLeadingConsonants("strvbc")
                        .WithTrailingConsonants("strvgfc"));

                var shipNames = new NameGenerator()
                    .UsingSyllableCount(2,3)
                    .UsingProvider(x => new DefaultSyllableProvider()
                        .WithProbability(x => x
                            .TrailingConsonantExists(0)
                            .VowelBecomesSequence(0)))
                    .UsingFilter(x => x
                        .DoNotAllowPattern("[wyz]")
                        .DoNotAllowPattern("[^aeiou]{3,}"));

                var chain = new TextChain()
                    .Append("{subject} {raised} in {birthplace} near {place}.", x => x
                        .Define("raised", "grew up", "was raised", "was brought up")
                        .Define("birthplace", x => x
                            .As("[village]") // square brackets tells text generator to record these as context tags
                            .ApplyAdjectives("small", "modest", "poor", "remote")
                            .As("[town]")
                            .ApplyAdjectives("large", "busy", "trade", "coastal", "underground")
                            .ApplyDeterminersToAll("a"))
                        .Define("place", x => x
                            .As("[Mountain]", "[Mountain] Range", "[Ocean]")
                            .ApplyDeterminers("", "the")
                            .ApplyNameGenerator(locationNames)))
                    .Append("Growing up, {pronoun} {waswere} always drawn to {passion}.", x => x
                        .Define("waswere", "was")
                        .Define("passion", "the beauty and power of [fire]", "the vastness of the [ocean]")
                        .WhenContextHas("mountain")) // this line of text won't be used unless "mountain" was used in previous lines of text
                    .Append("{pronoun} {waswere} banished from the community after accidentally setting the [house] of the local [priest] on fire.", x => x
                        .CapitalizeFirstWord()
                        .WhenContextHas("fire"))
                    .Append("With the priest trapped inside, to everyone's horror.", x => x
                        .WhenContextHas("fire", "priest"))
                    .Append("{pronoun} became a [sailor] at the age of {age}.", x => x
                        .CapitalizeFirstWord()
                        .Define("age", "16", "17", "18")
                        .WhenContextHas("ocean")
                        .AvoidWhenContextHas("mountain"))
                    .Append("At the age of {olderAge}, {pronoun} became captain of the {shipname}.", x => x
                        .Define("olderAge", "29", "30", "31")
                        .Define("shipname", shipNames)
                        .WhenContextHas("sailor"))
                    .Append("{subject} now sails {place}.", x => x
                        .WhenContextHas("sailor"))
                    .Append("{subject} now travels {world}.", x => x
                        .AvoidWhenContextHas("sailor"))
                    .DefineGlobally("world", worldNames)
                    .DefineGlobally("subject", "Alice", "Brian", "Cam")
                    .DefineGlobally("pronoun", "he", "she");


                var options = new JsonSerializerOptions() { WriteIndented = true };
                string result = JsonSerializer.Serialize<TextChain>(chain, options);
                File.WriteAllText("test.json.txt", result);

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(chain.Next());
                }
            }

            Console.WriteLine();

            {
                var g = new NameGenerator()
                        .UsingProvider(new SyllableSet(4, 16, 4)
                            .WithVowels("aeiou")
                            .WithLeadingConsonants("bcdfghijklmnprstvwyz"))
                        .UsingSyllableCount(3, 4);

                var t = new MarkovTextGenerator()
                        .FromCorpus("sample.txt")
                        .FromCorpus("sample2.txt")
                        // .UsingDepth(2)             // Default is 2
                        // .UsingDelimiter(' ')       // Default is a single space character ' '
                        .BeginTextWith("a")      // Try to start all generated text with this substring
                        // .EndTextWith("with")       // There are nuances to this
                        .LoadCorpus();                // If not called, corpuses will be implicitly loaded on first call to Next()

                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next().ToUpper());
                    Console.WriteLine(" " + t.Next());
                    Console.WriteLine();
                }

            }

            Console.WriteLine();

            {

                var worldNames = new NameGenerator()
                                .UsingProvider(x => x
                                    .WithVowels("aei")
                                    .WithLeadingConsonants("str"));
                var continentNames = new NameGenerator();

                var g = new Generator<World>()
                        .ForProperty<string>(x => x.Name, worldNames)
                        .ForProperty<string>(x => x.Description, new GibberishTextGenerator()
                            .UsingSentenceLength(2))
                        .ForListProperty<Continent>(x => x.Continents, new Generator<Continent>()
                            .ForProperty<string>(x => x.Name, continentNames)
                            .ForProperty<double[,]>(x => x.HeightMap, new HeightMapGenerator()))
                        .UsingSize(2);

                for(int i = 0; i < 3; i++)
                {
                    var world = g.Next();
                    Console.WriteLine("World of " + world.Name + " (" + world.Description + ")");
                    Console.WriteLine("Has continents:");

                    for(int j = 0; j < world.Continents.Count; j++)
                    {
                        Console.WriteLine(world.Continents[j].Name);
                    }
                }
                
            }

            Console.ReadLine();
        }
    }
}
