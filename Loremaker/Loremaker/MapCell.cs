using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
        public List<MapPoint> Shape { get; set; }
        public List<uint> AdjacentMapCellIds { get; set; }
        [IgnoreDataMember]
        public virtual List<MapCell> AdjacentMapCells { get; set; }
        public HashSet<MapAttribute> Attributes { get; set; }
        public double Elevation { get; set; }

        [IgnoreDataMember]
        public bool IsWater => this.Attributes.Contains(MapAttribute.Water);
        [IgnoreDataMember]
        public bool IsLand => this.Attributes.Contains(MapAttribute.Land);
        [IgnoreDataMember]
        public bool IsCoast => this.Attributes.Contains(MapAttribute.Coast);

        public MapCell()
        {
            this.Shape = new List<MapPoint>();
            this.AdjacentMapCellIds = new List<uint>();
            this.AdjacentMapCells = new List<MapCell>();
            this.Attributes = new HashSet<MapAttribute>();
            this.Center = new MapPoint(0, 0);
        }

    }
}
