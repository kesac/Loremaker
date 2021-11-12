using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public enum MapAttribute
    {
        Water,
        Land,
        Coast
    }

    public class MapCell : Identifiable
    {
        public uint Id { get; set; }
        public MapPoint Center { get; set; }
        public List<uint> MapPointIds { get; set; }
        [JsonIgnore]
        public List<MapPoint> MapPoints { get; set; }

        public List<uint> AdjacentMapCellIds { get; set; }
        [JsonIgnore]
        public virtual List<MapCell> AdjacentMapCells { get; set; }
        public HashSet<MapAttribute> Attributes { get; set; }
        public double Elevation { get; set; }

        [JsonIgnore]
        public bool IsWater => this.Attributes.Contains(MapAttribute.Water);
        [JsonIgnore]
        public bool IsLand => this.Attributes.Contains(MapAttribute.Land);
        [JsonIgnore]
        public bool IsCoast => this.Attributes.Contains(MapAttribute.Coast);

        public MapCell()
        {
            this.MapPointIds = new List<uint>();
            this.MapPoints = new List<MapPoint>();
            this.AdjacentMapCellIds = new List<uint>();
            this.AdjacentMapCells = new List<MapCell>();
            this.Attributes = new HashSet<MapAttribute>();
            this.Center = new MapPoint(0, 0);
        }

    }
}
