using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Text
{
    public class TextChain
    {

        private List<TextTemplate> Templates;
        private Dictionary<string, TextEntity> GlobalEntities;

        public TextChain()
        {
            this.Templates = new List<TextTemplate>();
            this.GlobalEntities = new Dictionary<string, TextEntity>();
        }

        public TextChain DefineGlobally(string key, Func<TextEntity, TextEntity> configureEntity)
        {
            var e = configureEntity(new TextEntity(key));

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

        public TextChain Append(string template, Func<TextTemplate, TextTemplate> configure)
        {
            this.Templates.Add(configure(new TextTemplate(template)));
            return this;
        }

        public string Next()
        {
            var result = new StringBuilder();
            var currentContext = new List<string>();

            foreach(var template in this.Templates)
            {
                if (template.IsFulfilledBy(currentContext.ToArray()))
                {
                    var output = template.NextOutput(this.GlobalEntities);
                    currentContext.AddRange(output.Context); // adds new context hints

                    result.Append(output.Value);
                    result.Append(" ");
                }
            }

            return result.ToString().Trim();
        }

    }
}
