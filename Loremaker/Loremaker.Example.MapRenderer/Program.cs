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
using System.Text.Json;
using Loremaker.Text;

namespace Loremaker.Example.MapRenderer
{
    // This is for experimentation only
    public class Program
    {
        private static FontCollection DefaultFontCollection = new FontCollection();
        private static FontFamily DefaultFontFamily = DefaultFontCollection.Install("Fonts/ferrum.ttf");
        private static Font SmallFont = DefaultFontFamily.CreateFont(25, FontStyle.Regular);
        private static Font NormalFont = DefaultFontFamily.CreateFont(50, FontStyle.Regular);
        private static Font BigFont = DefaultFontFamily.CreateFont(100, FontStyle.Regular);
        private static Random Random = new Random();
        private static string FilePath = "world.json";
        
        public static void Main(string[] args)
        {
            var start = DateTime.Now;

            var forceNew = true;

            if(!File.Exists(FilePath) || forceNew)
            {
                var worlds = new WorldGenerator()
                                .UsingMapGenerator(x => x
                                    .UsingDimension(2000, 2000)
                                    .UsingLandThreshold(0.3f))
                                .UsingNameGenerator(x => x
                                    .UsingSyllables(x => x
                                        .WithVowels("aeiouy")
                                        .WithConsonants("bcdfgthlmnprts")
                                        .WithLeadingConsonantSequences("qu")
                                        .WithProbability(x => x
                                            .OfLeadingConsonantIsSequence(0.02)))
                                    .UsingSyllableCount(2, 3))
                                .UsingDescriptionGenerator(new GibberishTextGenerator()
                                    .UsingSentenceLength(1));

                var world = worlds.Next();

                Console.WriteLine("Generated after {0} seconds", (DateTime.Now - start).TotalSeconds);

                DrawWorld(world);

                Console.WriteLine("Drew image after {0} seconds", (DateTime.Now - start).TotalSeconds);

                World.Serialize(world, FilePath);

                Console.WriteLine("Serialized after {0} seconds", (DateTime.Now - start).TotalSeconds);
            }
            else
            {

                var world = World.Deserialize(FilePath);

                Console.WriteLine("Loaded existing file after {0} seconds", (DateTime.Now - start).TotalSeconds);

                DrawWorld(world);

                Console.WriteLine("Drew image after {0} seconds", (DateTime.Now - start).TotalSeconds);
            }

        }

        private static void DrawWorld(World world)
        {
            var map = world.Map;
            var output = "cellmap.png";

            var e = map.MapCells.Values.ElementAt(10);

            var highlight = new List<uint>() { e.Id };
            highlight.AddRange(e.AdjacentMapCellIds);

            using (var image = new Image<Rgba32>(map.Width, map.Height))
            {

                // Draw edges first to ensure any pixel gaps between polygons
                // will be filled in
                foreach (var cell in map.MapCells.Values)
                {
                    if (cell.MapPoints.Count > 2)
                    {
                        var color = GetColor(cell, map.LandThreshold);

                        image.Mutate(x => x
                            .DrawLines(new Pen(color, 3f), cell.MapPoints.Select(p => new PointF(p.X,p.Y)).ToArray())
                        );
                    }
                }

                // Now draw map cell polygons
                foreach (var cell in map.MapCells.Values)
                {
                    if (cell.MapPoints.Count > 2)
                    {
                        var color = GetColor(cell, map.LandThreshold);

                        image.Mutate(x => x
                            .FillPolygon(color, cell.MapPoints.Select(p => new PointF(p.X, p.Y)).ToArray())
                        );
                    }
                }

                var colors = new List<Color>()
                {
                    Color.Red,
                    Color.Orange,
                    Color.Yellow,
                    Color.GreenYellow,
                    Color.Green,
                    Color.DarkGreen,
                    Color.DarkBlue,
                    Color.Blue,
                    Color.BlueViolet,
                    Color.Violet,
                    Color.PaleVioletRed
                };

                colors.AddRange(Color.WebSafePalette.ToArray());

                // Regions
                foreach (var territory in world.Territories.Values)
                {
                    var color = colors.RemoveRandom();

                    if (color == null) color = Color.White;

                    /*
                    foreach (var cell in territory.MapCells)
                    {
                        if (cell.MapPoints.Count > 2)
                        {
                            var centerX = cell.X;
                            var centerY = cell.Y;

                            image.Mutate(x => x
                                .FillPolygon(color, cell.MapPoints.Select(p => new PointF(p.X, p.Y)).ToArray()));
                        }
                    }/**/

                    /*
                    image.Mutate(x => x
                        .DrawLines(new Pen(color, 2f), territory.ContainingMapPointIds.Select(id => new PointF(world.Map.MapPoints[id].X, world.Map.MapPoints[id].Y)).ToArray()));
                    */
                }

                // Now draw population centers

                foreach (var pop in world.PopulationCenters.Values)
                {
                    image.Mutate(x => x
                        .DrawLines(new Pen(Color.LightGray, 10f), new PointF(pop.MapCell.X, pop.MapCell.Y), new PointF(pop.MapCell.X, pop.MapCell.Y))
                        .DrawText(pop.Name, SmallFont, Color.White, new PointF(pop.MapCell.X + 10, pop.MapCell.Y + 10))
                    );
                    
                }

                // Show names of landmasses
                var textOptions = new TextOptions()
                {
                     HorizontalAlignment = HorizontalAlignment.Center,
                     VerticalAlignment = VerticalAlignment.Center,
                };

                var drawingOptions = new DrawingOptions() { TextOptions = textOptions };

                
                foreach (var landmass in world.Landmasses.Values.Where(x => x.MapCells.Count > 2))
                {
                    if(landmass.MapCells.Count > 100)
                    {
                        image.Mutate(x => x
                            .DrawText(drawingOptions, landmass.Name, BigFont, Brushes.Solid(Color.White), Pens.Solid(Color.Black, 2.5f), new PointF(landmass.X, landmass.Y))
                        );
                    }
                    else if(landmass.MapCells.Count < 20)
                    {
                        /*
                        drawingOptions.TextOptions.VerticalAlignment = VerticalAlignment.Bottom;

                        image.Mutate(x => x
                            .DrawText(drawingOptions, landmass.Name, SmallFont, Brushes.Solid(Color.Black), Pens.Solid(Color.Black, 1), new PointF(landmass.Center.X, landmass.Center.Y))
                        );
                        /**/
                    }
                    else
                    {
                        image.Mutate(x => x
                            .DrawText(drawingOptions, landmass.Name, NormalFont, Brushes.Solid(Color.Black), Pens.Solid(Color.Black, 0.5f), new PointF(landmass.X, landmass.Y))
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
