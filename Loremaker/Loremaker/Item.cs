using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    /// <summary>
    /// Represents a fictional item in the world that can be used by fictional people.
    /// </summary>
    public class Item : Entity
    {
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

        public Item()
        {
            this.Type = "item";
            this.Name = string.Empty;
            this.Description = string.Empty;
        }
    }
}
