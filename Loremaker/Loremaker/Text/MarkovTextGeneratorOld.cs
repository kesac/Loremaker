using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Markov;

namespace Loremaker.Text
{
    public class MarkovTextGeneratorOld : ITextGeneratorOld
    {
        public int Depth { get; set; }
        public char Delimiter { get; set; }
        public List<string> CorpusFilepaths { get; set; }
        public List<string> StartingWords { get; set; }
        public string EndingWord { get; set; }
        private MarkovChain<string> MarkovChain { get; set; }

        public MarkovTextGeneratorOld()
        {
            this.Delimiter = ' ';
            this.Depth = 2;
            this.StartingWords = new List<string>();
            this.CorpusFilepaths = new List<string>();
        }

        public MarkovTextGeneratorOld UsingDepth(int depth)
        {
            this.Depth = depth;
            return this;
        }

        public MarkovTextGeneratorOld UsingDelimiter(char c)
        {
            this.Delimiter = c;
            return this;
        }

        public MarkovTextGeneratorOld FromCorpus(params string[] filepaths)
        {
            foreach(var filepath in filepaths)
            {
                this.CorpusFilepaths.Add(filepath);
            }
            
            return this;
        }

        public MarkovTextGeneratorOld LoadCorpus()
        {
            foreach (var filepath in this.CorpusFilepaths)
            {
                string[] lines = File.ReadAllLines(filepath);
                this.MarkovChain = new MarkovChain<string>(this.Depth);

                foreach (var line in lines)
                {
                    this.MarkovChain.Add(line.Split(this.Delimiter));
                }
            }

            return this;
        }

        public MarkovTextGeneratorOld BeginTextWith(params string[] words)
        {
            foreach(var w in words)
            {
                foreach(var s in w.Split(this.Delimiter))
                {
                    this.StartingWords.Add(s);
                }
            }
            
            return this;
        }

        public MarkovTextGeneratorOld EndTextWith(string substring)
        {
            this.EndingWord = substring;
            return this;
        }

        public string Next()
        {

            if(this.MarkovChain == null)
            {
                this.LoadCorpus();
            }

            var result = new StringBuilder();

            if (this.StartingWords != null && this.StartingWords.Count > 0)
            {
                result.Append(string.Join(this.Delimiter.ToString(), this.MarkovChain.Chain(this.StartingWords)));
                result.Insert(0, string.Format("{0}{1}", string.Join(this.Delimiter.ToString(), this.StartingWords), this.Delimiter.ToString()));
            }
            else
            {
                result.Append(string.Join(this.Delimiter.ToString(), this.MarkovChain.Chain()));
            }

            if(!string.IsNullOrEmpty(this.EndingWord))
            {
                int index = result.ToString().IndexOf(this.Delimiter + this.EndingWord);
                if(index >= 0)
                {
                    result.Length = index; // cut everything after the index
                    result.Append(this.Delimiter + this.EndingWord);
                }
                else
                {
                    result.Append(this.Delimiter + this.EndingWord);
                }
            }

            return result.ToString();

        }

        public TextOutputOld NextOutput()
        {
            return new TextOutputOld(this.Next());
        }
    }
}
