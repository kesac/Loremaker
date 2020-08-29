using Loremaker.Maps;
using Loremaker.Names;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class WorldGenerator
    {

        public ILocationNameGenerator LocationNameGenerator { get; private set; }
        public IHeightMapGenerator HeightMapGenerator { get; private set; }

        public int MinimumContinents { get; private set; }
        public int MaximumContinents { get; private set; }

        private Random _random;

        public WorldGenerator()
        {
            this.LocationNameGenerator = new DefaultNameGenerator();
            this.HeightMapGenerator = new DefaultHeightMapGenerator();
            _random = new Random();
        }

        public WorldGenerator SetNameGenerator(ILocationNameGenerator generator)
        {
            this.LocationNameGenerator = generator;
            return this;
        }

        public WorldGenerator SetHeightMapGenerator(IHeightMapGenerator generator)
        {
            this.HeightMapGenerator = generator;
            return this;
        }

        public WorldGenerator SetTotalContinents(int total)
        {
            return this.SetTotalContinents(total, total);
        }

        public WorldGenerator SetTotalContinents(int min, int max)
        {

            if(min < 1)
            {
                throw new ArgumentException("There must be at least one continent in the world.");
            }
            else if (min > max)
            {
                throw new ArgumentException("The desired minimum number of continents cannot exceed the desired maximum.");
            }

            this.MinimumContinents = min;
            this.MaximumContinents = max;
            return this;
        }

        public World Next()
        {
            var world = new World();
            world.Name = this.LocationNameGenerator.NextWorldName();

            var totalContinents = this.MinimumContinents + _random.Next(this.MaximumContinents - this.MinimumContinents);

            for (int i = 0; i < totalContinents; i++) {

                world.Continents.Add(new Continent()
                {
                    Name = this.LocationNameGenerator.NextContinentName(),
                    HeightMap = this.HeightMapGenerator.Next(1000, 1000)
                });
            }

            return world;
        }

    }
}
