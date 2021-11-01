using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class VoronoiMap : Delaunator
    {
        public VoronoiMap(IPoint[] points) : base(points) { }
    }
}
