using System;
using System.Collections.Generic;
using System.Text;
using Archigen;
using Syllabore;
using Syllabore.Fluent;

namespace Loremaker.Names
{
    /// <summary>
    /// A quick and dirty standalone name generator useful for prototyping.
    /// </summary>
    public class DefaultNameGenerator : ILocationNameGenerator, ICharacterNameGenerator, IOrganizationNameGenerator, IGenerator<string>
    {
        private Random Random { get; set; }
        private NameGenerator GeneralNames { get; set; }

        public DefaultNameGenerator()
        {
            this.Random = new Random();
            this.GeneralNames = new NameGenerator()
                .Start(x => x
                    .First("strlmngbhcdp")
                    .Middle("aeiou")
                    .Last("mnlrst"))
                .Inner(x => x
                    .First("strlmn")
                    .Middle("aeio"))
                .End(x => x.CopyInner());
        }

        public string Next()
        {
            this.GeneralNames.SetSize(2, 3);
            return this.GeneralNames.Next();
        }

        public string NextContinentName()
        {
            this.GeneralNames.SetSize(3);
            return this.GeneralNames.Next();
        }

        public string NextFamilyName()
        {
            this.GeneralNames.SetSize(1 + this.Random.Next(3));
            return this.GeneralNames.Next();
        }

        public string NextGovernmentName()
        {
            this.GeneralNames.SetSize(2, 3);
            return this.GeneralNames.Next();
        }

        public string NextFactionName()
        {
            this.GeneralNames.SetSize(2, 3);
            return this.GeneralNames.Next();
        }

        public string NextGuildName()
        {
            this.GeneralNames.SetSize(2, 3);
            return this.GeneralNames.Next() + " Guild";
        }

        public string NextName()
        {
            this.GeneralNames.SetSize(2, 3);
            return this.GeneralNames.Next();
        }

        public string NextRegionName()
        {
            this.GeneralNames.SetSize(3, 4);
            return this.GeneralNames.Next();
        }

        public string NextReligiousOrderName()
        {
            this.GeneralNames.SetSize(3);
            return "Order of " + this.GeneralNames.Next();
        }

        public string NextSettlementName()
        {
            this.GeneralNames.SetSize(2 + this.Random.Next(2));
            return this.GeneralNames.Next();
        }

        public string NextWorldName()
        {
            this.GeneralNames.SetSize(2);
            return this.GeneralNames.Next();
        }
    }
}
