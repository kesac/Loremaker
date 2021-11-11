using Loremaker.Maps;
using System;
using System.Collections.Generic;

namespace Loremaker
{
    public class World
    {
        public string Name { get;  set; }

        public string Description { get; set; }

        public Map Map { get; set; }

        public List<Landmass> Continents { get; set; }

        public World()
        {
            this.Continents = new List<Landmass>();
        }

    }
}
