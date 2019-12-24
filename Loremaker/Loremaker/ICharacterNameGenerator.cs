using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    interface ICharacterNameGenerator
    {
        string NextName();
        string NextFamilyName();
    }
}
