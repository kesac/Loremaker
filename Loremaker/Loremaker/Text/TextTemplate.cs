using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public class TextTemplate
    {
        private string Template;
        private HashSet<string> EntityKeys;
        private Dictionary<string, TextEntity> Entities;
        
        
        private List<string> ContextRequirements; // Used by TextChain to generate context aware chains of text

        public TextTemplate(string template)
        {
            this.Template = template;
            this.EntityKeys = new HashSet<string>();
            this.Entities = new Dictionary<string, TextEntity>();
            this.ContextRequirements = new List<string>();

            foreach (var m in Regex.Matches(template, @"({[^}]+})"))
            {
                var s = m.ToString();
                var key = s.Substring(1, s.Length - 2);

                if (this.EntityKeys.Contains(key))
                {
                    throw new InvalidOperationException(string.Format("An entity with key '{0}' already exists.", key));
                }
                else
                {
                    this.EntityKeys.Add(key);
                }
            }

        }

        public TextTemplate Define(string key, Func<TextEntity, TextEntity> configureEntity)
        {
            var e = configureEntity(new TextEntity(key));

            if (!this.EntityKeys.Contains(key))
            {
                throw new InvalidOperationException(string.Format("No entity with key '{0}' was defined through the TextGenerator constructor.", key));
            }
            else
            {
                this.Entities[key] = e;
            }

            return this;
        }

        /// <summary>
        /// Adds context tags that represent this template's context requirements.
        /// Context tags affect the return value of <c>MeetsContextRequirements()</c>
        /// </summary>
        public TextTemplate RequiresContext(params string[] contextRequirements)
        {
            foreach(var c in contextRequirements)
            {
                this.ContextRequirements.Add(c.ToLower());
            }
            return this;
        }

        /// <summary>
        /// Returns true if all the provided context tags are in the 
        /// pre-defined list of context requirements of this template.
        /// Context requirements are defined through <c>RequiredContext()</c>.
        /// This method is used in <c>TextChains</c> where text that appears
        /// earlier in a chain influences text added later to the chain.
        /// </summary>
        public bool IsFulfilledBy(params string[] context)
        {
            if(context.Length == 0) { return true; }

            bool usesContext = true;
            foreach(var c in this.ContextRequirements)
            {
                if (!context.Contains(c.ToLower()))
                {
                    usesContext = false;
                    break;
                }
            }
            return usesContext;
        }

        public string Next()
        {
            return this.NextOutput(null).Value;
        }

        /// <summary>
        /// Generates text from the template provided to the constructor.
        /// Placeholders are replaced with TextEntities defined through
        /// calls to <c>Define()</c>.
        /// </summary>
        public TextOutput NextOutput()
        {
            return this.NextOutput(null);
        }


        /// <summary>
        /// Identical to the parameterless <c>NextOutput()</c> except it
        /// also uses the specified Dictionary of TextEntities to replace
        /// placeholders. This method is used in <c>TextChains</c>.
        /// </summary>
        public TextOutput NextOutput(Dictionary<string, TextEntity> supplementaryEntities)
        {
            var result = new StringBuilder(this.Template);
            var output = new TextOutput();

            foreach (string key in this.Entities.Keys)
            {
                var entity = this.Entities[key].NextOutput();
                output.Context.AddRange(entity.Context);
                output.TextEntityOutput[key] = entity.Value;
                result.Replace("{" + key + "}", entity.Value);
            }


            // If there are still placeholders and supplementary text entities were supplied...
            if (Regex.IsMatch(result.ToString(), @"({[^}]+})") && supplementaryEntities != null)
            {
                foreach (var m in Regex.Matches(result.ToString(), @"({[^}]+})"))
                {
                    var s = m.ToString();
                    var key = s.Substring(1, s.Length - 2);

                    if (supplementaryEntities.Keys.Contains(key))
                    {
                        var entity = supplementaryEntities[key].NextOutput();
                        output.Context.AddRange(entity.Context);
                        output.TextEntityOutput[key] = entity.Value;
                        result.Replace("{" + key + "}", entity.Value);
                    }
                    
                }
            }

            output.Value = result.ToString().Trim();

            // Are there are context clues in the template?
            if (output.Value.HasContextClues())
            {
                output.Context.AddRange(output.Value.GetContextClues());
                output.Value = output.Value.RemoveSquareBrackets();
            }

            return output;
        }

    }
}
