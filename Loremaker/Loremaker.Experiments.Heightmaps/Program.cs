using Loremaker.Maps;
using System;
using System.Diagnostics;

namespace Loremaker.Experiments.Heightmaps
{
    // Experimentation
    public class Program
    {
        private static Stopwatch timer = new Stopwatch();
        private static HeightMapGenerator hg = new HeightMapGenerator();

        public static void Main(string[] args)
        {
            GenerateHeightMap(1000, 1000);
            GenerateHeightMap(1024, 1024);
            GenerateHeightMap(1025, 1025);
            GenerateHeightMap(2000, 2000);
            GenerateHeightMap(2048, 2048);
            GenerateHeightMap(2049, 2049);

            var map = hg.Next(4096, 4096);
            FindPercentageAboveThreshold(map, 1);
            FindPercentageAboveThreshold(map, 1);
            FindPercentageAboveThreshold(map, 2);
            FindPercentageAboveThreshold(map, 3);
            FindPercentageAboveThreshold(map, 4);
            FindPercentageAboveThreshold(map, 5);
            FindPercentageAboveThreshold(map, 10);

        }

        private static void FindPercentageAboveThreshold(float[][] map, int skip)
        {
            timer.Restart();
            var above = PercentageAboveThreshold(map, 0.5f, skip);
            timer.Stop();

            Console.WriteLine("{0}\t{1}:\t{2}ms\t({3})", "PercentageAboveThreshold w/ " + skip + " skip", map.Length + "x" + map[0].Length, timer.ElapsedMilliseconds, above);
        }

        private static float PercentageAboveThreshold(float[][] map, float threshold, int skip = 1)
        {
            int aboveThreshold = 0;

            for(int x = 0; x < map.Length; x += skip)
            {
                for(int y = 0; y < map[0].Length; y += skip)
                {
                    if(map[x][y] > threshold)
                    {
                        aboveThreshold++;
                    }
                }
            }

            return (float)aboveThreshold/(map.Length * map[0].Length)*skip*skip;
        }

        private static void GenerateHeightMap(int width, int height)
        {
            timer.Restart();
            hg.Next(width, height);
            timer.Stop();

            Console.WriteLine("{0}\t{1}:\t{2}ms", "HeightMapGenerator", width + "x" + height, timer.ElapsedMilliseconds);
        }

    }
}
