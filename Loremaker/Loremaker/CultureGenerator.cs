using Archigen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker
{
    public class CultureGenerator : IGenerator<Culture>
    {
        private IGenerator<string> _nameGenerator;

        public CultureGenerator(IGenerator<string> cultureNameGenerator)
        {
            _nameGenerator = cultureNameGenerator;
        }

        public Culture Next()
        {
            var cultureName = _nameGenerator.Next();
            var cultureDenonym = GenerateDenonym(cultureName);
            var result = new Culture(cultureName, cultureDenonym);
            return result;
        }

        public static string GenerateDenonym(string name)
        {
            var result = name;

            if (Regex.IsMatch(name, @"[a]$", RegexOptions.IgnoreCase))
            {
                var selector = new RandomSelector<string>("an", "ian");
                result = Regex.Replace(result, @"[aeiou]$", selector.Next(), RegexOptions.IgnoreCase);
            }
            else if (Regex.IsMatch(name, @"[e]$", RegexOptions.IgnoreCase))
            {
                result = name + "n";
            }
            else if (Regex.IsMatch(name, @"[i]$", RegexOptions.IgnoreCase))
            {
                var selector = new RandomSelector<string>("n", "nian", "nite");
                result = name + selector.Next();
            }
            else if (Regex.IsMatch(name, @"[o]$", RegexOptions.IgnoreCase))
            {
                var selector = new RandomSelector<string>("n", "nian", "nite");
                result = name + selector.Next();
            }
            else if (Regex.IsMatch(name, @"[u]$", RegexOptions.IgnoreCase))
            {
                var selector = new RandomSelector<string>("nian", "nite");
                result = name + selector.Next();
            }
            else
            {
                var selector = new RandomSelector<string>("an", "ite");
                result += selector.Next();
            }

            return result;
        }
    }
}
