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

        private NameGenerator NameGenerator { get; set; }
        private Random Random { get; set; }

        public DefaultNameGenerator()
        {
            var provider = new StandaloneSyllableProvider(); 
            var validator = new StandaloneNameValidator();
            this.NameGenerator = new NameGenerator(provider, validator);

            this.Random = new Random();

            this.NameGenerator.MinimumSyllables = 2;
            this.NameGenerator.MaximumSyllables = 2;
        }

        public string Next()
        {
            return this.NameGenerator.Next();
        }

        public string NextContinentName()
        {
            return this.NameGenerator.Next(3);
        }

        public string NextFamilyName()
        {
            return this.NameGenerator.Next(1 + this.Random.Next(3));
        }

        public string NextGovernmentName()
        {
            return this.NameGenerator.Next();
        }

        public string NextGroupName()
        {
            return this.NameGenerator.Next();
        }

        public string NextGuildName()
        {
            return this.NameGenerator.Next() + " Guild";
        }

        public string NextName()
        {
            return this.NameGenerator.Next();
        }

        public string NextRegionName()
        {
            return this.NameGenerator.Next();
        }

        public string NextReligiousOrderName()
        {
            return "Order of " + this.NameGenerator.Next();
        }

        public string NextSettlementName()
        {
            return this.NameGenerator.Next(2 + this.Random.Next(2));
        }

        public string NextWorldName()
        {
            return this.NameGenerator.Next();
        }
    }
}
