using Syllabore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Text
{
    public class TextChain : ITextGenerator
    {
        public List<TextTemplate> Templates { get; set; }
        public Dictionary<string, ITextGenerator> GlobalEntities { get; set; }

        public TextChain()
        {
            this.Templates = new List<TextTemplate>();
            this.GlobalEntities = new Dictionary<string, ITextGenerator>();
        }

        /// <summary>
        /// Used to define <c>TextEntities</c> for use across multiple <c>TextTemplates</c>
        /// in a <c>TextChain</c>.
        /// </summary>
        public TextChain DefineGlobally(string key, Func<TextEntityPool, TextEntityPool> configureEntity)
        {
            var e = configureEntity(new TextEntityPool());

            if (this.GlobalEntities.Keys.Contains(key))
            {
                throw new InvalidOperationException(string.Format("A global entity with key '{0}' was previously defined.", key));
            }
            else
            {
                this.GlobalEntities[key] = e;
            }

            return this;
        }

        public TextChain DefineGlobally(string key, ITextGenerator generator)
        {
            if (this.GlobalEntities.Keys.Contains(key))
            {
                throw new InvalidOperationException(string.Format("A global entity with key '{0}' was previously defined.", key));
            }
            else
            {
                this.GlobalEntities[key] = generator;
            }

            return this;
        }

        public TextChain DefineGlobally(string key, INameGenerator generator)
        {
            this.DefineGlobally(key, new TextEntity().UsingNameGenerator(generator));
            return this;
        }

        public TextChain DefineGlobally(string key, params string[] substitutions)
        {
            this.DefineGlobally(key, x => x.As(substitutions));
            return this;
        }

        public TextChain Append(string template)
        {
            this.Templates.Add(new TextTemplate(template));
            return this;
        }

        public TextChain Append(string template, Func<TextTemplate, TextTemplate> configure)
        {
            this.Templates.Add(configure(new TextTemplate(template)));
            return this;
        }

        public string Next()
        {
            var result = new StringBuilder();
            var previousValues = new Dictionary<string, string>();
            var currentContext = new List<string>();

            foreach(var template in this.Templates)
            {
                if (template.IsFulfilledBy(currentContext))
                {
                    var output = template.NextOutput(this.GlobalEntities, previousValues);
                    previousValues.AddRange(output.TextEntityOutput);
                    currentContext.AddRange(output.Context); // adds new context hints

                    result.Append(output.Value);
                    result.Append(" ");
                }
            }

            return result.ToString().Trim();
        }

        public TextOutput NextOutput()
        {
            // TODO: Should contain contexts?
            return new TextOutput(this.Next());
        }
    }
}
