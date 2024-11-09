using Archigen;
using Loremaker.Maps;
using Loremaker.Names;
using Loremaker.Text;
using Syllabore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker
{
    public class WorldGenerator : Generator<World>
    {
        public IGenerator<string> LocationNameGenerator { get; private set; }
        public IGenerator<string> DescriptionGenerator { get; private set; }
        public IGenerator<Map> MapGenerator { get; private set; }
        
        public WorldGenerator() : this(new MapGenerator(1000, 1000, 0.30f)) { }

        public WorldGenerator(IGenerator<Map> mapGenerator)
        {
            this.UsingMapGenerator(mapGenerator);
            this.UsingNameGenerator(new DefaultNameGenerator());
            this.UsingDescriptionGenerator(new GibberishGenerator().UsingSentenceLength(2));

            this.ForEach(x => { this.PostGeneration(x); });
        }

        private void PostGeneration(World world)
        {
            var scanner = new MapScanner(world.Map);
            var landmasses = scanner.FindLandmasses();

            foreach(var mass in landmasses)
            {
                mass.Name = this.LocationNameGenerator.Next();
                world.Landmasses.Add(mass.Id, mass);
            }
                   
            // Todo: Can we remove cast to list?
            var pcg = new PopulationCenterGenerator(world.Landmasses.Values.ToList(), this.LocationNameGenerator);

            foreach(var pc in pcg.Next())
            {
                world.PopulationCenters.Add(pc.Id, pc);
            }

            var tg = new TerritoryGenerator(world, this.LocationNameGenerator);

            foreach(var t in tg.Next())
            {
                world.Territories.Add(t.Id, t);
            }

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

        public WorldGenerator UsingMapGenerator(IGenerator<Map> generator)
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
