using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double LandThreshold { get; set; }
        public Dictionary<int, MapCell> Cells { get; set; }
        public double[,] HeightMap { get; set; }

        public Map()
        {
            this.Cells = new Dictionary<int, MapCell>();
        }

    }
}
