using Loremaker.Names;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class WorldGenerator
    {

        private ILocationNameGenerator LocationNameGenerator { get; set; }

        public WorldGenerator(ILocationNameGenerator generator)
        {
            this.LocationNameGenerator = generator;
        }

        public World Next()
        {
            var world = new World();
            world.Name = this.LocationNameGenerator.NextWorldName();

            return world;
        }

    }
}
