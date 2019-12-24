using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{

    public enum SettlementType
    {
        Village, 
        Town, 
        City
    }

    public class Settlement
    {
        public string Name { get; set; }
        public SettlementType Type { get; set; }
    }

}
