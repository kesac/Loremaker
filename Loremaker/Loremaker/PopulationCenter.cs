using Loremaker.Maps;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
        [IgnoreDataMember]
        public virtual MapCell MapCell { get; set; }
        public string Name { get; set; }
        public PopulationCenterType Type { get; set; }
    }

}
