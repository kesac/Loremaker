using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loremaker
{
    /// <summary>
    /// A general-purpose entity with dynamic properties.
    /// </summary>
    public class Entity : IEntity
    {
        public uint Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the faction.
        /// </summary>
        [JsonIgnore]
        public string Type
        {
            get => this.HasProperty("type") ? this["type"] : null;
            set => this["type"] = value;
        }

        public Dictionary<string, string> Properties { get; set; }

        public Entity()
        {
            // Order matters
            this.Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.Type = "entity";
        }

        /// <summary>
        /// Gets or sets a property value by name. All property names are case-insensitive.
        /// </summary>
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (!this.Properties.ContainsKey(propertyName))
                {
                    throw new KeyNotFoundException($"Property '{propertyName}' does not exist.");
                }

                return this.Properties[propertyName];
            }
            set
            {
                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                this.Properties[propertyName] = value;
            }
        }

        public void AddProperty(string propertyName, object value)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            this.Properties[propertyName] = value?.ToString();
        }

        public void SetProperty(string propertyName, object value)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (!this.Properties.ContainsKey(propertyName))
            {
                throw new KeyNotFoundException($"Property '{propertyName}' does not exist.");
            }

            this.Properties[propertyName] = value?.ToString();
        }

        public string GetProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (!this.Properties.ContainsKey(propertyName))
            {
                throw new KeyNotFoundException($"Property '{propertyName}' does not exist.");
            }

            return this.Properties[propertyName];
        }

        public T GetProperty<T>(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            string value = this.GetProperty(propertyName);

            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)value;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException)
            {
                throw new InvalidCastException($"Cannot convert property '{propertyName}' value '{value}' to type {typeof(T).Name}", ex);
            }
        }

        public bool RemoveProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return this.Properties.Remove(propertyName);
        }

        public bool HasProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return this.Properties.ContainsKey(propertyName);
        }

        public IEnumerable<string> GetPropertyNames()
        {
            return this.Properties.Keys;
        }
    }
}
