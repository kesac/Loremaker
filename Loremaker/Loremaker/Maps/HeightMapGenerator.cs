using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    /// <summary>
    /// A basic height map generator that uses the diamond-square algorithm.
    /// </summary>
    public class HeightMapGenerator : IHeightMapGenerator
    {
        protected Random Random { get; set; }
        public bool AllowSeeding { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// This property adjusts the rate at which variance drops
        /// in each diamond-square step. The property of this value must
        /// be between 0 and 1 inclusive. Values closer to 0 make generated
        /// maps smoother. Values closer to 1 make generated maps bumpier.
        /// </summary>
        public float VarianceDropModifier { get; set; }

        public HeightMapGenerator()
        {
            this.Random = new Random();
            this.AllowSeeding = true;
            this.Width = 1024;
            this.Height = 1024;
            this.VarianceDropModifier = 0.5f;
        }

        /// <summary>
        /// The higher the variance, the more chaotic the output.
        /// </summary>
        public HeightMapGenerator UsingVarianceDrop(float varianceDrop)
        {
            this.VarianceDropModifier = varianceDrop;
            return this;
        }

        public HeightMapGenerator UsingSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            return this;
        }

        public virtual float[][] Next()
        {
            return this.Next(this.Width, this.Height);
        }

        public virtual float[][] Next(int width, int height)
        {

            if(this.VarianceDropModifier < 0 || this.VarianceDropModifier > 1)
            {
                throw new InvalidOperationException("VarianceModifier must be a value between 0 and 1 inclusive");
            }

            // The diamond-square algorithm can only generate maps of size 2^n+1.
            // In order to generate maps of any dimension, we generate a 2^n+1 map that
            // is larger than the desired dimensions than remove the "extra" parts.

            if(width > height)
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

        private float[][] ShrinkArray(float[][] original, int targetWidth, int targetHeight)
        {

            if(original.Length == targetWidth && original[0].Length == targetHeight)
            {
                return original;
            }

            var result = new float[targetWidth][];

            for(int i = 0; i < result.GetLength(0); i++)
            {
                result[i] = new float[targetHeight];
            }

            for (int i = 0; i < targetWidth; i++)
            {
                for(int j = 0; j < targetHeight; j++)
                {
                    result[i][j] = original[i][j];
                }
            }

            return result;
        }

        // This implementation is based off 'hmapgen' (https://github.com/kesac/hmapgen).
        private float[][] GenerateHeightMap(int mapsize)
        {
            float[][] map = new float[mapsize][];
            for(int i = 0; i < map.GetLength(0); i++)
            {
                map[i] = new float[mapsize];
            }

            if (this.AllowSeeding)
            {
                this.SeedMap(map);
            }

            int step = mapsize - 1;
            float variance = 1;

            while (step > 1)
            {

                // Square step
                for (int i = 0; i < mapsize - 1; i += step)
                {
                    for (int j = 0; j < mapsize - 1; j += step)
                    {
                        if (map[i + step / 2][j + step / 2] == 0)  // check if not pre-seeded
                        {
                            float average = (map[i][j] + map[i + step][j] + map[i][j + step] + map[i + step][j + step]) / 4;
                            map[i + step / 2][j + step / 2] = average + this.GetRandomVariance(variance);
                            this.EnforceBounds(map, i + step / 2, j + step / 2);
                        }
                    }
                }

                // Diamond step
                for (int i = 0; i < mapsize - 1; i += step)
                {
                    for (int j = 0; j < mapsize - 1; j += step)
                    {
                        this.CalculateDiamondStep(map, i + step / 2, j, step, variance);
                        this.CalculateDiamondStep(map, i, j + step / 2, step, variance);
                        this.CalculateDiamondStep(map, i + step, j + step / 2, step, variance);
                        this.CalculateDiamondStep(map, i + step / 2, j + step, step, variance);
                    }
                }

                variance *= this.VarianceDropModifier;
                step /= 2;
            }

            return map;
        }

        protected virtual void SeedMap(float[][] map)
        {
            map[0][0] = (float)this.Random.NextDouble();
            map[0][map[0].Length - 1] = (float)this.Random.NextDouble();
            map[map.GetLength(0) - 1][0] = (float)this.Random.NextDouble();
            map[map.GetLength(0) - 1][map[0].Length - 1] = (float)this.Random.NextDouble();
        }

        private void CalculateDiamondStep(float[][] map, int x, int y, int step, float variance)
        {
            if (map[x][y] == 0)
            {
                map[x][y] = this.GetDiamondStepAverage(map, x, y, step) + this.GetRandomVariance(variance);
                this.EnforceBounds(map, x, y);
            }
        }


        private float GetDiamondStepAverage(float[][] map, int x, int y, int step)
        {

            int count = 0;
            float average = 0;

            if (x - step / 2 >= 0)
            {
                count++;
                average += map[x - step / 2][y];
            }

            if (x + step / 2 < map.Length)
            {
                count++;
                average += map[x + step / 2][y];
            }

            if (y - step / 2 >= 0)
            {
                count++;
                average += map[x][y - step / 2];
            }

            if (y + step / 2 < map[0].Length)
            {
                count++;
                average += map[x][y + step / 2];
            }

            return average / count;
        }

        private float GetRandomVariance(float variance)
        {
            return (float)this.Random.NextDouble() * 2 * variance - variance;
        }

        private void EnforceBounds(float[][] map, int x, int y)
        {
            if (map[x][y] < 0)
            {
                map[x][y] = 0;
            }
            else if (map[x][y] > 1)
            {
                map[x][y] = 1;
            }
        }

    }
}
