using Loremaker.Text;
using Syllabore;

namespace Loremaker.Example.Syntax
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var template = TextTemplate.LoadFromFile("SyntaxTemplate.lore");

            var names = new NameGenerator();
            template.Substitutions.Add("placeName", names);
            template.Substitutions.Add("person", names);

            var result = template.Next();

            Console.WriteLine(result);

        }
    }
}
