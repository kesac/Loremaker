using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Names
{
    public interface ILocationNameGenerator
    {
        string NextWorldName();
        string NextContinentName();
        string NextRegionName();
        string NextSettlementName();

    }
}
