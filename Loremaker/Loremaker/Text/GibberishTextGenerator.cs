using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Text
{
    /// <summary>
    /// Experimental text generator that outputs
    /// sentences of "cohesive" gibberish. Cohesive
    /// in the way that only a limited set of phonemes
    /// are used to generate the gibberish much like a
    /// real language.
    /// </summary>
    public class GibberishTextGenerator : ITextGenerator
    {

        public ISyllableProvider SyllableGenerator { get; set; }

        public int SentenceLength { get; set; }

        public string Conjunction { get; set; }

        public GibberishTextGenerator()
        {
            this.SyllableGenerator = new SyllableSet(8, 32, 4)
                .InitializeWith(x => x
                    .WithVowels("ae").Weight(4)
                    .WithVowels("i").Weight(2)
                    .WithVowels("ou").Weight(1)
                    .WithLeadingConsonants("hlmnstr").Weight(4)
                    .WithLeadingConsonants("dpc").Weight(2)
                    .WithLeadingConsonants("bgv").Weight(1));

            this.SentenceLength = 1;

            this.Conjunction = this.SyllableGenerator.NextStartingSyllable().ToLower();
        }

        public GibberishTextGenerator UsingSentenceLength(int length)
        {
            this.SentenceLength = length;
            return this;
        }

        public string NextSentence()
        {
            var result = new StringBuilder();
            var wordLength = Chance.Between(8, 12);

            for (int i = 0; i < wordLength; i++)
            {
                var syllableLength = Chance.Between(2, 4);
                for (int j = 0; j < syllableLength; j++)
                {
                    result.Append(this.SyllableGenerator.NextSyllable());
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

        public TextOutput NextOutput()
        {
            return new TextOutput(this.Next());
        }

        public string Next()
        {
            var result = new StringBuilder();
            var wordLength = Chance.Between(8, 12);

            for (int h = 0; h < this.SentenceLength; h++)
            {
                result.Append(NextSentence());
                result.Append(" ");
            }

            return result.ToString().Trim(); ;
        }
    }
}
