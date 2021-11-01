using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public static class MapExtensions
    {

        public static IPoint Average(this IPoint[] points)
        {
            return new DelaunatorSharp.Point(points.Average(x => x.X), points.Average(x => x.Y));
        }

        public static int AverageX(this IVoronoiCell cell)
        {
            return (int)cell.Points.Average(x => x.X);
        }

        public static int AverageY(this IVoronoiCell cell)
        {
            return (int)cell.Points.Average(x => x.Y);
        }
    }
}
