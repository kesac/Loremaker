using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Loremaker.Completions.OpenRouter
{
    /// <summary>
    /// Represents a request body meant to be 
    /// sent to the Models API of OpenRouter.
    /// </summary>
    public class CompletionsApiRequest
    {
        /// <summary>
        /// The name of the model that this API request 
        /// is directed at.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The set of messages that the Completions API 
        /// needs to complete.
        /// </summary>

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        /// <summary>
        /// Instantiates a request body for the specified
        /// model.
        /// </summary>
        public CompletionsApiRequest(string modelName)
        {
            Model = modelName;
            Messages = new List<Message>();
        }

        /// <summary>
        /// Convenience method to add a message that
        /// the Completions API needs to complete.
        /// </summary>
        public void AddMessage(string role, string text)
        {
            Messages.Add(new Message(role, text));
        }

        /// <summary>
        /// Converts this object into JSON text.
        /// </summary>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };

            return JsonSerializer.Serialize(this, options);
        }

    }

}
