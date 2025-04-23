using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Loremaker.Completions
{
    /// <summary>
    /// Helps retrieves prompt templates from embedded resources.
    /// </summary>
    public static class Prompts
    {
        private static Assembly _assembly;

        private static void Initialize()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Retrieves a prompt template from an embedded resource.
        /// </summary>
        public static string Get(string promptName)
        {
            if (_assembly == null)
            {
                Initialize();
            }

            if(promptName == null)
            {
                throw new ArgumentNullException(nameof(promptName));
            }

            var resourceName = $"Loremaker.Completions.{promptName.Trim().ToLower()}.prompt";

            using (var stream = _assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

    }
}
