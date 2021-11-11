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
using System.Diagnostics;
using SixLabors.Fonts;

namespace Loremaker.Example.MapRenderer
{

    public class Program
    {

        private static Random Random = new Random();

        public static void Main(string[] args)
        {
            var start = DateTime.Now;

            var worlds = new WorldGenerator(1000, 2000);

            DrawWorld(worlds.Next());

            Console.WriteLine("Generated in {0} seconds", (DateTime.Now - start).TotalSeconds);

        }

        private static void DrawWorld(World world)
        {

            var map = world.Map;
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

            
            
            try
            {
                var process = new ProcessStartInfo(output) { UseShellExecute = true, Verb = "open" };
                Process.Start(process);
            }
            catch(Exception)
            {

            }
        }

        private static Color GetColor(MapCell cell, double landThreshold)
        {
            var result = new Rgba32() { A = 255 };
            var elevation = cell.Elevation;

            if (cell.IsWater)
            {
                result.R = 0;
                result.G = (byte)((150 - 30) * (elevation / landThreshold) + 30);
                result.B = (byte)(elevation < (landThreshold / 2) ? ((255 - 150) * elevation / (landThreshold / 2) + 150) : 255);
            }
            else
            {
                result.R = 30;
                result.G = (byte)Math.Min(255, ((255 - 155) * elevation / (1 - landThreshold) + 155));
                result.B = 0;
            }

            return result;
        }


    }
}
