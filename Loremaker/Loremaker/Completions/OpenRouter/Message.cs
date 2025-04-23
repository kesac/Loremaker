using System.Text.Json.Serialization;

namespace Loremaker.Completions.OpenRouter
{
    /// <summary>
    /// Represents a message between the user and system.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Can be one of "user", "assistant", or "system".
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// The text value of this message.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        public Message() { }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        public override string ToString()
        {
            return Content?.ToString();
        }

    }


}
