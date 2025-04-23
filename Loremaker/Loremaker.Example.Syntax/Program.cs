using Archigen;
using Loremaker.Text;
using Syllabore;
using Syllabore.Fluent;

namespace Loremaker.Example.Syntax
{
    /// <summary>
    /// Loads the file "SyntaxTemplate.lore"
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var template = TextTemplate.LoadFromFile("SyntaxTemplate.lore");
            var names = new NameGenerator()
                .Any(x => x
                    .First("str")
                    .Middle("ae"));

            var personName = names.Next();

            template.Substitutions.Add("placeName", names);
            template.Substitutions.Add("person", new ConstantValue<string>(personName));

            var result = template.Next();

            Console.WriteLine(result);
        }
    }
}
