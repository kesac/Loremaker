using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class VoronoiMap : Delaunator
    {
        public VoronoiMap(MapPoint[] points) 
            : base(points.Select(point => (DelaunatorSharp.IPoint)new DelaunatorSharp.Point(point.X, point.Y)).ToArray()) 
        { 

        }
    }
}
