using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    /// <summary>
    /// Textomatic is an experimental generator. It functions similar to
    /// TextTemplate except that it randomly selects the initial template.
    /// </summary>
    public class Textomatic : IGenerator<string>
    {
        private Random _random;
        public IGenerator<string> Templates { get; set; }
        public Dictionary<string, IGenerator<string>> Generators { get; set; }


        public Textomatic(IGenerator<string> templates)
        {
            _random = new Random();
            this.Templates = templates;
            this.Generators = new Dictionary<string, IGenerator<string>>();
        }

        public void Define(string name, IGenerator<string> generator)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Generator name cannot be null or empty.", nameof(name));
            }
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            this.Generators[name] = generator;
        }

        public string Next()
        {
            var template = new TextTemplate(this.Templates.Next());

            foreach(var key in this.Generators.Keys)
            {
                var generator = this.Generators[key];
                template.Substitute(key, generator);
            }

            return template.Next();
        }

    }
}
