using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public class Culture : Entity
    {

        [JsonIgnore]
        public string Name
        {
            get => this.HasProperty("name") ? this["name"] : null;
            set => this["name"] = value;
        }

        [JsonIgnore]
        public string Denonym
        {
            get => this.HasProperty("denonym") ? this["denonym"] : null;
            set => this["denonym"] = value;
        }

        public Culture(string name, string denonym)
        {
            this.Name = name;
            this.Denonym = denonym;
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Denonym}s)";
        }

    }
}
