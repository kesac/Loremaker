using Archigen;
using System;
using System.Collections.Generic;

namespace Loremaker.Text
{
    /// <summary>
    /// A generator that returns random elements
    /// from a specified list.
    /// </summary>
    public class Randomizer<T> : IGenerator<T>
    {
        protected Random _random;

        /// <summary>
        /// The list of values that the generator will
        /// use to generate output during calls to <see cref="Next()"/>.
        /// All values are equally likely to be selected.
        /// </summary>
        public List<T> Values { get; set; }

        /// <summary>
        /// Creates a new <see cref="Randomizer{T}"/>
        /// that will return random values from the specified list.
        /// </summary>
        public Randomizer(IEnumerable<T> values)
        {
            _random = new Random();
            Values = new List<T>(values);
        }

        /// <summary>
        /// Returns a random value from a list of strings.
        /// </summary>
        public virtual T Next()
        {
            return Values[_random.Next(Values.Count)];
        }
    }
}
