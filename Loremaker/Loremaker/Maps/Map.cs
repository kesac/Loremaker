using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double[,] HeightMap { get; set; }
        public VoronoiMap VoronoiMap { get; set; }

        public Map()
        {
            
        }
    }
}
