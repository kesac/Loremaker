using System.Collections.Generic;

namespace Loremaker.Text
{
    /// <summary>
    /// A generator that returns random 
    /// subjects with leading adjectives and determiners.
    /// </summary>
    public class SubjectRandomizer : Randomizer<string>
    {
        /// <summary>
        /// The list of determiners that the generator will
        /// include in the output during calls to <see cref="Next()"/>.
        /// This can be left empty. Determiners will appear before
        /// adjectives and the subject. All values are equally likely
        /// to be selected.
        /// </summary>
        public Randomizer<string> Determiners { get; set; }

        /// <summary>
        /// The list of adjectives that the generator will
        /// include in the output during calls to <see cref="Next()"/>.
        /// The adjectives will appear before the subject, but after
        /// determiners. All values are equally likely to be selected.
        /// </summary>
        public Randomizer<string> Adjectives { get; set; }

        /// <summary>
        /// Creates a new <see cref="SubjectRandomizer"/> that
        /// will return subjects from the specified list.
        /// </summary>
        public SubjectRandomizer(IEnumerable<string> values) : base(values) { }

        /// <summary>
        /// Sets the determiners that the generator will use.
        /// </summary>
        public void SetDeterminers(IEnumerable<string> determiners)
        {
            Determiners = new Randomizer<string>(determiners);
        }

        /// <summary>
        /// Sets the adjectives that the generator will use.
        /// </summary>
        public void SetAdjectives(IEnumerable<string> adjectives)
        {
            Adjectives = new Randomizer<string>(adjectives);
        }

        /// <summary>
        /// Generates a random subject in the format "[determiner] [adjective] [subject]".
        /// </summary>
        public override string Next()
        {
            var result = base.Next();

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
