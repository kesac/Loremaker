using Archigen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Loremaker.Text
{
    public class TextLine
    {
        public string Value { get; set; }
        public string RequiredContext { get; set; }

        public TextLine(string value)
        {
            Value = value;
        }
        public TextLine(string value, string requiredContext)
        {
            Value = value;
            RequiredContext = requiredContext;
        }

        public bool HasRequiredContext()
        {
            return RequiredContext != null;
        }

        public override string ToString()
        {
            return Value;
        }

    }

    public class SimpleGenerator : IGenerator<string>
    {
        private Random _random;
        public List<string> Values { get; set; }

        public SimpleGenerator(List<string> values)
        {
            _random = new Random();
            Values = values;
        }

        public string Next()
        {
            return Values[_random.Next(Values.Count)];
        }
    }

    public class SubjectDto
    {
        [JsonPropertyName("determiners")]
        public List<string> Determiners { get; set; }
        [JsonPropertyName("adjectives")]
        public List<string> Adjectives { get; set; }
        [JsonPropertyName("values")]
        public List<string> Values { get; set; }
    }

    public class SubjectGenerator : IGenerator<string>
    {
        private Random _random;
        public SimpleGenerator Determiners { get; set; }
        public SimpleGenerator Adjectives { get; set; }
        public SimpleGenerator Values { get; set; }

        public SubjectGenerator(List<string> values)
        {
            _random = new Random();
            Values = new SimpleGenerator(values);
        }

        public void SetDeterminers(List<string> determiners)
        {
            Determiners = new SimpleGenerator(determiners);
        }

        public void SetAdjectives(List<string> adjectives)
        {
            Adjectives = new SimpleGenerator(adjectives);
        }

        public string Next()
        {
            var result = Values.Next();

            if (Adjectives != null)
            {
                result = Adjectives.Next() + " " + result;
            }

            if (Determiners != null)
            {
                result = Determiners.Next() + " " + result;
            }

            return result;
        }
    }

    public class TextTemplate : IGenerator<string>
    {
        public List<TextLine> Lines { get; set; }
        public Dictionary<string, IGenerator<string>> Substitutions { get; set; }
        public List<string> Required { get; set; }

        public TextTemplate() 
        {
            Lines = new List<TextLine>();
            Substitutions = new Dictionary<string, IGenerator<string>>();
            Required = new List<string>();
        }

        public string Next()
        {
            foreach (var requirement in Required)
            {
                if (!Substitutions.ContainsKey(requirement))
                {
                    throw new InvalidOperationException("Cannot generate text for this template without substitution '" + requirement + "' defined");
                }
            }

            var finalizedSubstitutions = new Dictionary<string, string>();

            foreach (var substitution in Substitutions)
            {
                finalizedSubstitutions[substitution.Key] = substitution.Value.Next();
            }

            var result = new StringBuilder();

            foreach (var line in Lines)
            {
                if (!line.HasRequiredContext() 
                    || (line.HasRequiredContext() && result.ToString().Contains(line.RequiredContext)))
                {
                    var processedLine = line.Value;

                    foreach (var finalizedSubstitution in finalizedSubstitutions)
                    {
                        if (processedLine.Contains("$" + finalizedSubstitution.Key))
                        {
                            processedLine = processedLine.Replace("$" + finalizedSubstitution.Key, finalizedSubstitution.Value);
                        }
                    }

                    result.AppendLine(processedLine);

                }
            }

            return result.ToString();
        }

        public static TextTemplate LoadFromFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("No file path was specified.");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path + " does not exist.");
            }

            var result = new TextTemplate();

            var allText = File.ReadAllText(path);

            // Regex breakdown:
            //    ^\s*        Match the start of the line, followed by any amount of whitespace
            //    (add|when)  Match either "add" or "when", case insensitive
            //    .*          Match all remaining characters on the line, if there are any
            var rawLines = Regex.Matches(allText, @"^\s*(add|when).*", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (Match rawLine in rawLines)
            {
                var tokens = GetTokens(rawLine.Value.Trim());

                // Format: add "text"
                if (tokens.Count >= 2 
                    && String.Equals("add", tokens[0], StringComparison.OrdinalIgnoreCase))
                {
                    result.Lines.Add(new TextLine(tokens[1]));
                }
                // Format: when "condition" add "text"
                else if (tokens.Count >= 4 
                    && String.Equals("when", tokens[0], StringComparison.OrdinalIgnoreCase)
                    && String.Equals("add", tokens[2], StringComparison.OrdinalIgnoreCase))
                {
                    result.Lines.Add(new TextLine(tokens[3], tokens[1]));
                }
            }

            // Regex breakdown:
            //    ^\s*                 Match the start of the line, followed by any amount of whitespace
            //    \$[a-zA-Z0-9_]+      Match an alphanumeric variable name that starts with a dollar sign
            //    \s*=\s*              Match an equals sign, surrounded by any amount of whitespace
            //    \[.*?\]\s*$          After the equals sign, match everything between opening and closing brackets. The closing bracket must end a line
            //    |\{.*?\}\s*$         Or match anything between opening and closing curly brackets. The closing brackets must end a line
            //    |@[a-zA-Z0-9_]+\s*$  Or match an "@" sign followed by any alphanumeric characters
            var rawSubstitutions = Regex.Matches(
                allText,
                @"^\s*\$[a-zA-Z0-9_]+\s*=\s*(\[.*?\]\s*$|\{.*?\}\s*$|@[a-zA-Z0-9_]+?\s*$)",
                RegexOptions.Multiline | RegexOptions.Singleline
            );

            foreach (Match rawSubstitution in rawSubstitutions)
            {
                var tokens = Regex.Split(rawSubstitution.Value, @"\s*=\s*", RegexOptions.None);

                var key = tokens[0].Trim().Substring(1); // removes the leading "$"

                if (tokens[1].Trim().StartsWith("["))
                {
                    var data = JsonSerializer.Deserialize<List<string>>(tokens[1]);
                    result.Substitutions[key] = new SimpleGenerator(data);
                }
                else if (tokens[1].Trim().StartsWith("{"))
                {
                    var data = JsonSerializer.Deserialize<SubjectDto>(tokens[1]);
                    var generator = new SubjectGenerator(data.Values);
                    generator.SetDeterminers(data.Determiners);
                    generator.SetAdjectives(data.Adjectives);
                    result.Substitutions[key] = generator;
                }
                else if (string.Equals(tokens[1].Trim(), "@input", StringComparison.OrdinalIgnoreCase))
                { 
                    result.Required.Add(key);
                }

            }

            return result;
        }

        private static List<string> GetTokens(string line)
        {
            var result = new List<string>();

            // Using regex, split the line into individual words, but ensure
            // everything between quotes is treated as a single word.
            var matches = Regex.Matches(line, "\"[^\"]*\"|[^ ]+");

            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var value = match.ToString();

                    if(value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        result.Add(value.Substring(1, value.Length - 2));
                    }
                    else
                    {
                        result.Add(value);
                    }

                }
            }
            else
            {
                result.Add(line);
            }

            return result;
        }

    }
}
