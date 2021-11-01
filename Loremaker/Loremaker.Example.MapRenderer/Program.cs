using DelaunatorSharp;
using Loremaker.Maps;
using SixLabors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.IO;
using SixLabors.ImageSharp.Drawing.Processing.Processors.Drawing;
using System.Linq;

namespace Loremaker.Example.MapRenderer
{

    public static class MapRendererExtensions
    {
        public static PointF ToPointF(this IPoint point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }

        public static PointF[] ToPointFArray(this IPoint[] points)
        {
            return points.Select(x => x.ToPointF()).ToArray();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var defaultGenerator = new HeightMapGenerator();
            defaultGenerator.AllowSeeding = false;
            // Generate(defaultGenerator, "Default", 1);

            var constrainedGenerator = new ConstrainedHeightMapGenerator();
            constrainedGenerator.AllowSeeding = false;
            constrainedGenerator.MaximumAttemps = 1000;
            constrainedGenerator.HeightThreshold = 0.25;
            constrainedGenerator.DesiredMinimumPercentageBelowThreshold = 0.5;
            // Generate(constrainedGenerator, "Constrained", 1);

            var islandGenerator = new IslandHeightMapGenerator(2).UsingVarianceDrop(0.4);
            // Generate(islandGenerator, "Island", 1);


            var mapGenerator = new MapGenerator(1000, 1000);
            var map = mapGenerator.Next();
            DrawCellmap(map);
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

        private static void DrawCellmap(Map map)
        {
            var start = DateTime.Now;
            var output = "cellmap.png";

            using (var image = new Image<Rgba32>(map.Width, map.Height))
            {

                var cells = map.VoronoiMap.GetVoronoiCells();

                foreach (var cell in cells)
                {
                    if (cell.Points.Length > 2)
                    {
                        var elevation = map.HeightMap[cell.AverageX(), cell.AverageY()];
                        var color = new Rgba32() { A = 255 };

                        if(elevation < 0.20)
                        {
                            color.R = 0;
                            color.G = (byte)((150-30) * elevation/0.20 + 30);
                            color.B = (byte)(elevation < 0.10 ? ((255 - 150) * elevation / 0.10 + 150) : 255);
                        }
                        else
                        {
                            color.R = 30;
                            color.G = (byte)((255 - 155) * elevation / 0.80 + 155);
                            color.B = 0;
                        }

                        image.Mutate(x => x
                            .DrawLines(new Pen(color, 3f), cell.Points.ToPointFArray())
                        );
                    }
                }

                foreach (var cell in cells)
                {
                    if (cell.Points.Length > 2)
                    {
                        var elevation = map.HeightMap[cell.AverageX(), cell.AverageY()];
                        var color = new Rgba32() { A = 255 };

                        if (elevation < 0.20)
                        {
                            color.R = 0;
                            color.G = (byte)((150 - 30) * elevation / 0.20 + 30);
                            color.B = (byte)(elevation < 0.10 ? ((255 - 150) * elevation / 0.10 + 150) : 255);
                        }
                        else
                        {
                            color.R = 30;
                            color.G = (byte)((255 - 155) * elevation / 0.80 + 155);
                            color.B = 0;
                        }

                        image.Mutate(x => x
                            .FillPolygon(color, cell.Points.ToPointFArray())
                        );
                    }
                }

                using (var filestream = new FileStream(output, FileMode.Create))
                {
                    image.SaveAsPng(filestream);
                }
            }

            Console.WriteLine("Generated {0} in {1} seconds", output, (DateTime.Now - start).TotalSeconds);
        }
    }
}
