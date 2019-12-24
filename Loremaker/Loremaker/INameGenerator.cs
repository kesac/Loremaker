using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    interface INameGenerator : ILocationNameGenerator, ICharacterNameGenerator, IOrganizationNameGenerator
    {
        string Next();
    }
}
