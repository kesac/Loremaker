using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public class TextTemplate
    {
        public string Template { get; set; }
        public HashSet<string> EntityKeys { get; set; }
        public Dictionary<string, TextEntity> Entities { get; set; }
        
        public List<string> MandatoryContextTags { get; set; } // Used by TextChain to generate context aware chains of text
        public List<string> RejectedContextTags { get; set; } 

        public bool EnableCapitalization { get; set; }

        public TextTemplate(string template)
        {
            this.Template = template;
            this.EntityKeys = new HashSet<string>();
            this.Entities = new Dictionary<string, TextEntity>();
            this.MandatoryContextTags = new List<string>();
            this.RejectedContextTags = new List<string>();

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



        /// <summary>
        /// Defines a new <see cref="TextEntity"/> and adds it to this TextTemplate.
        /// </summary>
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
        /// Convenience method for defining text substitutions if you don't
        /// need to specify determiners, adjectives, context, etc. 
        /// </summary>
        public TextTemplate Define(string key, params string[] substitutions)
        {
            this.Define(key, x => x.As(substitutions));
            return this;
        }

        public TextTemplate CapitalizeFirstWord()
        {
            this.EnableCapitalization = true;
            return this;
        }

        public TextTemplate DoNotCapitalizeFirstWord()
        {
            this.EnableCapitalization = false;
            return this;
        }

        /// <summary>
        /// Adds context tags that represent this template's context requirements.
        /// Context tags affect the return value of <c>MeetsContextRequirements()</c>
        /// </summary>
        public TextTemplate WhenContextHas(params string[] contextRequirements)
        {
            foreach(var c in contextRequirements)
            {
                this.MandatoryContextTags.Add(c.ToLower());
            }
            return this;
        }

        public TextTemplate AvoidWhenContextHas(params string[] rejectedContextRequirements)
        {
            foreach (var c in rejectedContextRequirements)
            {
                this.RejectedContextTags.Add(c.ToLower());
            }
            return this;
        }

        /// <summary>
        /// Returns true if all the provided context tags are in the 
        /// list of approved context tags and none are in the list of 
        /// rejected context tags..
        /// Context requirements are defined through <c>RequiredContext()</c>.
        /// This method is used in <c>TextChains</c> where text that appears
        /// earlier in a chain influences text added later to the chain.
        /// </summary>
        public bool IsFulfilledBy(List<string> contextTags)
        {
            if(contextTags.Count == 0) { return true; }

            bool isFullfilled = true;
            foreach(var tag in this.MandatoryContextTags)
            {
                if (!contextTags.Contains(tag.ToLower()))
                {
                    isFullfilled = false;
                    break;
                }
            }

            foreach (var tag in this.RejectedContextTags)
            {
                if (contextTags.Contains(tag.ToLower()))
                {
                    isFullfilled = false;
                    break;
                }
            }

            return isFullfilled;
        }

        public string Next()
        {
            return this.NextOutput(null, null).Value;
        }

        /// <summary>
        /// Generates text from the template provided to the constructor.
        /// Placeholders are replaced with TextEntities defined through
        /// calls to <c>Define()</c>.
        /// </summary>
        public TextOutput NextOutput()
        {
            return this.NextOutput(null, null);
        }



        /// TODO this method doesn't need two for-loops
        /// <summary>
        /// This method is used by <c>TextChain</c>.
        /// Identical to the parameterless <c>NextOutput()</c> except it
        /// takes a Dictionary of TextEntities to supplement this template's
        /// TextEntities (generation pulls from both pools of entities). This is useful
        /// when "globally" defining TextEntities across multiple templates. 
        /// This method also takes a Dictionary representing previously generated
        /// values. This is useful when the first output of a TextEntity must be
        /// used across multiple templates.
        /// </summary>
        /// 

        public TextOutput NextOutput(Dictionary<string, TextEntity> supplementaryEntities, Dictionary<string, string> overrideValues)
        {
            var result = new StringBuilder(this.Template);
            var output = new TextOutput();

            foreach (var m in Regex.Matches(result.ToString(), @"({[^}]+})"))
            {
                var bracketedKey = m.ToString();
                var key = bracketedKey.RemoveCurlyBrackets();

                if (overrideValues != null && overrideValues.ContainsKey(key))
                {
                    result.Replace("{" + key + "}", overrideValues[key]);
                    output.TextEntityOutput[key] = overrideValues[key];
                }
                else if (this.Entities.ContainsKey(key))
                {
                    var e = this.Entities[key].NextOutput();
                    result.Replace("{" + key + "}", e.Value);
                    output.Context.AddRange(e.Context);
                    output.TextEntityOutput[key] = e.Value;
                }
                else if (supplementaryEntities.ContainsKey(key))
                {
                    var s = supplementaryEntities[key].NextOutput();
                    result.Replace("{" + key + "}", s.Value);
                    output.Context.AddRange(s.Context);
                    output.TextEntityOutput[key] = s.Value;
                }
                

            }

            output.Value = result.ToString().Trim();

            // Are there are context clues in the template?
            if (output.Value.HasContextClues())
            {
                output.Context.AddRange(output.Value.GetContextClues());
                output.Value = output.Value.RemoveSquareBrackets();
            }

            if (this.EnableCapitalization)
            {
                output.Value = output.Value.Substring(0, 1).ToUpper() + output.Value.Substring(1);
            }

            return output;
        }


    }
}
