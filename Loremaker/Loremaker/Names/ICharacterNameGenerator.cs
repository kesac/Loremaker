using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Names
{
    public interface ICharacterNameGenerator
    {
        string NextName();
        string NextFamilyName();
    }
}
