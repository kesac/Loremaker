using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    /// <summary>
    /// A basic height map generator that uses the diamond-square algorithm.
    /// </summary>
    public class DefaultHeightMapGenerator : IHeightMapGenerator
    {
        private Random Random { get; set; }

        public bool SeedCorners { get; set; }

        public DefaultHeightMapGenerator()
        {
            this.Random = new Random();
            this.SeedCorners = true;
        }

        public double[,] Next(int width, int height)
        {
            // The diamond-square algorithm can only generate maps of size 2^n+1.
            // In order to generate maps of any dimension, we generate a 2^n+1 map that
            // is larger than the desired dimensions than remove the "extra" parts.

            if(width < height)
            {
                double exponentForWidth = Math.Log(width) / Math.Log(2);
                var map = this.GenerateHeightMap(Convert.ToInt32(Math.Pow(2, Math.Ceiling(exponentForWidth)) + 1));
                return ShrinkArray(map, width, height);  
                
            }
            else
            {
                double exponentForHeight = Math.Log(height) / Math.Log(2);
                var map = this.GenerateHeightMap(Convert.ToInt32(Math.Pow(2, Math.Ceiling(exponentForHeight)) + 1));
                return ShrinkArray(map, width, height);
            }

        }

        private double[,] ShrinkArray(double[,] original, int targetWidth, int targetHeight)
        {
            var result = new double[targetWidth, targetHeight];
            for (int i = 0; i < targetWidth; i++)
            {
                for(int j = 0; j < targetHeight; j++)
                {
                    result[i, j] = original[i, j];
                }
            }
            return result;
        }

        // This implementation is based off 'hmapgen' (https://github.com/kesac/hmapgen).
        private double[,] GenerateHeightMap(int mapsize)
        {
            double[,] map = new double[mapsize,mapsize];

            if (this.SeedCorners)
            {
                map[0, 0] = this.Random.NextDouble() / 2 + 0.25;
                map[0, mapsize - 1] = this.Random.NextDouble() / 2 + 0.25;
                map[mapsize - 1, 0] = this.Random.NextDouble() / 2 + 0.25;
                map[mapsize - 1, mapsize - 1] = this.Random.NextDouble() / 2 + 0.25;
            }

            int step = mapsize - 1;
            double variance = 1;

            while (step > 1)
            {

                // Square step
                for (int i = 0; i < mapsize - 1; i += step)
                {
                    for (int j = 0; j < mapsize - 1; j += step)
                    {

                        double average = (map[i, j] + map[i + step, j] + map[i, j + step] + map[i + step, j + step]) / 4;

                        if (map[i + step / 2, j + step / 2] == 0)  // check if not pre-seeded
                        {
                            map[i + step / 2, j + step / 2] = average + GetRandomVariance(variance);
                        }
                    }
                }

                // Diamond step
                for (int i = 0; i < mapsize - 1; i += step)
                {
                    for (int j = 0; j < mapsize - 1; j += step)
                    {
                        if (map[i + step / 2, j] == 0)
                        {
                            map[i + step / 2, j] = GetDiamondAverage(map, i + step / 2, j, step) + GetRandomVariance(variance);
                        }

                        if (map[i, j + step / 2] == 0)
                        {
                            map[i, j + step / 2] = GetDiamondAverage(map, i, j + step / 2, step) + GetRandomVariance(variance);
                        }

                        if (map[i + step, j + step / 2] == 0)
                        {
                            map[i + step, j + step / 2] = GetDiamondAverage(map, i + step, j + step / 2, step) + GetRandomVariance(variance);
                        }

                        if (map[i + step / 2, j + step] == 0)
                        {
                            map[i + step / 2, j + step] = GetDiamondAverage(map, i + step / 2, j + step, step) + GetRandomVariance(variance);
                        }
                    }
                }

                variance /= 2;
                step /= 2;
            }

            return map;
        }

        private double GetDiamondAverage(double[,] map, int x, int y, int step)
        {

            int count = 0;
            double average = 0;

            if (x - step / 2 >= 0)
            {
                count++;
                average += map[x - step / 2, y];
            }

            if (x + step / 2 < map.GetLength(0))
            {
                count++;
                average += map[x + step / 2, y];
            }

            if (y - step / 2 >= 0)
            {
                count++;
                average += map[x, y - step / 2];
            }

            if (y + step / 2 < map.GetLength(1))
            {
                count++;
                average += map[x, y + step / 2];
            }

            return average / count;
        }

        private double GetRandomVariance(double v)
        {
            return this.Random.NextDouble() * 2 * v - v;
        }

    }
}
