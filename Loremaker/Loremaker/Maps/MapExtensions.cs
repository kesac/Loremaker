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


        public static double Distance(this MapPoint p1, MapPoint p2)
        {
            float x = p1.X - p2.X;
            float y = p1.Y - p2.Y;

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }



        /// <summary>
        /// <para>
        /// Given two groups A and B of map cells, this method returns the closest distance
        /// between the center of any two cells P1 and P2, where P1 is a member of A and
        /// P2 is a member of B.
        /// </para>
        /// <para>
        /// This method is useful in determining the distance between two landmasses 
        /// where their cells are the closest.
        /// </para>
        /// </summary>
        public static double ShortestCellDistance(this List<MapCell> cells1, List<MapCell> cells2)
        {
            double shortestDistance = double.MaxValue;

            foreach(var cell1 in cells1)
            {
                foreach(var cell2 in cells2)
                {
                    var distance = cell1.Center.Distance(cell2.Center);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                    }
                }
            }

            return shortestDistance;
        }

        public static DelaunatorSharp.IPoint Average(this DelaunatorSharp.IPoint[] points)
        {
            return new DelaunatorSharp.Point(points.Average(x => x.X), points.Average(x => x.Y));
        }



    }
}
