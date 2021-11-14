using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{

    public enum PopulationCenterType
    {
        Unknown,
        Village, 
        Town, 
        City
    }

    public class PopulationCenter : Identifiable
    {
        public uint Id { get; set; }
        public uint MapCellId { get; set; }
        [JsonIgnore]
        public virtual MapCell MapCell { get; set; }        
        [JsonIgnore]
        public virtual Landmass Landmass { get; set; }
        public string Name { get; set; }
        public PopulationCenterType Type { get; set; }
    }

}
