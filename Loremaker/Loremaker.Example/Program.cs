using Archigen;
using Loremaker.Completions;
using Loremaker.Completions.OpenRouter;
using Loremaker.Data;
using Loremaker.Maps;
using Loremaker.Names;
using Loremaker.Text;
using Syllabore;
using Syllabore.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Loremaker.Example
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            // TryChainedWorldGeneration();
            // TryWorldGeneration();

            // TryTextTemplate();
            // TryGibberishGenerator();
            // TryTextomatic();
            
            // await GetModelsAsync();
            // await SummarizeHistoricalEventsAsync();
            // await CompleteItemsAsync();
        }

        private static void TryChainedWorldGeneration()
        {

            var worldNames = new NameGenerator()
                .Any(x => x
                    .First("str")
                    .Middle("aei"));

            var continentNames = new NameGenerator();

            var g = new Generator<World>()
                    .ForProperty<string>(x => x.Name, worldNames)
                    .ForProperty<string>(x => x.Description, new GibberishGenerator()
                        .UsingSentenceLength(2))
                    .ForProperty<Map>(x => x.Map, new MapGenerator())
                    .ForEach(x =>
                    {
                        // To do: populate continents
                    });

            for (int i = 0; i < 3; i++)
            {
                var world = g.Next();
                Console.WriteLine("World of " + world.Name + " (" + world.Description + ")");
                Console.WriteLine("Has continents:");

                // TODO: Continents not generating
                for (uint j = 0; j < world.Continents.Count; j++)
                {
                    Console.WriteLine(world.Continents[j].Name);
                }
            }
        }

        private static void TryWorldGeneration()
        {
            Console.WriteLine();
            var generator = new WorldGenerator();
            var world = generator.Next();
            Console.WriteLine("World: " + world.Name + "\nDescription: " + world.Description);
        }

        private static void TryTextTemplate()
        {
            var names = new NameGenerator("stlrmn", "aeiou");
            var template = new TextTemplate();

            template.Add("$person $raised in $birthplace near $place.");
            template.Add("Their family was poor, but they were happy.");
            template.Add("Growing up, they were always drawn to the vastness of the ocean.", "Mountain");
            template.Add("They became a sailor at the age of $age.", "Mountain");
            template.Add("An encounter with a $color $monster gave them a facial scar.");
            template.Add("Later, in their 40s, they returned to $place and became the mayor.");

            template.Substitute("person", names);
            template.Substitute("raised", "grew up", "was raised", "was brought up");

            var subjects = new string[] { "village", "town" };
            var adjectives = new List<string> { "modest", "poor", "busy", "remote", "trade", "coastal", "underground" };
            var determiners = new[] { "a" };

            var birthPlaces = new SubjectRandomizer(subjects);
            birthPlaces.SetAdjectives(adjectives);
            birthPlaces.SetDeterminers(determiners);

            template.Substitute("birthplace", birthPlaces);

            var placeNames = new string[] {
                "$placeName River",
                "the Mountains of $placeName",
                "the $placeName Mountain Range",
                "$placeName Forest",
                "the Ruins of $placeName",
                "the $placeName Sea",
                "$placeName Lake",
                "the $placeName Plains"
            };

            template.Substitute("place", placeNames);
            template.Substitute("placeName", names);


            var monsters = new string[] { "dragon", "ogre", "goblin", "troll", "giant" };
            template.Substitute("monster", monsters);
            template.Substitute("color", Things.GetDefaultColorGenerator());

            template.Substitute("age", "20", "21", "22");

            Console.WriteLine(template.Next());
            
        }

        private static void TryGibberishGenerator()
        {
            Console.WriteLine();

            var result = new StringBuilder();
            var gibberish = new GibberishGenerator();

            for (int i = 0; i < 10; i++)
            {
                result.Append(" " + gibberish.Next());
            }

            Console.WriteLine(result.ToString().Trim());
        }

        private static void TryTextomatic()
        {
            var textomatic = new Textomatic(Things.GetDefaultItemDescriptionTemplateGenerator());
            textomatic.Define("object", Things.GetDefaultObjectsGenerator());
            textomatic.Define("color", Things.GetDefaultColorGenerator());
            textomatic.Define("material", Things.GetDefaultMaterialsGenerator());
            textomatic.Define("concept", Things.GetDefaultConceptsGenerator());

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(textomatic.Next());
            }
        }



        private async static Task GetModelsAsync()
        {
            var openRouter = new OpenRouterClient();
            var models = await openRouter.GetModels("models.json");
            foreach (var model in models.Models)
            {
                Console.WriteLine($"{model.Id} - {model.Name}");
            }
            Console.ReadLine();
        }

        private async static Task CompleteItemsAsync()
        {
            var items = new List<Item>();
            for (int i = 0; i < 5; i++)
            {
                var item = new Item();
                var itemThemes = new List<string>();
                itemThemes.Add(Things.GetDefaultConceptsGenerator().Next());
                itemThemes.Add(Things.GetDefaultConceptsGenerator().Next());
                itemThemes.Add(Things.GetDefaultMaterialsGenerator().Next());

                if (new Random().NextDouble() > 0.5)
                {
                    itemThemes.Add(Things.GetDefaultColorGenerator().Next());
                }
                else
                {
                    itemThemes.Add(Things.GetDefaultConceptsGenerator().Next());
                }

                item["thematic-hints"] = string.Join(", ", itemThemes);
                items.Add(item);
            }

            var prompt = Prompts.Get("item-completer") + JsonSerializer.Serialize(items);

            Console.WriteLine(prompt);
            Console.WriteLine("---");

            var openRouter = new OpenRouterClient();
            var response = await openRouter.GetCompletion("openai/gpt-4.1-mini", prompt);

            var completedItems = JsonSerializer.Deserialize<List<Item>>(response.CompletionText);
            foreach (var item in completedItems)
            {
                Console.WriteLine($"[{item.Name}]");
                Console.WriteLine($"{item.Description}");
                Console.WriteLine();
            }
        }

        private async static Task SummarizeHistoricalEventsAsync()
        {
            var g = new HistoryGenerator(1001, 1100);
            var codex = g.Next();

            var useCompletion = true;
            if (useCompletion)
            {
                var openRouter = new OpenRouterClient();
                var people = codex.Entities.Values.Where(x => x.Type == "person").Select(x => x as Person).ToList();
                var biographerPrompt = Prompts.Get("biographer");

                foreach (var person in people)
                {
                    var prompt = $"{biographerPrompt}\n{JsonSerializer.Serialize(person)}";
                    var bioResponse = await openRouter.GetCompletion("openai/gpt-4.1-mini", prompt);
                    Console.WriteLine("-----");
                    Console.WriteLine(bioResponse.CompletionText);
                }

                var codexJson = JsonSerializer.Serialize(codex, new JsonSerializerOptions
                {
                    IncludeFields = true
                });

                var peoplesContext = JsonSerializer.Serialize(people);
                var historianPrompt = $"{peoplesContext}\n\n{Prompts.Get("historian")}\n\n{codexJson}";
                var historyResponse = await openRouter.GetCompletion("openai/gpt-4o-mini", historianPrompt);

                Console.WriteLine("-----");
                Console.WriteLine(historyResponse.CompletionText);
            }

        }

    }
}
