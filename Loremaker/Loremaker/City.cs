using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{

    public enum CityType
    {
        Unknown,
        Village, 
        Town, 
        City
    }

    public class City : IEntity
    {
        public uint Id { get; set; }
        public uint MapCellId { get; set; }
        [JsonIgnore]
        public virtual MapCell MapCell { get; set; }        
        [JsonIgnore]
        public virtual Continent Landmass { get; set; }
        public string Name { get; set; }
        public CityType Type { get; set; }
    }

}
