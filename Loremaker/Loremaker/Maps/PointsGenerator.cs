using Archigen;
using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class PointsGenerator : IGenerator<IPoint[]>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Spacing { get; set; }
        public bool ShiftingPermitted { get; set; }
        public int ShiftingMaxDistance { get; set; }

        public PointsGenerator(int width, int height, int initialSpacing)
        {
            this.Width = width;
            this.Height = height;
            this.Spacing = initialSpacing;
            this.ShiftingPermitted = true;
            this.ShiftingMaxDistance = 3;
        }

        public PointsGenerator ShiftPoints(bool allow)
        {
            this.ShiftingPermitted = allow;
            return this;
        }

        public PointsGenerator ShiftPointsUpTo(int distance)
        {
            this.ShiftingMaxDistance = distance;
            return this;
        }

        public IPoint[] Next()
        {
            List<IPoint> points = new List<IPoint>();
            for(int x = this.Spacing; x <= this.Width - this.Spacing; x += this.Spacing)
            {
                for(int y = this.Spacing; y <= this.Height - this.Spacing; y += this.Spacing)
                {
                    points.Add(new Point(
                        x + Chance.Between(-this.ShiftingMaxDistance, this.ShiftingMaxDistance),
                        y + Chance.Between(-this.ShiftingMaxDistance, this.ShiftingMaxDistance)));
                }
            }

            return points.ToArray();
        }
    }
}
