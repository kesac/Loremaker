using Archigen;
using Loremaker.Names;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class PersonGenerator : IGenerator<Person>
    {
        private IGenerator<string> _nameGenerator;

        public PersonGenerator()
        {
            _nameGenerator = new DefaultNameGenerator();
        }

        public Person Next()
        {
            var result = new Person();
            result.GivenName = _nameGenerator.Next();
            result.FamilyName = _nameGenerator.Next();
            return result;
        }

    }
}
