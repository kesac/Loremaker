﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    /// <summary>
    /// A instance of this class represents a string
    /// generated by a ITextGenerator that has context tags.
    /// </summary>
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

        public TextOutput(string value) : base()
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value;
        }

    }
}
