using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    // Does not implement Identifiable because
    // Worlds can currently only have one map at the moment.
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float LandThreshold { get; set; }
        public Dictionary<uint, MapPoint> MapPoints { get; set; }
        public Dictionary<uint, MapCell> MapCells { get; set; }

        public Map()
        {
            this.MapPoints = new Dictionary<uint, MapPoint>();
            this.MapCells = new Dictionary<uint, MapCell>();
        }

    }
}
