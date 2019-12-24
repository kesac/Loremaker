using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    interface ILocationNameGenerator
    {
        string NextWorldName();
        string NextContinentName();
        string NextRegionName();
        string NextSettlementName();

    }
}
