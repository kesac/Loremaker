using System;
using System.Collections.Generic;
using System.Linq;

namespace Loremaker
{
    public class Codex
    {
        public Dictionary<uint, Entity> Entities { get; set; }
        public Dictionary<uint, Event> Events { get; set; }
        public uint NextId { get; set; }

        public Codex()
        {
            this.Entities = new Dictionary<uint, Entity>();
            this.Events = new Dictionary<uint, Event>();
            this.NextId = 1;
        }

        public void Register(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Assign a unique ID if the entity doesn't have one already or has ID 0
            if (entity.Id == 0)
            {
                entity.Id = this.GetNextAvailableId();
            }
            else if (this.Entities.ContainsKey(entity.Id))
            {
                throw new InvalidOperationException($"An entity with ID {entity.Id} is already registered.");
            }

            this.Entities[entity.Id] = entity;

            // If it's an Event, also register in the Events dictionary
            if (entity is Event eventEntity)
            {
                this.Events[entity.Id] = eventEntity;
            }
        }
        public Event Find(uint id)
        {
            return this.Find<Event>(id);
        }

        public T Find<T>(uint id) where T : Event
        {
            if (this.Events.TryGetValue(id, out Event eventEntity))
            {
                if (eventEntity is T typedEvent)
                {
                    return typedEvent;
                }
            }

            return null;
        }

        public bool Remove(uint id)
        {
            bool removed = this.Entities.Remove(id);

            // Also remove from Events if it exists there
            this.Events.Remove(id);

            return removed;
        }

        public bool Remove(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return this.Remove(entity.Id);
        }

        private uint GetNextAvailableId()
        {
            // Find the next available ID
            while (this.Entities.ContainsKey(this.NextId))
            {
                this.NextId++;

                // Guard against overflow
                if (this.NextId == 0)
                {
                    throw new InvalidOperationException("No more IDs available.");
                }
            }

            return this.NextId++;
        }

        public IEnumerable<Entity> GetEntities()
        {
            return this.Entities.Values;
        }

        public IEnumerable<Event> GetEvents()
        {
            return this.Events.Values;
        }

        public IEnumerable<Event> GetEventsChronologically()
        {
            return this.Events.Values.OrderBy(e => e.Year);
        }
    }
}
