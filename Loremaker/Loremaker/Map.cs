using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float LandThreshold { get; set; }
        public List<MapPoint> MapPoints { get; set; }
        [JsonIgnore]
        public Dictionary<uint, MapPoint> MapPointsById { get; set; }
        public List<MapCell> MapCells { get; set; }
        [JsonIgnore]
        public Dictionary<uint, MapCell> MapCellsById { get; set; }
        public List<Landmass> Landmasses { get; set; }
        [JsonIgnore]
        public Dictionary<uint, Landmass> LandmassesById { get; set; }

        public Map()
        {
            this.MapPoints = new List<MapPoint>();
            this.MapPointsById = new Dictionary<uint, MapPoint>();
            this.MapCells = new List<MapCell>();
            this.MapCellsById = new Dictionary<uint, MapCell>();
            this.Landmasses = new List<Landmass>();
            this.LandmassesById = new Dictionary<uint, Landmass>();
        }

    }
}
