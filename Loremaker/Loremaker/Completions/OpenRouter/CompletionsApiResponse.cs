using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loremaker.Completions.OpenRouter
{
    /// <summary>
    /// Contains a response from the Completions API of OpenRouter.
    /// </summary>
    public class CompletionsApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the service provider who owns the model used
        /// to generate this response.
        /// </summary>
        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// The name of the model used to generate this response.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// One of "chat.completion" or "chat.completion.chunk".
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; }

        /// <summary>
        /// The time that this response was created in Unix time format. 
        /// Use <see cref="CreatedDate"/> to get
        /// the equivalent time as a <see cref="DateTime"/> object.
        /// </summary>
        [JsonPropertyName("created")]
        public long Created { get; set; }

        /// <summary>
        /// The time that this response was created.
        /// </summary>
        [JsonIgnore]
        public DateTime CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created).DateTime.ToLocalTime();

        /// <summary>
        /// Choice objects containing content returned from the Completions API.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonIgnore]
        public string CompletionText => Choices[0].Message.Content;

        /// <summary>
        /// Not guaranteed to be populated for all service providers.
        /// </summary>
        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        /// <summary>
        /// Information on how many tokens were consumed
        /// in processing the prompt and generating a completion.
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }

    /// <summary>
    /// Contains the output of a completions request.
    /// This class represents a NonChatChoice, NonStreamingChoice, and StreamingChoice.
    /// </summary>
    public class Choice
    {
        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        // To be implemented:
        // [JsonPropertyName("error")]
        // public ChoiceError Error { get; set; }

    }

    /// <summary>
    /// Contains information on how many tokens were consumed
    /// in processing the prompt and generating a completion.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// Tokens consumed in processing the prompt.
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// Tokens consumed in generating the completion.
        /// </summary>

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// Total tokens consumed in processing the prompt
        /// and generating the completion.
        /// </summary>

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

}