using Loremaker.Maps;
using SixLabors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace Loremaker.Example.MapRenderer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var defaultGenerator = new DefaultHeightMapGenerator();
            defaultGenerator.AllowSeeding = false;
            Generate(defaultGenerator, "Default", 3);

            var constrainedGenerator = new ConstrainedHeightMapGenerator();
            constrainedGenerator.AllowSeeding = false;
            constrainedGenerator.MaximumAttemps = 1000;
            constrainedGenerator.HeightThreshold = 0.25;
            constrainedGenerator.DesiredMinimumPercentageBelowThreshold = 0.5;
            Generate(constrainedGenerator, "Constrained", 3);

            var islandGenerator = new IslandHeightMapGenerator();
            islandGenerator.Margin = 2;
            islandGenerator.VarianceDropModifier = 0.4;
            Generate(islandGenerator, "Island", 3);
        }

        private static void Generate(IHeightMapGenerator generator, string fileprefix, int runs)
        {
            for (int run = 0; run < runs; run++)
            {
                var start = DateTime.Now;
                var map = generator.Next(1000, 1000);
                var imageName = fileprefix + "-test-" + run + ".png";

                using (var image = new Image<Rgba32>(map.GetLength(0), map.GetLength(1)))
                {
                    for (int i = 0; i < map.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.GetLength(1); j++)
                        {
                            var mapval = map[i, j];
                            var color = Convert.ToByte(Math.Ceiling(mapval * 255));
                            image[i, j] = new Rgba32() { R = color, G = color, B = color, A = 255 };
                        }
                    }

                    using (var filestream = new FileStream(imageName, FileMode.Create))
                    {
                        image.SaveAsPng(filestream);
                    }
                }

                Console.WriteLine("Generated {0} in {1} seconds", imageName, (DateTime.Now - start).TotalSeconds);
            }
        }
    }
}
