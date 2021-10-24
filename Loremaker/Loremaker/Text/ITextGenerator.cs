using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    public interface ITextGenerator
    {
        string Next();
        TextOutput NextOutput();
    }
}
