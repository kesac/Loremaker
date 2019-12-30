using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public class IslandHeightMapGenerator : DefaultHeightMapGenerator
    {
        public int Margin { get; set; }

        public IslandHeightMapGenerator()
        {
            this.Margin = 1;
        }

        protected override void SeedMap(double[,] map)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int m = 0; m < this.Margin; m++)
                {
                    map[x, 0 + m] = 0.01;
                    map[x, height - 1 - m] = 0.01;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int m = 0; m < this.Margin; m++)
                {
                    map[0 + m, y] = 0.01;
                    map[width - 1 - m, y] = 0.01;
                }
            }
        }
    }
}
