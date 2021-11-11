using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public enum MapAttribute
    {
        Water,
        Land,
        Coast
    }

    public class MapCell
    {
        public int CellId { get; set; }
        public MapPoint Center { get; set; }
        public List<MapPoint> Shape { get; set; }
        public List<int> AdjacentCellIds { get; set; }
        public HashSet<MapAttribute> Attributes { get; set; }
        public double Elevation { get; set; }

        public bool IsWater => this.Attributes.Contains(MapAttribute.Water);
        public bool IsLand => this.Attributes.Contains(MapAttribute.Land);
        public bool IsCoast => this.Attributes.Contains(MapAttribute.Coast);

        public MapCell()
        {
            this.Shape = new List<MapPoint>();
            this.AdjacentCellIds = new List<int>();
            this.Attributes = new HashSet<MapAttribute>();
            this.Center = new MapPoint(0, 0);
        }

    }
}
