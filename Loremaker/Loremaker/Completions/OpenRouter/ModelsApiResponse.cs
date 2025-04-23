using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Loremaker.Completions.OpenRouter
{
    /// <summary>
    /// Contains a response from the Models API of OpenRouter.
    /// </summary>
    public class ModelsApiResponse
    {
        [JsonPropertyName("data")]
        public List<ModelInformation> Models { get; set; }

        public ModelsApiResponse()
        {
            Models = new List<ModelInformation>();
        }
    }

    /// <summary>
    /// Data returned by the Models API.
    /// </summary>
    public class ModelInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created).DateTime.ToLocalTime();

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("context_length")]
        public int ContextLength { get; set; }

        [JsonPropertyName("architecture")]
        public ModelArchitecture Architecture { get; set; }

        [JsonPropertyName("pricing")]
        public ModelPricing Pricing { get; set; }

        [JsonPropertyName("top_provider")]
        public ModelTopProvider TopProvider { get; set; }

        [JsonPropertyName("per_request_limits")]
        public string PerRequestLimits { get; set; } // Unsure on data type
    }

    public class ModelTopProvider
    {
        [JsonPropertyName("context_length")]
        public int? ContextLength { get; set; }

        [JsonPropertyName("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }

        [JsonPropertyName("is_moderated")]
        public bool IsModerated { get; set; }
    }

    public class ModelPricing
    {
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        [JsonPropertyName("completion")]
        public string Completion { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("request")]
        public string Request { get; set; }
    }

    public class ModelArchitecture
    {
        [JsonPropertyName("modality")]
        public string Modality { get; set; }

        [JsonPropertyName("tokenizer")]
        public string Tokenizer { get; set; }

        [JsonPropertyName("instruct_type")]
        public string InstructType { get; set; }
    }

}
