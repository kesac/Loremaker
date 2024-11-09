using Syllabore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    /// <summary>
    /// Represents one or more entities that can be captured
    /// as a single word of text. Entities can have adjectives,
    /// determiners, or form part of a name. TextEntities produce
    /// <see cref="TextOutputOld"/>.
    /// </summary>
    public class TextEntityPoolOld : ITextGeneratorOld
    {
        public List<TextEntityOld> Entities { get; set; }

        private List<TextEntityOld> LastAddedEntities { get; set; }

        public TextEntityPoolOld()
        {
            this.Entities = new List<TextEntityOld>();
        }

        public TextEntityPoolOld As(params string[] objects)
        {
            this.LastAddedEntities = objects.Select(x => new TextEntityOld(x)).ToList();
            this.Entities.AddRange(this.LastAddedEntities);
            return this;
        }

        public TextEntityPoolOld As(Func<TextEntityOld, TextEntityOld> configure)
        {
            var entity = configure(new TextEntityOld(string.Empty));
            this.LastAddedEntities = new List<TextEntityOld>() { entity };
            this.Entities.Add(entity);
            return this;
        }

        public TextEntityPoolOld ApplyAdjectives(params string[] adjectives)
        {
            this.LastAddedEntities.ForEach(x => x.Adjectives.AddRange(adjectives));
            return this;
        }

        public TextEntityPoolOld ApplyAdjectivesToAll(params string[] adjectives)
        {
            this.Entities.ForEach(x => x.Adjectives.AddRange(adjectives));
            return this;
        }

        public TextEntityPoolOld ApplyDeterminers(params string[] determiners)
        {
            this.LastAddedEntities.ForEach(x => x.Determiners.AddRange(determiners));
            return this;
        }

        public TextEntityPoolOld ApplyDeterminersToAll(params string[] determiners)
        {
            this.Entities.ForEach(x => x.Determiners.AddRange(determiners));
            return this;
        }

        public TextEntityPoolOld ApplyNameGenerator(INameGenerator nameGenerator)
        {
            this.LastAddedEntities.ForEach(x => x.NameGenerator = nameGenerator);
            return this;
        }

        public TextEntityPoolOld ApplyNameGeneratorToAll(INameGenerator nameGenerator)
        {
            this.Entities.ForEach(x => x.NameGenerator = nameGenerator);
            return this;
        }

        public TextEntityPoolOld ApplyNameGenerator(Func<NameGenerator, NameGenerator> configure)
        {
            var generator = configure(new NameGenerator());
            this.LastAddedEntities.ForEach(x => x.NameGenerator = generator);
            return this;
        }

        public TextEntityPoolOld ApplyNameGeneratorToAll(Func<NameGenerator, NameGenerator> configure)
        {
            var generator = configure(new NameGenerator());
            this.Entities.ForEach(x => x.NameGenerator = generator);
            return this;
        }

        public string Next()
        {
            return NextOutput().Value;
        }

        public TextOutputOld NextOutput()
        {
            var builder = new StringBuilder();
            var result = new TextOutputOld();
            var context = new List<string>();


            if (this.Entities.Count > 0)
            {

                var o = this.Entities.GetRandom<TextEntityOld>().Next();

                if (o.HasContextClues())
                {
                    context.AddRange(o.GetContextClues());
                    o = o.RemoveSquareBrackets();
                }

                builder.Append(" ");
                builder.Append(o);
            }

            /*
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
            /**/

            result.Value = builder.ToString().Trim();
            result.Context.AddRange(context);
            return result;
            
        }



    }
}
