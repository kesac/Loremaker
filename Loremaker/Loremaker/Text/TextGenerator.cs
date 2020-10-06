using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public class TextGenerator
    {
        private string TextTemplate;
        private HashSet<string> EntityKeys;
        private Dictionary<string, TextEntity> Entities;

        public TextGenerator(string text)
        {
            this.TextTemplate = text;
            this.EntityKeys = new HashSet<string>();
            this.Entities = new Dictionary<string, TextEntity>();

            foreach (var m in Regex.Matches(text, @"({[^}]+})"))
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

        public TextGenerator Define(string key, Func<TextEntity, TextEntity> configureEntity)
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


        public string Next()
        {
            var result = new StringBuilder(this.TextTemplate);
            foreach(string key in this.Entities.Keys)
            {
                result.Replace("{" + key + "}", this.Entities[key].Next());
            }
            return result.ToString();
        }

    }
}
