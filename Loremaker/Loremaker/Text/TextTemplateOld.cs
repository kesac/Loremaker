using Syllabore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    /// <summary>
    /// A TextTemplate is specfically formatted string where
    /// specific substrings can be substituted by <see cref="TextOutputOld"/>
    /// generated from <see cref="ITextGeneratorOld">ITextGenerators</see>,
    /// especially those created by <see cref="TextEntityPoolOld">TextEntities</see>
    /// </summary>
    public class TextTemplateOld : ITextGeneratorOld
    {
        public string Template { get; set; }

        // When a template is initantiated, only the placeholders for TextEntities are known.
        // When a TextEntity is defined, the specified entity ID is checked with the list
        // EntityPlaceholders before being stored in EntityDefinitions.
        public HashSet<string> EntityPlaceholders { get; set; } 
        public Dictionary<string, ITextGeneratorOld> EntityDefinitions { get; set; }
        
        public List<string> MandatoryContextTags { get; set; } // Used by TextChain to generate context aware chains of text
        public List<string> RejectedContextTags { get; set; } 

        public bool EnableCapitalization { get; set; }

        public TextTemplateOld(string template)
        {
            this.Template = template;
            this.EntityPlaceholders = new HashSet<string>();
            this.EntityDefinitions = new Dictionary<string, ITextGeneratorOld>();
            this.MandatoryContextTags = new List<string>();
            this.RejectedContextTags = new List<string>();

            foreach (var m in Regex.Matches(template, @"({[^}]+})"))
            {
                var s = m.ToString();
                var key = s.Substring(1, s.Length - 2);

                if (this.EntityPlaceholders.Contains(key))
                {
                    throw new InvalidOperationException(string.Format("An entity with key '{0}' already exists.", key));
                }
                else
                {
                    this.EntityPlaceholders.Add(key);
                }
            }

        }



        /// <summary>
        /// Defines a new <see cref="TextEntityPoolOld"/> and adds it to this TextTemplate.
        /// </summary>
        public TextTemplateOld Define(string key, Func<TextEntityPoolOld, TextEntityPoolOld> configureEntity)
        {
            var e = configureEntity(new TextEntityPoolOld());

            if (!this.EntityPlaceholders.Contains(key))
            {
                throw new InvalidOperationException(string.Format("No entity with key '{0}' was defined through the TextGenerator constructor.", key));
            }
            else
            {
                this.EntityDefinitions[key] = e;
            }

            return this;
        }

        public TextTemplateOld Define(string key, ITextGeneratorOld generator)
        {
            if (!this.EntityPlaceholders.Contains(key))
            {
                throw new InvalidOperationException(string.Format("No entity with key '{0}' was defined through the TextGenerator constructor.", key));
            }
            else
            {
                this.EntityDefinitions[key] = generator;
            }

            return this;
        }

        public TextTemplateOld Define(string key, INameGenerator generator)
        {
            this.Define(key, new TextEntityOld().UsingNameGenerator(generator));
            return this;
        }

        /// <summary>
        /// Convenience method for defining text substitutions if you don't
        /// need to specify determiners, adjectives, context, etc. 
        /// </summary>
        public TextTemplateOld Define(string key, params string[] substitutions)
        {
            this.Define(key, x => x.As(substitutions));
            return this;
        }

        public TextTemplateOld CapitalizeFirstWord()
        {
            this.EnableCapitalization = true;
            return this;
        }

        public TextTemplateOld DoNotCapitalizeFirstWord()
        {
            this.EnableCapitalization = false;
            return this;
        }

        /// <summary>
        /// Adds context tags that represent this template's context requirements.
        /// Context tags affect the return value of <c>MeetsContextRequirements()</c>
        /// </summary>
        public TextTemplateOld WhenContextHas(params string[] contextRequirements)
        {
            foreach(var c in contextRequirements)
            {
                this.MandatoryContextTags.Add(c.ToLower());
            }
            return this;
        }

        public TextTemplateOld AvoidWhenContextHas(params string[] rejectedContextRequirements)
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
        public TextOutputOld NextOutput()
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

        public TextOutputOld NextOutput(Dictionary<string, ITextGeneratorOld> supplementaryEntities, Dictionary<string, string> overrideValues)
        {
            var result = new StringBuilder(this.Template);
            var output = new TextOutputOld();

            foreach (var m in Regex.Matches(result.ToString(), @"({[^}]+})"))
            {
                var bracketedKey = m.ToString();
                var key = bracketedKey.RemoveCurlyBrackets();

                if (overrideValues != null && overrideValues.ContainsKey(key))
                {
                    result.Replace("{" + key + "}", overrideValues[key]);
                    output.TextEntityOutput[key] = overrideValues[key];
                }
                else if (this.EntityDefinitions.ContainsKey(key))
                {
                    var e = this.EntityDefinitions[key].NextOutput();
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
