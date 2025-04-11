using System.Text.Json.Serialization;

namespace Loremaker
{
    /// <summary>
    /// Represents a location in the world.
    /// </summary>
    public class Location : Entity
    {
        /// <summary>
        /// Initializes a new location.
        /// </summary>
        public Location() : base()
        {
            this.Type = "faction"; // Set type to "faction" as requested
        }

        /// <summary>
        /// The type of the location (eg. city, village, town).
        /// </summary>
        [JsonIgnore]
        public string LocationType
        {
            get => this.HasProperty("location-type") ? this["location-type"] : null;
            set => this["location-type"] = value;
        }

        /// <summary>
        /// The name of the location.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get => this.HasProperty("name") ? this["name"] : null;
            set => this["name"] = value;
        }
    }
}