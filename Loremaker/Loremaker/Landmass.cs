using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Loremaker
{
    public class Landmass : Identifiable
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public MapPoint Center { get; set; }
        public List<uint> MapCellIds { get; set; }
        [IgnoreDataMember]
        public virtual List<MapCell> MapCells { get; set; }

        public Landmass()
        {
            this.MapCellIds = new List<uint>();
            this.MapCells = new List<MapCell>();
        }
    }
}
