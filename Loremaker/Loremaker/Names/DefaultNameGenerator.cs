using System;
using System.Collections.Generic;
using System.Text;
using Syllabore;

namespace Loremaker.Names
{
    /// <summary>
    /// A quick and dirty standalone name generator useful for prototyping.
    /// </summary>
    public class DefaultNameGenerator : INameGenerator
    {

        private NameGenerator GeneralNames { get; set; }
        private Random Random { get; set; }

        public DefaultNameGenerator()
        {

            this.Random = new Random();

            this.GeneralNames = new NameGenerator()
                .UsingProvider(p => p
                    .WithVowels("aeio")
                    .WithLeadingConsonants("strlpn")
                    .WithTrailingConsonantSequences("rt", "py"))
                .UsingMutator(m => m
                    .WithMutation(x => x.Syllables.Add("gard"))
                    .WithMutation(x => x.Syllables.Insert(0, "gran")))
                .UsingValidator(v => v
                    .InvalidateRegex(@"(\w)\1\1"))
                .LimitMutationChance(0.50)
                .LimitSyllableCount(2,3);
        }

        public string Next()
        {
            return this.GeneralNames.Next();
        }

        public string NextContinentName()
        {
            return this.GeneralNames.Next(3);
        }

        public string NextFamilyName()
        {
            return this.GeneralNames.Next(1 + this.Random.Next(3));
        }

        public string NextGovernmentName()
        {
            return this.GeneralNames.Next();
        }

        public string NextGroupName()
        {
            return this.GeneralNames.Next();
        }

        public string NextGuildName()
        {
            return this.GeneralNames.Next() + " Guild";
        }

        public string NextName()
        {
            return this.GeneralNames.Next();
        }

        public string NextRegionName()
        {
            return this.GeneralNames.Next();
        }

        public string NextReligiousOrderName()
        {
            return "Order of " + this.GeneralNames.Next();
        }

        public string NextSettlementName()
        {
            return this.GeneralNames.Next(2 + this.Random.Next(2));
        }

        public string NextWorldName()
        {
            return this.GeneralNames.Next();
        }
    }
}
