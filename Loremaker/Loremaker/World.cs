using System;
using System.Collections.Generic;

namespace Loremaker
{
    public class World
    {
        public string Name { get;  set; }

        public List<Continent> Continents { get; set; }

        public World()
        {
            this.Continents = new List<Continent>();
        }

    }
}
