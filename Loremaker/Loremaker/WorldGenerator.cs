using Archigen;
using Loremaker.Maps;
using Loremaker.Names;
using Loremaker.Text;
using Syllabore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class WorldGenerator : Generator<World>
    {
        public IGenerator<string> LocationNameGenerator { get; private set; }
        public IGenerator<string> DescriptionGenerator { get; private set; }
        public MapGenerator MapGenerator { get; private set; }
        
        public WorldGenerator() : this(new MapGenerator(1000, 1000, 0.30f)) { }

        public WorldGenerator(MapGenerator mapGenerator)
        {
            this.LocationNameGenerator = new DefaultNameGenerator();
            this.DescriptionGenerator = new GibberishTextGenerator().UsingSentenceLength(2);            
            this.MapGenerator = mapGenerator;

            this.ForProperty<Map>(x => x.Map, this.MapGenerator);
            this.ForProperty<string>(x => x.Name, this.LocationNameGenerator);
            this.ForProperty<string>(x => x.Description, this.DescriptionGenerator);
            this.ForEach(x => { this.PostGeneration(x); });
        }

        private void PostGeneration(World world)
        {
            var scanner = new MapScanner(world.Map);
            world.Map.Landmasses = scanner.FindLandmasses();

            foreach (var c in world.Map.Landmasses)
            {
                c.Name = this.LocationNameGenerator.Next();
                world.Map.LandmassesById[c.Id] = c;
            }

            var pcg = new PopulationCenterGenerator(world.Map.Landmasses, this.LocationNameGenerator);
            world.PopulationCenters.AddRange(pcg.Next());

            var tg = new TerritoryGenerator(world);
            world.Territories.AddRange(tg.Next());
        }

        public WorldGenerator UsingNameGenerator(IGenerator<string> generator)
        {
            this.LocationNameGenerator = generator;
            this.ForProperty<string>(x => x.Name, this.LocationNameGenerator);
            return this;
        }

        public WorldGenerator UsingNameGenerator(Func<NameGenerator, NameGenerator> config)
        {
            this.LocationNameGenerator = config(new NameGenerator());
            this.ForProperty<string>(x => x.Name, this.LocationNameGenerator);
            return this;
        }

        public WorldGenerator UsingDescriptionGenerator(IGenerator<string> generator)
        {
            this.DescriptionGenerator = generator;
            this.ForProperty<string>(x => x.Description, this.DescriptionGenerator);
            return this;
        }

        public WorldGenerator UsingMapGenerator(MapGenerator generator)
        {
            this.MapGenerator = generator;
            this.ForProperty<Map>(x => x.Map, this.MapGenerator);
            return this;
        }

        public WorldGenerator UsingMapGenerator(Func<MapGenerator,MapGenerator> config)
        {
            this.MapGenerator = config(new MapGenerator());
            this.ForProperty<Map>(x => x.Map, this.MapGenerator);
            return this;
        }


    }
}
