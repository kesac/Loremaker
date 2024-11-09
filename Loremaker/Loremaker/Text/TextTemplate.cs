﻿using Archigen;
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

    /// <summary>
    /// A type of random generator that generates text
    /// from a template with substitution rules.
    /// </summary>
    public class TextTemplate : IGenerator<string>
    {
        /// <summary>
        /// Used during .lore file processing. Subject data
        /// is stored in a JSON format and this class is used
        /// as part of the deserialization process. Not meant
        /// for use outside of  <see cref="TextTemplate"/>.
        /// </summary>
        private class IntermediarySubjectData
        {
            [JsonPropertyName("determiners")]
            public List<string> Determiners { get; set; }
            [JsonPropertyName("adjectives")]
            public List<string> Adjectives { get; set; }
            [JsonPropertyName("values")]
            public List<string> Values { get; set; }
        }

        /// <summary>
        /// The lines of text that will be used to
        /// generate text during calls to <see cref="Next()"/>.
        /// The order of lines matter.
        /// </summary>
        public List<TextTemplateLine> Lines { get; set; }

        /// <summary>
        /// The substitution rules that will be used to generate 
        /// text during calls to <see cref="Next()"/>.
        /// </summary>
        public Dictionary<string, IGenerator<string>> Substitutions { get; set; }

        /// <summary>
        /// A list of substitution keys that are required to be defined 
        /// before a call to <see cref="Next()"/> can be made. 
        /// (<see cref="Substitutions"/> must contain the key.)
        /// </summary>
        public List<string> Required { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="TextTemplate"/>.
        /// </summary>
        public TextTemplate() 
        {
            Lines = new List<TextTemplateLine>();
            Substitutions = new Dictionary<string, IGenerator<string>>();
            Required = new List<string>();
        }

        /// <summary>
        /// <para>
        ///    Generates text based on the template and substitution rules.
        /// </para>
        /// <para>
        ///    To build the output, each line in the template has
        ///    the substitution rules applied to it individually. If a line has 
        ///    a context requirement, it will only be added to the output if any
        ///    previous line contains the context.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public string Next()
        {
            foreach (var requirement in Required)
            {
                if (!Substitutions.ContainsKey(requirement))
                {
                    throw new InvalidOperationException("Cannot generate text for this template without substitution '" + requirement + "' defined");
                }
            }

            // Keep substitution values consistent for multiple instances
            // of the same substitution. This could become toggleable in the future.
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

        /// <summary>
        /// Reads and parses a .lore file from the specified path 
        /// and returns a <see cref="TextTemplate"/> based on its contents.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static TextTemplate LoadFromFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("No file path was specified.");
            }
            else if (!File.Exists(path))
            {
                throw new FileNotFoundException(path + " does not exist.");
            }

            var result = new TextTemplate();
            var rawText = File.ReadAllText(path);
            var rawLines = Regex.Matches(rawText, @"^\s*(add|when).*", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (Match rawLine in rawLines)
            {
                var tokens = rawLine.Value.Trim().Tokenize();

                if (tokens.Count >= 2 && tokens[0].CaseInsensitiveEquals("add"))
                {
                    result.Lines.Add(new TextTemplateLine(tokens[1])); // "ADD <text>"
                }
                else if (tokens.Count >= 4 && tokens[0].CaseInsensitiveEquals("when") && tokens[2].CaseInsensitiveEquals("add"))
                {
                    result.Lines.Add(new TextTemplateLine(tokens[3], tokens[1])); // "WHEN <condition> ADD <text>"
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
                rawText,
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
                    result.Substitutions[key] = new Randomizer<string>(data);
                }
                else if (tokens[1].Trim().StartsWith("{"))
                {
                    var data = JsonSerializer.Deserialize<IntermediarySubjectData>(tokens[1]);
                    var generator = new SubjectRandomizer(data.Values);
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

    }
}
