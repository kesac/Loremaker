using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class LandmassRegion
    {
        public virtual Landmass Parent { get; set; }
        public string Name { get; set; }
        public List<MapCell> MapCells { get; set; }


        public LandmassRegion()
        {
            this.MapCells = new List<MapCell>();
        }

    }
}
