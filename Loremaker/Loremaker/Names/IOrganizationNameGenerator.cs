using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Names
{
    public interface IOrganizationNameGenerator
    {
        string NextGovernmentName();
        string NextReligiousOrderName();
        string NextGuildName();
        string NextFactionName();
    }
}
