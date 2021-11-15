using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public class Landmass : Identifiable, Locatable
    {
        public uint Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public List<uint> MapCellIds { get; set; }
        [JsonIgnore]
        public virtual List<MapCell> MapCells { get; set; }

        [JsonIgnore]
        public int Size 
        { 
            get
            {
                return this.MapCellIds.Count;
            }
        }

        public Landmass()
        {
            this.MapCellIds = new List<uint>();
            this.MapCells = new List<MapCell>();
        }
    }
}
