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
        public HeightMapGenerator HeightMapGenerator { get; private set; }

        public int HeightMapWidth { get; set; }
        public int HeightMapHeight { get; set; }

        public int MinimumContinents { get; private set; }
        public int MaximumContinents { get; private set; }

        public WorldGenerator()
        {
            this.LocationNameGenerator = new DefaultNameGenerator();
            this.DescriptionGenerator = new GibberishTextGenerator().UsingSentenceLength(2);
            this.HeightMapGenerator = new HeightMapGenerator();

            this.HeightMapWidth = 1001;
            this.HeightMapHeight = 1001;

            this.RefreshWorldsProperty();
            this.RefreshContinentsProperty();
        }

        private void RefreshWorldsProperty()
        {
            this.ForProperty<string>(x => x.Name, this.LocationNameGenerator)
                .ForProperty<string>(x => x.Description, this.DescriptionGenerator);
        }

        private void RefreshContinentsProperty()
        {
            this.ForListProperty<Continent>(x => x.Continents, new Generator<Continent>()
                    .ForProperty<string>(x => x.Name, this.LocationNameGenerator)
                    .ForProperty<double[,]>(x => x.HeightMap, this.HeightMapGenerator
                        .UsingSize(this.HeightMapWidth, this.HeightMapHeight)))
                .UsingSize(5);
        }

        public WorldGenerator UsingNameGenerator(IGenerator<string> generator)
        {
            this.LocationNameGenerator = generator;
            this.RefreshWorldsProperty();
            this.RefreshContinentsProperty();
            return this;
        }

        public WorldGenerator UsingHeightMapGenerator(HeightMapGenerator generator)
        {
            this.HeightMapGenerator = generator;
            this.RefreshContinentsProperty();
            return this;
        }

    }
}
