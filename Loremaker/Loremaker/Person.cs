using System;
using System.Text.Json.Serialization;

namespace Loremaker
{
    /// <summary>
    /// Represents a person.
    /// </summary>
    public class Person : Entity
    {
        /// <summary>
        /// Initializes a new person.
        /// </summary>
        public Person() : base()
        {
            this.Type = "person";
        }

        /// <summary>
        /// Gets or sets the given name (first name) of the person.
        /// </summary>
        [JsonIgnore]
        public string GivenName
        {
            get => this.HasProperty("given-name") ? this["given-name"] : null;
            set => this["given-name"] = value;
        }

        /// <summary>
        /// Gets or sets the family name (last name) of the person.
        /// </summary>
        [JsonIgnore]
        public string FamilyName
        {
            get => this.HasProperty("family-name") ? this["family-name"] : null;
            set => this["family-name"] = value;
        }

        /// <summary>
        /// Gets or sets the ID of the faction thhis person belongs to.
        /// </summary>
        [JsonIgnore]
        public string FactionId
        {
            get => this.HasProperty("faction-id") ? this["faction-id"] : null;
            set => this["faction-id"] = value;
        }

        /// <summary>
        /// Gets or sets the age of this person.
        /// </summary>
        [JsonIgnore]
        public int Age
        {
            get => this.HasProperty("age") ? this.GetProperty<int>("age") : 0;
            set => this["age"] = value.ToString();
        }
    }
}