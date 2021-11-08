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
using System.Collections.Generic;

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


            var mapGenerator = new MapGenerator(1000, 1000, 0.20);
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

        private static Color GetColor(MapCell cell, double landThreshold)
        {
            var result = new Rgba32() { A = 255 };
            var elevation = cell.Elevation;

            if (cell.IsWater)
            {
                result.R = 0;
                result.G = (byte)((150 - 30) * elevation / landThreshold + 30);
                result.B = (byte)(elevation < (landThreshold / 2) ? ((255 - 150) * elevation / (landThreshold / 2) + 150) : 255);
            }
            else
            {
                result.R = 30;
                result.G = (byte)((255 - 155) * elevation / (1 - landThreshold) + 155);
                result.B = 0;
            }

            return result;
        }

        private static void DrawCellmap(Map map)
        {
            var start = DateTime.Now;
            var output = "cellmap.png";

            var e = map.Cells.Values.ElementAt(10);

            var highlight = new List<int>() { e.CellId };
            highlight.AddRange(e.AdjacentCellIds);

            using (var image = new Image<Rgba32>(map.Width, map.Height))
            {

                foreach (var cell in map.Cells.Values)
                {
                    if (cell.Shape.Count > 2)
                    {
                        var color = GetColor(cell, map.LandThreshold);

                        image.Mutate(x => x
                            .DrawLines(new Pen(color, 3f), cell.Shape.Select(p => new PointF(p.X,p.Y)).ToArray())
                        );
                    }
                }

                foreach (var cell in map.Cells.Values)
                {
                    if (cell.Shape.Count > 2)
                    {
                        var color = GetColor(cell, map.LandThreshold);

                        image.Mutate(x => x
                            .FillPolygon(color, cell.Shape.Select(p => new PointF(p.X, p.Y)).ToArray())
                        );
                    }
                }

                foreach (var cell in map.Cells.Values)
                {
                    if(cell.Shape.Count > 2 && cell.IsCoast)
                    {
                        // var color = new Rgba32() { R = 150, B = 25, G = 150, A = 255 };
                        var color = Color.Red;

                        image.Mutate(x => x
                            //.FillPolygon(color, cell.Shape.Select(p => new PointF(p.X, p.Y)).ToArray())
                            .DrawLines(new Pen(color, 3f), new PointF(cell.Center.X, cell.Center.Y), new PointF(cell.Center.X, cell.Center.Y))
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
