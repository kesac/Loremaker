using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Loremaker
{
    public class LandmassRegion : Identifiable
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint ParentId { get; set; }
        [IgnoreDataMember]
        public virtual Landmass Parent { get; set; }
        public List<uint> MapCellIds { get; set; }
        [IgnoreDataMember]
        public virtual List<MapCell> MapCells { get; set; }

        public LandmassRegion()
        {
            this.MapCellIds = new List<uint>();
            this.MapCells = new List<MapCell>();
        }

    }
}
