using Archigen;
using Loremaker.Maps;
using Loremaker.Names;
using Loremaker.Text;
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
        public int WorldMapWidth { get; private set; }
        public int WorldMapHeight { get; private set; }
        public float WorldMapLandThreshold { get; set; }

        public WorldGenerator(int width = 1000, int height = 1000, float landThreshold = 0.30f)
        {
            this.LocationNameGenerator = new DefaultNameGenerator();
            this.DescriptionGenerator = new GibberishTextGenerator().UsingSentenceLength(2);
            
            this.WorldMapWidth = width;
            this.WorldMapHeight = height;
            this.WorldMapLandThreshold = landThreshold;

            this.MapGenerator = new MapGenerator(width, height, 0.30f);

            this.RefreshProperties();
        }

        private void RefreshProperties()
        {
            this.ForProperty<Map>(x => x.Map, this.MapGenerator);
            this.ForProperty<string>(x => x.Name, this.LocationNameGenerator);
            this.ForProperty<string>(x => x.Description, this.DescriptionGenerator);
            this.ForEach(x =>
            {
                var scanner = new MapScanner(x.Map);
                x.Map.Landmasses = scanner.FindLandmasses();

                foreach(var c in x.Map.Landmasses)
                {
                    c.Name = this.LocationNameGenerator.Next();
                }

                var pcg = new PopulationCenterGenerator(x.Map.Landmasses, this.LocationNameGenerator);
                x.PopulationCenters.AddRange(pcg.Next());

            });
        }

        public WorldGenerator UsingNameGenerator(IGenerator<string> generator)
        {
            this.LocationNameGenerator = generator;
            this.RefreshProperties();
            return this;
        }

        public WorldGenerator UsingMapGenerator(MapGenerator generator)
        {
            this.MapGenerator = generator
                                .UsingDimension(this.WorldMapWidth, this.WorldMapHeight)
                                .UsingLandThreshold(this.WorldMapLandThreshold);

            this.RefreshProperties();
            return this;
        }

    }
}
