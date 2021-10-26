using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    public interface ITextGenerator : IGenerator<string>
    {
        // string Next();
        TextOutput NextOutput();
    }
}
