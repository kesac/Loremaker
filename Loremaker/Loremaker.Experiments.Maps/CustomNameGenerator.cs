using Archigen;
using Loremaker.Names;
using Syllabore;
using Syllabore.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Experiments.Maps
{
    public class CustomNameGenerator : ILocationNameGenerator, IGenerator<string>
    {

        private IGenerator<string> worlds;
        private IGenerator<string> continents;
        private DefaultNameGenerator DefaultNameGenerator;

        public CustomNameGenerator()
        {
            worlds = new NameGenerator()
                .Any(x => x
                    .First(x => x
                        .Add("vrstl").Weight(4)
                        .Add("wznm"))
                    .Middle(x => x
                        .Add("a").Weight(4)
                        .Add("ei").Weight(2)
                        .Add("ou")))
                .Transform(new TransformSet()
                    .Chance(0.33)
                    .RandomlySelect(1)
                    .Add(x => x.Append("gard"))
                    .Add(x => x.Append("grim"))
                    .Add(x => x.Append("dar")))
                .SetSize(2);

            continents = new NameGenerator()
                .Any(x => x
                    .First(x => x
                        .Add("vrznmstl").Weight(4)
                        .Add("bckghw"))
                    .Middle(x => x
                        .Add("a").Weight(4)
                        .Add("ei").Weight(2)
                        .Add("ou")))
                .SetSize(2, 3);

        }

        public string Next()
        {
            return this.DefaultNameGenerator.Next();
        }

        public string NextContinentName()
        {
            return continents.Next();
        }

        public string NextRegionName()
        {
            return this.DefaultNameGenerator.NextRegionName();
        }

        public string NextSettlementName()
        {
            return this.DefaultNameGenerator.NextSettlementName();
        }

        public string NextWorldName()
        {
            return worlds.Next();
        }
    }
}
