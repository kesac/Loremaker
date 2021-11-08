using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class VoronoiMapGenerator : IGenerator<VoronoiMap>
    {
        public IGenerator<MapPoint[]> PointsGenerator { get; set; }

        public VoronoiMapGenerator(IGenerator<MapPoint[]> generator)
        {
            this.PointsGenerator = generator;
        }

        public VoronoiMap Next()
        {
            return new VoronoiMap(this.PointsGenerator.Next());
        }

    }
}
