using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    /// <summary>
    /// Identical to the DefaultHeightMapGenerator except it seeds the borders
    /// of the map with a uniform low value. This causes generated height maps
    /// to look like an island or continent.
    /// </summary>
    public class OldIslandHeightMapGenerator : HeightMapGenerator
    {

        /// <summary>
        /// Essentially how thick the space between the map
        /// border and land should be.
        /// </summary>
        public int Margin { get; set; }

        public OldIslandHeightMapGenerator(int margin)
        {
            this.Margin = margin;
        }

        private float GetRandomSeaElevation()
        {
            return 0.15f + (0.02f * (float)Random.NextDouble());
        }

        protected override void SeedMap(float[][] map)
        {
            
            var width = this.Width;
            var height = this.Height;

            for (int i = 0; i < width; i++)
            {
                for (int m = 0; m < this.Margin; m++)
                {
                    map[i][0 + m] = GetRandomSeaElevation();
                    map[i][height - 1 - m] = GetRandomSeaElevation();

                    map[0 + m][i] = GetRandomSeaElevation();
                    map[width - 1 - m][i] = GetRandomSeaElevation();
                }
            }

            var count = (width + height) / 20;

            for(int i = 0; i < count; i++)
            {
                var x = Random.Next(this.Margin, width - this.Margin);
                var y = Random.Next(this.Margin, height - this.Margin);

                for(int j = x; j < 10 && j < width; j++)
                {
                    for(int h = y; h < 10 && h < height; h++)
                    {
                        map[j][h] = GetRandomSeaElevation();
                    }
                }
            }
        }
    }
}
