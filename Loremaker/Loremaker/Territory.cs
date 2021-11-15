using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public class Territory : Identifiable
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public MapPoint Center { get; set; }
        public HashSet<uint> MapCellIds { get; set; }
        [JsonIgnore]
        public virtual List<MapCell> MapCells { get; set; }

    

        public Territory()
        {
            this.MapCellIds = new HashSet<uint>();
            this.MapCells = new List<MapCell>();
        }

        
    }
}
