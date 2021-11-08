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
        public List<MapAttribute> Attributes { get; set; }
        public double Elevation { get; set; }

        public bool IsWater { 
            get
            {
                return this.Attributes.Contains(MapAttribute.Water);
            } 
        }

        public bool IsLand
        {
            get
            {
                return this.Attributes.Contains(MapAttribute.Land);
            }
        }

        public bool IsCoast
        {
            get
            {
                return this.Attributes.Contains(MapAttribute.Coast);
            }
        }

        public MapCell()
        {
            this.Shape = new List<MapPoint>();
            this.AdjacentCellIds = new List<int>();
            this.Attributes = new List<MapAttribute>();
            this.Center = new MapPoint(0, 0);
        }

    }
}
