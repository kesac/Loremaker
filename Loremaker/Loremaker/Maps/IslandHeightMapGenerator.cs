using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class IslandHeightMapGenerator : HeightMapGenerator
    {
        public double RadiusBuffer { get; set; }
        public double DropOffPercentage { get; set; }
        public double DropOffPercentageVariance { get; set; }
        public int Margin { get; set; }

        public IslandHeightMapGenerator(int width, int height) : this()
        {
            this.Width = width;
            this.Height = height;
        }

        public IslandHeightMapGenerator()
        {
            this.RadiusBuffer = 300;
            this.VarianceDropModifier = 0.55;
            this.DropOffPercentage = 0.15;
            this.DropOffPercentageVariance = 0.02;
            this.Margin = 0;
        }

        private double GetDropOffPercentage()
        {
            return this.DropOffPercentage + ((this.DropOffPercentageVariance*2) * Random.NextDouble()) - this.DropOffPercentageVariance;
        }

        protected override void SeedMap(double[,] map)
        {
            map[0, 0] = 0.01;
            map[0, map.GetLength(1) - 1] = 0.01;
            map[map.GetLength(0) - 1, 0] = 0.01;
            map[map.GetLength(0) - 1, map.GetLength(1) - 1] = 0.01;
        }

        public override double[,] Next()
        {
            return this.Next(this.Width, this.Height);
        }

        public override double[,] Next(int width, int height)
        {
            var result = base.Next(width, height);

            var outerRadius = (double)(width / 2) - this.Margin;
            var innerRadius = outerRadius - this.RadiusBuffer;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var distance = DistanceFromCenter(x, y, width, height);
                    // var angleFromCenter = Math.Atan2(y - height/2, x - width/2);
                    // var variance = Math.Sin(angleFromCenter * Math.PI) * 100;
                    var variance = 0;

                    if (distance > outerRadius + variance)
                    {
                        result[x, y] = result[x, y] * GetDropOffPercentage();
                    }
                    else if (distance > innerRadius + variance)
                    {
                        var gradient = 1 - ((distance - innerRadius) / this.RadiusBuffer);
                        result[x, y] = Math.Max(result[x, y] * gradient, result[x, y] * GetDropOffPercentage());
                    }

                }
            }

            return result;
        }

        private double DistanceFromCenter(int x, int y, int width, int height)
        {
            return Math.Sqrt(Math.Pow(x - width/2, 2) + Math.Pow(y - height / 2, 2));
        }

    }
}
