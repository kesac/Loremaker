using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{

    public enum PopulationCenterType
    {
        Village, 
        Town, 
        City
    }

    public class PopulationCenter
    {
        public string Name { get; set; }
        public PopulationCenterType Type { get; set; }
    }

}
