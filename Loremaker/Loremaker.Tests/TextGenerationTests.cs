using Loremaker.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syllabore;

namespace Loremaker.Tests
{
    [TestClass]
    public class TextGenerationTests
    {
        [TestMethod]
        public void TextEntity_NoValueGeneratesBlankString()
        {
            var e = new TextEntity();
            Assert.IsTrue(e.Next() == string.Empty);
        }

        [TestMethod]
        public void TextEntity_ValuePropertyAppearsInOutput()
        {
            var e = new TextEntity("planet");
            Assert.IsTrue(e.Next() == "planet");
        }

        [TestMethod]
        public void TextEntity_AdjectivesAppearInOutput()
        {

            string[] adjectives = new string[] { "large", "red", "fragile" };
            bool[] adjectiveAppeared = new bool[adjectives.Length];

            var e = new TextEntity("planet").UsingAdjectives(adjectives);

            for(int i = 0; i < 100; i++)
            {
                var output = e.Next();

                for (int j = 0; j < adjectiveAppeared.Length; j++)
                {
                    if(output.Contains(adjectives[j]))
                    {
                        adjectiveAppeared[j] = true;
                    }
                }
            }

            for(int i = 0; i < adjectiveAppeared.Length; i++)
            {
                Assert.IsTrue(adjectiveAppeared[i]);
            }

        }

        [TestMethod]
        public void TextEntity_DeterminersAppearInOutput()
        {

            string[] determiners = new string[] { "a", "the", "one" };
            bool[] determinersAppeared = new bool[determiners.Length];

            var e = new TextEntity("planet").UsingDeterminers(determiners);

            for (int i = 0; i < 100; i++)
            {
                var output = e.Next();

                for (int j = 0; j < determinersAppeared.Length; j++)
                {
                    if (output.Contains(determiners[j]))
                    {
                        determinersAppeared[j] = true;
                    }
                }
            }

            for (int i = 0; i < determinersAppeared.Length; i++)
            {
                Assert.IsTrue(determinersAppeared[i]);
            }

        }

        [TestMethod]
        public void TextEntity_NamesAppearInOutput()
        {

            var g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("ae")
                        .WithLeadingConsonants("str"));

            var e = new TextEntity("planet").UsingNameGenerator(g);

            for (int i = 0; i < 100; i++)
            {
                var output = e.Next();
                Assert.IsTrue(output.Replace("planet", string.Empty).Length > 0);
            }

        }

        [TestMethod]
        public void TextEntityPool_Placeholder()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TextTemplate_BasicSubstitutionsAppearInOutput()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TextTemplate_ITextGeneratorSubstitutionsAppearInOutput()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TextTemplate_INameGeneratorSubstitutionsAppearInOutput()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TextTemplate_FirstLetterCanBeCapitlized()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TextTemplate_ContextTagsAppearInTextOutputResult()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TextTemplate_ContextTagsCanBeValidated()
        {
            Assert.IsTrue(false);
        }



        [TestMethod]
        public void TextChain_Placeholder()
        {
            Assert.IsTrue(false);
        }

    }
}
