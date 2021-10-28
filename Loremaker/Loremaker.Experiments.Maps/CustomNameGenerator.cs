using Archigen;
using Loremaker.Names;
using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Experiments.Maps
{
    public class CustomNameGenerator : ILocationNameGenerator, IGenerator<string>
    {

        private NameGenerator worlds;
        private NameGenerator continents;
        private NameGenerator regions;
        private NameGenerator settlements;

        public CustomNameGenerator()
        {
            worlds = new NameGenerator()
                        .UsingProvider(x => x
                            .WithLeadingConsonants("vrstl").Weight(4)
                            .WithLeadingConsonants("wznm")
                            .WithVowels("a").Weight(4)
                            .WithVowels("ei").Weight(2)
                            .WithVowels("ou"))
                        .UsingTransformer(x => x
                            .Select(1).Chance(0.33)
                            .WithTransform(x => x.AppendSyllable("gard"))
                            .WithTransform(x => x.AppendSyllable("grim"))
                            .WithTransform(x => x.AppendSyllable("dar")))
                        .UsingSyllableCount(2);

            continents = new NameGenerator()
                        .UsingProvider(x => x
                            .WithLeadingConsonants("vrznmstl").Weight(2)
                            .WithLeadingConsonants("bckghw")
                            .WithVowels("a").Weight(4)
                            .WithVowels("ei").Weight(2)
                            .WithVowels("ou"))
                        .UsingSyllableCount(2, 3);

            regions = new NameGenerator()
                        .UsingProvider(x => x.WithProbability(x => x.TrailingConsonantExists(0)))
                        .UsingSyllableCount(2, 3);

            settlements = new NameGenerator()
                        .UsingProvider(x => x.WithProbability(x => x.TrailingConsonantExists(0)))
                        .UsingSyllableCount(2, 4);

        }

        public string Next()
        {
            return this.settlements.Next();
        }

        public string NextContinentName()
        {
            return continents.Next();
        }

        public string NextRegionName()
        {
            return regions.Next();
        }

        public string NextSettlementName()
        {
            return settlements.Next();
        }

        public string NextWorldName()
        {
            return worlds.Next();
        }
    }
}
