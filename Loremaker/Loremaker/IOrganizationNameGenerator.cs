using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    interface IOrganizationNameGenerator
    {
        string NextGovernmentName();
        string NextReligiousOrderName();
        string NextGuildName();
        string NextGroupName();
    }
}
