using Loremaker.Names;
using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Experiments.Maps
{
    public class CustomNameGenerator : ILocationNameGenerator
    {

        private NameGenerator _worldNameGenerator;
        private NameGenerator _continentNameGenerator;
        private NameGenerator _regionNameGenerator;
        private NameGenerator _SettlementNameGenerator;

        public CustomNameGenerator(string filename)
        {
            var loader = new XmlFileLoader(filename);
            loader.Load();

            _worldNameGenerator = loader.GetNameGenerator("World");
            _continentNameGenerator = loader.GetNameGenerator("Continent");
            _regionNameGenerator = loader.GetNameGenerator("Region");
            _SettlementNameGenerator = loader.GetNameGenerator("Settlement");
        }

        public string NextContinentName()
        {
            return _continentNameGenerator.Next();
        }

        public string NextRegionName()
        {
            return _regionNameGenerator.Next();
        }

        public string NextSettlementName()
        {
            return _SettlementNameGenerator.Next();
        }

        public string NextWorldName()
        {
            return _worldNameGenerator.Next();
        }
    }
}
