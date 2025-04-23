using Archigen;
using System.Collections.Generic;

namespace Loremaker.Text
{
    /// <summary>
    /// A generator that returns random 
    /// subjects with leading adjectives and determiners.
    /// </summary>
    public class SubjectRandomizer : IGenerator<string>
    {

        /// <summary>
        /// The list of subjects that the generator will
        /// return in output during calls to <see cref="Next()"/>.
        /// </summary>
        public RandomSelector<string> Values { get; set; }

        /// <summary>
        /// The list of determiners that the generator will
        /// include in the output during calls to <see cref="Next()"/>.
        /// This can be left empty. Determiners will appear before
        /// adjectives and the subject. All values are equally likely
        /// to be selected.
        /// </summary>
        public RandomSelector<string> Determiners { get; set; }

        /// <summary>
        /// The list of adjectives that the generator will
        /// include in the output during calls to <see cref="Next()"/>.
        /// The adjectives will appear before the subject, but after
        /// determiners. All values are equally likely to be selected.
        /// </summary>
        public RandomSelector<string> Adjectives { get; set; }

        /// <summary>
        /// Creates an empty <see cref="SubjectRandomizer"/>.
        /// </summary>
        public SubjectRandomizer()
        {
            Values = new RandomSelector<string>();
        }

        /// <summary>
        /// Creates a new <see cref="SubjectRandomizer"/> that
        /// will return subjects from the specified list.
        /// </summary>
        public SubjectRandomizer(IEnumerable<string> values)
        {
            Values = new RandomSelector<string>(values);
        }

        /// <summary>
        /// Creates a new <see cref="SubjectRandomizer"/> that
        /// will return subjects from the specified values.
        /// </summary>
        public SubjectRandomizer(params string[] values)
        {
            Values = new RandomSelector<string>(values);
        }

        /// <summary>
        /// Sets the determiners that the generator will use.
        /// </summary>
        public SubjectRandomizer SetValues(IEnumerable<string> values)
        {
            Values = new RandomSelector<string>(values);
            return this;
        }

        /// <summary>
        /// Sets the determiners that the generator will use.
        /// </summary>
        public SubjectRandomizer SetValues(params string[] values)
        {
            Values = new RandomSelector<string>(values);
            return this;
        }

        /// <summary>
        /// Sets the determiners that the generator will use.
        /// </summary>
        public SubjectRandomizer SetDeterminers(IEnumerable<string> determiners)
        {
            Determiners = new RandomSelector<string>(determiners);
            return this;
        }

        /// <summary>
        /// Sets the determiners that the generator will use.
        /// </summary>
        public SubjectRandomizer SetDeterminers(params string[] determiners)
        {
            Determiners = new RandomSelector<string>(determiners);
            return this;
        }

        /// <summary>
        /// Sets the adjectives that the generator will use.
        /// </summary>
        public SubjectRandomizer SetAdjectives(IEnumerable<string> adjectives)
        {
            Adjectives = new RandomSelector<string>(adjectives);
            return this;
        }

        /// <summary>
        /// Sets the adjectives that the generator will use.
        /// </summary>
        public SubjectRandomizer SetAdjectives(params string[] adjectives)
        {
            Adjectives = new RandomSelector<string>(adjectives);
            return this;
        }

        /// <summary>
        /// Generates a random subject in the format "[determiner] [adjective] [subject]".
        /// </summary>
        public string Next()
        {
            var result = Values.Next();

            if (Adjectives != null && Adjectives.Values.Count > 0)
            {
                result = Adjectives.Next() + " " + result;
            }

            if (Determiners != null && Determiners.Values.Count > 0)
            {
                result = Determiners.Next() + " " + result;
            }

            return result;
        }
    }
}
