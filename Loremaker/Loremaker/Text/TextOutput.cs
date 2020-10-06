using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    public class TextOutput
    {
        public string Value { get; set; }
        public List<string> Context { get; private set; }
        public Dictionary<string, string> TextEntityOutput {get; private set;}

        public TextOutput()
        {
            this.Context = new List<string>();
            this.TextEntityOutput = new Dictionary<string, string>();
        }

    }
}
