using Loremaker.Names;
using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Experiments.Maps
{
    public class CustomNameGenerator : ILocationNameGenerator
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
                        .UsingMutator(x => x
                            .WithMutation(x => x.AppendSyllable("gard"))
                            .WithMutation(x => x.AppendSyllable("grim"))
                            .WithMutation(x => x.AppendSyllable("dar")))
                        .LimitMutationChance(0.33)
                        .LimitSyllableCount(2, 2);

            continents = new NameGenerator()
                        .UsingProvider(x => x
                            .WithLeadingConsonants("vrznmstl").Weight(2)
                            .WithLeadingConsonants("bckghw")
                            .WithVowels("a").Weight(4)
                            .WithVowels("ei").Weight(2)
                            .WithVowels("ou"))
                        .LimitSyllableCount(2, 3);

            regions = new NameGenerator()
                        .UsingProvider(x => x.WithProbability(x => x.TrailingConsonantExists(0)))
                        .LimitSyllableCount(2, 3);

            settlements = new NameGenerator()
                        .UsingProvider(x => x.WithProbability(x => x.TrailingConsonantExists(0)))
                        .LimitSyllableCount(2, 4);

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
