using Archigen;
using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class MapGenerator : Generator<Map>
    {
        public MapGenerator(int width, int height)
        {
            this.ForProperty<double[,]>(x => x.HeightMap, new IslandHeightMapGenerator(3).UsingVarianceDrop(0.4).UsingSize(width, height));
            this.ForProperty<VoronoiMap>(x => x.VoronoiMap, new VoronoiMapGenerator(new PointsGenerator(width, height, 20)));
            this.ForProperty<int>(x => x.Width, width);
            this.ForProperty<int>(x => x.Height, height);
        }
    }
}
