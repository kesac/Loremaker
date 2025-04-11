using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loremaker
{
    public class Event : Entity
    {
        [JsonIgnore]
        public int Year
        {
            get
            {
                if (this.HasProperty("year"))
                {
                    return this.GetProperty<int>("year");
                }
                return 0;
            }
            set
            {
                this.AddProperty("year", value);
            }
        }

        [JsonIgnore]
        public string Name
        {
            get
            {
                if (this.HasProperty("name"))
                {
                    return this.GetProperty("name");
                }
                return string.Empty;
            }
            set
            {
                this.AddProperty("name", value);
            }
        }

        [JsonIgnore]
        public string Description
        {
            get
            {
                if (this.HasProperty("description"))
                {
                    return this.GetProperty("description");
                }
                return string.Empty;
            }
            set
            {
                this.AddProperty("description", value);
            }
        }

        [JsonIgnore]
        public string EventType
        {
            get
            {
                if (this.HasProperty("event-type"))
                {
                    return this.GetProperty("event-type");
                }
                return string.Empty;
            }
            set
            {
                this.AddProperty("event-type", value);
            }
        }

        [JsonIgnore]
        public string EventClassification
        {
            get
            {
                if (this.HasProperty("event-classification"))
                {
                    return this.GetProperty("event-classification");
                }
                return string.Empty;
            }
            set
            {
                this.AddProperty("event-classification", value);
            }
        }

        [JsonConstructor]
        public Event(int year, string name) : base()
        {
            this.Type = "event";
            this.Year = year;
            this.Name = name;
        }

        public override string ToString()
        {
            return $"{this.Year}: {this.Name} - {this.Description}";
        }
    }
}
