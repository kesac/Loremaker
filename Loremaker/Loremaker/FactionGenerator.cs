using Archigen;
using Loremaker.Names;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class FactionGenerator : IGenerator<Faction>
    {
        private IGenerator<string> _nameGenerator;

        public FactionGenerator()
        {
            _nameGenerator = new DefaultNameGenerator();
        }

        public Faction Next()
        {
            var factionName = _nameGenerator.Next();
            var result = new Faction(factionName);
            result.DecoratedName = FactionGenerator.GenerateDecoratedName(factionName);
            result.Denonym = CultureGenerator.GenerateDenonym(factionName);


            return result;
        }

        public static string GenerateDecoratedName(string name)
        {
            var templates = new string[]
            {
               "{name} Clan",
               "the {name} Tribe",
               "the {name} House",
               "House {name}",
               "the {name} Guild",
               "the {name} Order",
               "Order of {name}",
               "the {name} Kingdom",
               "the Kingdom of {name}",
            };

            var selector = new RandomSelector<string>(templates);
            return selector.Next().Replace("{name}", name);

        }
    }
}
