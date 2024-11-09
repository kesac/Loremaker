using Loremaker.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syllabore;
using System;
using System.Diagnostics;

namespace Loremaker.Tests
{
    [TestClass]
    public class TextGenerationTests
    {
        [TestMethod]
        public void TextEntity_NoValueGeneratesBlankString()
        {
            var e = new TextTemplate();
            Assert.IsTrue(e.Next() == string.Empty);
        }

        [TestMethod]
        public void TextEntity_ValuePropertyAppearsInOutput()
        {
            var template = new TextTemplate();
            template.Add("planet");

            var result = template.Next();

            Assert.IsTrue(result == "planet");
        }

        [TestMethod]
        public void TextEntity_AdjectivesAppearInOutput()
        {

            var planetNames = new string[] { "Elnea", "Setaphor Prime", "Gestadomeia" };
            var planetAdjectives = new string[] { "green", "bright blue" };
            bool[] adjectiveAppeared = new bool[planetAdjectives.Length];


            var planetRandomizer = new SubjectRandomizer(planetNames);
            planetRandomizer.SetAdjectives(planetAdjectives);

            var e = new TextTemplate();
            e.Add("$planet is populated by a million people.");
            e.Substitute("planet", planetRandomizer);

            for (int i = 0; i < 100; i++)
            {
                var output = e.Next();

                for (int j = 0; j < adjectiveAppeared.Length; j++)
                {
                    if(output.Contains(planetAdjectives[j]))
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
            var planetNames = new string[] { "Elnea", "Setaphor Prime", "Gestadomeia" };
            var planetDeterminers = new string[] { "the" };
            bool[] determinersAppeared = new bool[planetDeterminers.Length];

            var planetRandomizer = new SubjectRandomizer(planetNames);
            planetRandomizer.SetDeterminers(planetDeterminers);

            var e = new TextTemplate();
            e.Add("$planet is populated by a million people.");
            e.Substitute("planet", planetRandomizer);

            for (int i = 0; i < 100; i++)
            {
                var output = e.Next();

                for (int j = 0; j < determinersAppeared.Length; j++)
                {
                    if (output.Contains(planetDeterminers[j]))
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

            var g = new NameGenerator("ae", "str");

            var e = new TextTemplate();
            e.Add("$planet");
            e.Substitute("planet", g);

            for (int i = 0; i < 100; i++)
            {
                var output = e.Next();
                Assert.IsTrue(output.Replace("planet", string.Empty).Length > 0);
            }

        }

    }
}
