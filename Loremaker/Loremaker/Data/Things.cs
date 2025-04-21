using Archigen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker.Data
{
    public class Things
    {
        private static Assembly _assembly;
        private static IGenerator<string> _colorGenerator;

        private static void Initialize()
        {
            if (_assembly == null)
            {
                _assembly = Assembly.GetExecutingAssembly();
            }
        }

        /// <summary>
        /// Gets a no-frills, no-context color generator.
        /// </summary>
        public static IGenerator<string> GetDefaultColorGenerator()
        {
            if (_colorGenerator == null)
            {
                Initialize();

                using (var stream = _assembly.GetManifestResourceStream("Loremaker.Data.Colors.json"))
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException("Could not find resource: Loremaker.Data.Colors.json");
                    }

                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        var colorData = System.Text.Json.JsonSerializer.Deserialize<List<ColorInfo>>(json);

                        var allColorVariations = new List<string>();
                        foreach (var data in colorData)
                        {
                            allColorVariations.Add(data.Name);
                            allColorVariations.AddRange(data.Variations);
                        }

                        _colorGenerator = new RandomSelector<string>(allColorVariations);
                    }
                }
            }

            return _colorGenerator;
        }

        // Class to match the JSON structure
        private class ColorInfo
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("variations")]
            public List<string> Variations { get; set; }
        }

    }
}
