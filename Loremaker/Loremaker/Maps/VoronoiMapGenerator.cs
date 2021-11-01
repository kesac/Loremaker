using Archigen;
using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class VoronoiMapGenerator : IGenerator<VoronoiMap>
    {
        public IGenerator<IPoint[]> PointsGenerator { get; set; }

        public VoronoiMapGenerator(IGenerator<IPoint[]> generator)
        {
            this.PointsGenerator = generator;
        }

        public VoronoiMap Next()
        {
            var points = this.PointsGenerator.Next();
            var d = new VoronoiMap(points);

            return d;
        }

    }
}
