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

        private void Add(ref List<string> list, string[] strings)
        {
            // There used to be additional logic here.
            // That's why this has been abstracted out.
            // Might not need it anymore.
            list.AddRange(strings);
        }

        public TextEntity As(params string[] objects)
        {
            var list = this.Objects;
            this.Add(ref list, objects);
            return this;
        }

        public TextEntity UsingAdjectives(params string[] adjectives)
        {
            var list = this.Adjectives;
            this.Add(ref list, adjectives);
            return this;
        }

        public TextEntity UsingDeterminers(params string[] determiners)
        {
            var list = this.Determiners;
            this.Add(ref list, determiners);
            return this;
        }
        public string Next()
        {
            return NextOutput().Value;
        }
        public TextOutput NextOutput()
        {
            var result = new StringBuilder();
            var output = new TextOutput();
            var context = new List<string>();

            if (this.NameGenerator != null)
            {
                result.Append(" ");
                result.Append(this.NameGenerator.Next());
            }

            if (this.Objects.Count > 0)
            {

                var o = this.Objects.GetRandom();

                if (o.HasContextClues())
                {
                    context.AddRange(o.GetContextClues());
                    o = o.RemoveSquareBrackets();
                }

                result.Append(" ");
                result.Append(o);
            }

            if (this.Adjectives.Count > 0)
            {

                var adjective = this.Adjectives.GetRandom();

                if (adjective.HasContextClues())
                {
                    context.AddRange(adjective.GetContextClues());
                    adjective = adjective.RemoveSquareBrackets();
                }

                result.Insert(0, adjective);
                result.Insert(0, " ");
            }

            if (this.Determiners.Count > 0)
            {

                var determiner = this.Determiners.GetRandom();

                if (determiner.HasContextClues())
                {
                    context.AddRange(determiner.GetContextClues());
                    determiner = determiner.RemoveSquareBrackets();
                }

                if (determiner == "a" && result.ToString().Trim().StartsWithVowel())
                {
                    determiner = "an";
                }
                else if (determiner == "an" && !result.ToString().Trim().StartsWithVowel())
                {
                    determiner = "a";
                }

                result.Insert(0, determiner);
                result.Insert(0, " ");
            }

            output.Value = result.ToString().Trim();
            output.Context.AddRange(context);
            return output;
            
        }



    }
}
