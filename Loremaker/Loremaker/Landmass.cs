using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class Landmass
    {
        public virtual World Parent { get; set; }
        public string Name { get; set; }
        public MapPoint Center { get; set; }
        public List<MapCell> MapCells { get; set; }

        public Landmass()
        {
            this.MapCells = new List<MapCell>();
        }
    }
}
