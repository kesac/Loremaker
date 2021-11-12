using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double LandThreshold { get; set; }
        public double[,] HeightMap { get; set; }
        public Dictionary<uint, MapCell> Cells { get; set; }
        public List<Landmass> Landmasses { get; set; }

        public Map()
        {
            this.Cells = new Dictionary<uint, MapCell>();
        }

    }
}
