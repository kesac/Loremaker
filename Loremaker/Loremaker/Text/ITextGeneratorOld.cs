using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    public interface ITextGeneratorOld : IGenerator<string>
    {
        // string Next();
        TextOutputOld NextOutput();
    }
}
