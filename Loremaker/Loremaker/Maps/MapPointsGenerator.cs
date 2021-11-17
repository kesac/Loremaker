using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class MapPointsGenerator : IGenerator<MapPoint[]>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Spacing { get; set; }
        public bool ShiftingPermitted { get; set; }
        public int ShiftingMaxDistance { get; set; }

        public MapPointsGenerator(int width, int height, int initialSpacing)
        {
            this.Width = width;
            this.Height = height;
            this.Spacing = initialSpacing;
            this.ShiftingPermitted = true;
            this.ShiftingMaxDistance = 4;
        }

        public MapPointsGenerator ShiftPoints(bool allow)
        {
            this.ShiftingPermitted = allow;
            return this;
        }

        public MapPointsGenerator ShiftPointsUpTo(int distance)
        {
            this.ShiftingMaxDistance = distance;
            return this;
        }

        public MapPoint[] Next()
        {
            var points = new List<MapPoint>();

            for(int x = this.Spacing; x <= this.Width - this.Spacing; x += this.Spacing)
            {
                for(int y = this.Spacing; y <= this.Height - this.Spacing; y += this.Spacing)
                {
                    var point = new MapPoint(
                        x + Chance.Between(-this.ShiftingMaxDistance, this.ShiftingMaxDistance),
                        y + Chance.Between(-this.ShiftingMaxDistance, this.ShiftingMaxDistance)
                    );

                    points.Add(point);
                }
            }

            return points.ToArray();
        }
    }
}
