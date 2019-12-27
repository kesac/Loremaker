using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Names
{
    public interface INameGenerator : ILocationNameGenerator, ICharacterNameGenerator, IOrganizationNameGenerator
    {
        string Next();
    }
}
