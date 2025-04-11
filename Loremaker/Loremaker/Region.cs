using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public class Region : IEntity, ILocatable
    {
        public uint Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public HashSet<uint> MapCellIds { get; set; }
        [JsonIgnore]
        public virtual List<MapCell> MapCells { get; set; }

        public Region()
        {
            this.MapCellIds = new HashSet<uint>();
            this.MapCells = new List<MapCell>();
        }

    }
}
