using Syllabore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Text
{
    public class TextChainOld : ITextGeneratorOld
    {
        public List<TextTemplateOld> Templates { get; set; }
        public Dictionary<string, ITextGeneratorOld> GlobalEntities { get; set; }

        public TextChainOld()
        {
            this.Templates = new List<TextTemplateOld>();
            this.GlobalEntities = new Dictionary<string, ITextGeneratorOld>();
        }

        /// <summary>
        /// Used to define <c>TextEntities</c> for use across multiple <c>TextTemplates</c>
        /// in a <c>TextChain</c>.
        /// </summary>
        public TextChainOld DefineGlobally(string key, Func<TextEntityPoolOld, TextEntityPoolOld> configureEntity)
        {
            var e = configureEntity(new TextEntityPoolOld());

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

        public TextChainOld DefineGlobally(string key, ITextGeneratorOld generator)
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

        public TextChainOld DefineGlobally(string key, INameGenerator generator)
        {
            this.DefineGlobally(key, new TextEntityOld().UsingNameGenerator(generator));
            return this;
        }

        public TextChainOld DefineGlobally(string key, params string[] substitutions)
        {
            this.DefineGlobally(key, x => x.As(substitutions));
            return this;
        }

        public TextChainOld Append(string template)
        {
            this.Templates.Add(new TextTemplateOld(template));
            return this;
        }

        public TextChainOld Append(string template, Func<TextTemplateOld, TextTemplateOld> configure)
        {
            this.Templates.Add(configure(new TextTemplateOld(template)));
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

        public TextOutputOld NextOutput()
        {
            // TODO: Should contain contexts?
            return new TextOutputOld(this.Next());
        }
    }
}
