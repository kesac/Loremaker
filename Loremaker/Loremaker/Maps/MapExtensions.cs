using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public static class MapExtensions
    {
        /*
        public static List<MapPoint> ToMapPoints(this DelaunatorSharp.IPoint[] points)
        {
            return points.Select(point => new MapPoint((int)point.X, (int)point.Y)).ToList();
        }
        */

        public static MapPoint Average(this List<MapPoint> points)
        {
            return new MapPoint((int)points.Average(p => p.X), (int)points.Average(p => p.Y));
        }

        public static DelaunatorSharp.IPoint Average(this DelaunatorSharp.IPoint[] points)
        {
            return new DelaunatorSharp.Point(points.Average(x => x.X), points.Average(x => x.Y));
        }



    }
}
