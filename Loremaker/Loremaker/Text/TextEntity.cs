using Loremaker.Names;
using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    public class TextEntity : ITextGenerator
    {
        private Random Random { get; set; }
        public string Value { get; set; }
        public List<string> Adjectives { get; set; }
        public List<string> Determiners { get; set; }
        public INameGenerator NameGenerator { get; set; }


        public TextEntity(string value = "")
        {
            this.Value = value;
            this.Random = new Random();
            this.Adjectives = new List<string>();
            this.Determiners = new List<string>();
        }

        public TextEntity UsingValue(string value)
        {
            this.Value = value;
            return this;
        }

        public TextEntity UsingAdjectives(params string[] adjectives)
        {
            this.Adjectives.AddRange(adjectives);
            return this;
        }

        public TextEntity UsingDeterminers(params string[] determiners)
        {
            this.Determiners.AddRange(determiners);
            return this;
        }

        public TextEntity UsingNameGenerator(INameGenerator nameGenerator)
        {
            this.NameGenerator = nameGenerator;
            return this;
        }

        public TextEntity UsingNameGenerator(Func<NameGenerator, NameGenerator> configure)
        {
            this.NameGenerator = configure(new NameGenerator());
            return this;
        }

        public string Next()
        {
            return this.NextOutput().Value;
        }

        public TextOutput NextOutput()
        {
            var result = new TextOutput();
            var builder = new StringBuilder();

            if(this.Determiners.Count > 0)
            {
                builder.Append(this.Determiners[this.Random.Next(this.Determiners.Count)] + " ");
            }

            if (this.Adjectives.Count > 0)
            {
                builder.Append(this.Adjectives[this.Random.Next(this.Adjectives.Count)] + " ");
            }

            if (this.NameGenerator != null)
            {
                builder.Append(this.NameGenerator.Next() + " ");
            }

            builder.Append(this.Value);

            return new TextOutput(builder.ToString().Trim());

        }
    }
}
