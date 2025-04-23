using Archigen;
using Loremaker.Text;
using System;
using System.Text.Json.Serialization;

namespace Loremaker
{
    /// <summary>
    /// Represents a faction entity in the Loremaker system.
    /// </summary>
    public class Faction : Entity
    {
        /// <summary>
        /// The name of the faction.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get => this.HasProperty("name") ? this["name"] : null;
            set => this["name"] = value;
        }

        [JsonIgnore]
        public string Description
        {
            get => this.HasProperty("description") ? this["description"] : null;
            set => this["description"] = value;
        }

        /// <summary>
        /// What the members of the faction call themselves
        /// </summary>
        [JsonIgnore]
        public string Denonym
        {
            get => this.HasProperty("denonym") ? this["denonym"] : null;
            set => this["denonym"] = value;
        }

        [JsonIgnore]
        public string DecoratedName
        {
            get => this.HasProperty("decorated-name") ? this["decorated-name"] : null;
            set => this["decorated-name"] = value;
        }

        /// <summary>
        /// Gets or sets the location of the faction.
        /// </summary>
        [JsonIgnore]
        public string LocationId
        {
            get => this.HasProperty("location-id") ? this["location-id"] : null;
            set => this["location-id"] = value;
        }

        /// <summary>
        /// Initializes a new instance of the Faction class.
        /// </summary>
        [JsonConstructor]
        public Faction(string name) : base()
        {
            this.Name = name;
            this.Type = "faction";
        }


    }
}