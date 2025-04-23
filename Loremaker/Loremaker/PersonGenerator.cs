using Archigen;
using Loremaker.Names;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class PersonGenerator : IGenerator<Person>
    {
        private Random _random;
        private IGenerator<string> _nameGenerator;

        public PersonGenerator()
        {
            _random = new Random();
            _nameGenerator = new DefaultNameGenerator();
        }

        public Person Next()
        {
            var result = new Person();
            result.GivenName = _nameGenerator.Next();
            result.FamilyName = _nameGenerator.Next();
            result.Age = _random.Next(20, 100);
            return result;
        }

    }
}
