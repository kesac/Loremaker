using Archigen;
using Syllabore;
using Syllabore.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    /// <summary>
    /// Text generator that outputs sentences of "cohesive" 
    /// gibberish. Cohesive meaning that only a limited set
    /// of phonemes are used to generate words. This makes the
    /// gibberish look like some kind of language.
    /// </summary>
    public class GibberishGenerator : IGenerator<string>
    {
        private ISyllableGenerator _generator;

        /// <summary>
        /// The number of sentences to generate on each call to <see cref="Next"/>.
        /// The default number of sentences is 1.
        /// </summary>
        public int SentenceLength { get; set; }

        /// <summary>
        /// A word that will be used like a conjunction
        /// to make the gibberish seem it has a structure.
        /// This is randomly generated on instantiation.
        /// </summary>
        public string Conjunction { get; set; }

        public GibberishGenerator()
        {
            var syllables = new SyllableGenerator()
                .Add(SymbolPosition.First, "strlmnbcd")
                .Add(SymbolPosition.Middle, "aeio");

            _generator = new SyllableSet(syllables, 16);

            this.SentenceLength = 1;
            this.Conjunction = _generator.Next().ToLower();
        }

        /// <summary>
        /// Sets the number of sentences to generate on each call to <see cref="Next"/>.
        /// The default number of sentences is 1.
        /// </summary>
        public GibberishGenerator UsingSentenceLength(int length)
        {
            this.SentenceLength = length;
            return this;
        }

        /// <summary>
        /// Returns structured gibberish!
        /// </summary>
        public string Next()
        {
            var result = new StringBuilder();
            var wordLength = Chance.Between(8, 12);

            for (int h = 0; h < this.SentenceLength; h++)
            {
                result.Append(NextSentence());
                result.Append(" ");
            }

            return result.ToString().Trim();
        }

        private string NextSentence()
        {
            var result = new StringBuilder();
            var wordLength = Chance.Between(8, 12);

            for (int i = 0; i < wordLength; i++)
            {
                var syllableLength = Chance.Between(2, 4);
                for (int j = 0; j < syllableLength; j++)
                {
                    result.Append(_generator.Next());
                }

                if (Chance.Roll(0.05) && i < wordLength - 1)
                {
                    result.Append(",");
                }

                if (Chance.Roll(0.20))
                {
                    result.Append(" " + this.Conjunction);
                }

                result.Append(" ");
            }

            return result[0].ToString().ToUpper() + result.ToString().Substring(1).Trim() + ".";
        }


    }
}
