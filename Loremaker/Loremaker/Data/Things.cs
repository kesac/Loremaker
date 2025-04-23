using Archigen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker.Data
{
    /// <summary>
    /// Gives access to generators that select from embedded lists of "things".
    /// </summary>
    public static class Things
    {
        private static Assembly _assembly;
        private static IGenerator<string> _colorGenerator;
        private static IGenerator<string> _objectsGenerator;
        private static IGenerator<string> _materialsGenerator;
        private static IGenerator<string> _conceptsGenerator;
        private static IGenerator<string> _itemDescriptionTemplateGenerator;

        private static void Initialize()
        {
            if (_assembly == null)
            {
                _assembly = Assembly.GetExecutingAssembly();
            }
        }

        private static IGenerator<string> GetDefaultListGenerator(string name)
        {
            if (_assembly == null)
            {
                Initialize();
            }

            var resourceName = $"Loremaker.Data.{name}.json";
            using (var stream = _assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Could not find resource: {resourceName}");
                }
                using (var reader = new System.IO.StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var items = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                    return new RandomSelector<string>(items);
                }
            }
        }

        public static IGenerator<string> GetDefaultObjectsGenerator()
        {
            if (_objectsGenerator == null)
            {
                _objectsGenerator = GetDefaultListGenerator("objects");
            }

            return _objectsGenerator;
        }

        public static IGenerator<string> GetDefaultItemDescriptionTemplateGenerator()
        {
            if (_objectsGenerator == null)
            {
                _itemDescriptionTemplateGenerator = GetDefaultListGenerator("object-descriptions");
            }

            return _itemDescriptionTemplateGenerator;
        }

        public static IGenerator<string> GetDefaultMaterialsGenerator()
        {
            if (_materialsGenerator == null)
            {
                _materialsGenerator = GetDefaultListGenerator("materials");
            }

            return _materialsGenerator;
        }

        public static IGenerator<string> GetDefaultConceptsGenerator()
        {
            if (_conceptsGenerator == null)
            {
                _conceptsGenerator = GetDefaultListGenerator("concepts");
            }

            return _conceptsGenerator;
        }

        /// <summary>
        /// Gets a no-frills, no-context color generator.
        /// </summary>
        public static IGenerator<string> GetDefaultColorGenerator()
        {
            if (_colorGenerator == null)
            {
                Initialize();

                using (var stream = _assembly.GetManifestResourceStream("Loremaker.Data.colors.json"))
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException("Could not find resource: Loremaker.Data.colors.json");
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
