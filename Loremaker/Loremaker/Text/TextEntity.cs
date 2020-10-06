using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public class TextEntity
    {
        private Random Random { get; set; }

        public string Key { get; set; }
        public NameGenerator NameGenerator { get; set; }
        public List<string> Objects { get; set; }
        public List<string> Adjectives { get; set; }
        public List<string> Determiners { get; set; }

        public TextEntity(string key)
        {
            this.Key = key;
            this.Random = new Random();
            this.Objects = new List<string>();
            this.Adjectives = new List<string>();
            this.Determiners = new List<string>();
        }

        public TextEntity UsingNamesFrom(NameGenerator generator)
        {
            this.NameGenerator = generator;
            return this;
        }

        public TextEntity As(params string[] objects)
        {
            this.Objects.AddRange(objects);
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

        public string Next()
        {
            var result = new StringBuilder();

            if(this.NameGenerator != null)
            {
                result.Append(" ");
                result.Append(this.NameGenerator.Next());
            }

            if (this.Objects.Count > 0)
            {
                result.Append(" ");
                result.Append(this.Objects[this.Random.Next(this.Objects.Count)]);
            }

            if (this.Adjectives.Count > 0)
            {
                result.Insert(0, this.Adjectives[this.Random.Next(this.Adjectives.Count)]);
                result.Insert(0, " ");
            }

            if (this.Determiners.Count > 0)
            {

                var determiner = this.Determiners[this.Random.Next(this.Determiners.Count)];
               
                if(determiner == "a" && Regex.IsMatch(result.ToString().Trim(), "^[aieouAIEOU]"))
                {
                    determiner = "an";
                }

                result.Insert(0, determiner);
                result.Insert(0, " ");
            }

            return result.ToString().Trim();
        }

    }
}
