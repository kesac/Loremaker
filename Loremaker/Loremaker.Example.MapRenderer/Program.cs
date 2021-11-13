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
        private static List<Color> SolidColors = new List<Color>() { Color.Red, Color.Blue, Color.Purple, Color.Gray, Color.BlueViolet, Color.DeepPink, Color.Orange };

        public static void Main(string[] args)
        {
            var start = DateTime.Now;

            if(!File.Exists(FilePath))
            {
                var worlds = new WorldGenerator(2000, 2000);
                var world = worlds.Next();

                Console.WriteLine("Generated after {0} seconds", (DateTime.Now - start).TotalSeconds);

                DrawWorld(world);

                Console.WriteLine("Drew image after {0} seconds", (DateTime.Now - start).TotalSeconds);

                var json = JsonSerializer.Serialize(world);
                File.WriteAllText(FilePath, json);

                Console.WriteLine("Serialized after {0} seconds", (DateTime.Now - start).TotalSeconds);
            }
            else
            {
                var json = File.ReadAllText(FilePath);
                var world = JsonSerializer.Deserialize<World>(json);

                Console.WriteLine("Loaded existing file after {0} seconds", (DateTime.Now - start).TotalSeconds);

                // Restore back references

                foreach (var point in world.Map.MapPoints)
                {
                    world.Map.MapPointsById[point.Id] = point;
                }

                foreach (var cell in world.Map.MapCells)
                {
                    world.Map.MapCellsById[cell.Id] = cell;
                    var points = cell.MapPointIds.Select(id => world.Map.MapPointsById[id]);
                    cell.MapPoints.AddRange(points);
                }

                // This next loop cannot run until MapCellsById is fully populated
                // in the previous loop
                foreach (var cell in world.Map.MapCellsById.Values)
                {
                    var cells = cell.AdjacentMapCellIds.Select(id => world.Map.MapCellsById[id]);
                    cell.AdjacentMapCells.AddRange(cells);
                }

                foreach (var landmass in world.Map.Landmasses)
                {
                    var cells = landmass.MapCellIds.Select(id => world.Map.MapCellsById[id]);
                    landmass.MapCells.AddRange(cells);

                    world.Map.LandmassesById[landmass.Id] = landmass;
                }

                // More backreferences

                foreach (var pop in world.PopulationCenters)
                {
                    pop.MapCell = world.Map.MapCellsById[pop.MapCellId];
                    pop.Landmass = world.Map.LandmassesById[pop.LandmassId];
                }

                foreach (var territory in world.Territories)
                {
                    var cells = territory.MapCellIds.Select(id => world.Map.MapCellsById[id]);
                    territory.MapCells.AddRange(cells);
                }

                Console.WriteLine("Backreferences restored after {0} seconds", (DateTime.Now - start).TotalSeconds);

                DrawWorld(world);

                Console.WriteLine("Drew image after {0} seconds", (DateTime.Now - start).TotalSeconds);
            }

        }

        private static void DrawWorld(World world)
        {

            var map = world.Map;
            var output = "cellmap.png";

            var e = map.MapCellsById.Values.ElementAt(10);

            var highlight = new List<uint>() { e.Id };
            highlight.AddRange(e.AdjacentMapCellIds);

            using (var image = new Image<Rgba32>(map.Width, map.Height))
            {

                // Draw edges first to ensure any pixel gaps between polygons
                // will be filled in
                foreach (var cell in map.MapCells)
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
                foreach (var cell in map.MapCells)
                {
                    if (cell.MapPoints.Count > 2)
                    {
                        var color = GetColor(cell, map.LandThreshold);

                        image.Mutate(x => x
                            .FillPolygon(color, cell.MapPoints.Select(p => new PointF(p.X, p.Y)).ToArray())
                        );
                    }
                }

                // Regions
                foreach (var territory in world.Territories)
                {
                    var color = SolidColors.GetRandom();

                    foreach (var cell in territory.MapCells)
                    {
                        if (cell.MapPoints.Count > 2)
                        {
                            var centerX = cell.Center.X;
                            var centerY = cell.Center.Y;

                            image.Mutate(x => x
                                .FillPolygon(color, cell.MapPoints.Select(p => new PointF(p.X, p.Y)).ToArray())
                            );
                        }
                    }
                }


                // Now draw population centers

                foreach (var pop in world.PopulationCenters)
                {
                    image.Mutate(x => x
                        .DrawLines(new Pen(Color.LightGray, 10f), new PointF(pop.MapCell.Center.X, pop.MapCell.Center.Y), new PointF(pop.MapCell.Center.X, pop.MapCell.Center.Y))
                        .DrawText(pop.Name, SmallFont, Color.White, new PointF(pop.MapCell.Center.X + 10, pop.MapCell.Center.Y + 10))
                    );
                    
                }

                // Show names of landmasses
                var textOptions = new TextOptions()
                {
                     HorizontalAlignment = HorizontalAlignment.Center,
                     VerticalAlignment = VerticalAlignment.Center,
                };

                var drawingOptions = new DrawingOptions() { TextOptions = textOptions };

                
                foreach (var landmass in world.Map.Landmasses.Where(x => x.MapCells.Count > 2))
                {
                    if(landmass.MapCells.Count > 100)
                    {
                        image.Mutate(x => x
                            .DrawText(drawingOptions, landmass.Name, BigFont, Brushes.Solid(Color.White), Pens.Solid(Color.Black, 2.5f), new PointF(landmass.Center.X, landmass.Center.Y))
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
                            .DrawText(drawingOptions, landmass.Name, NormalFont, Brushes.Solid(Color.Black), Pens.Solid(Color.Black, 0.5f), new PointF(landmass.Center.X, landmass.Center.Y))
                        );
                    }   
                }

                /*
                foreach(var landmass in world.Landmasses)
                {
                    var color = new Rgba32() { R = (byte)Random.Next(255), B = (byte)Random.Next(255), G = (byte)Random.Next(255), A = 255 };

                    foreach (var cell in landmass.MapCells)
                    {
                        if (cell.Shape.Count > 2)
                        {
                            image.Mutate(x => x
                                //.FillPolygon(color, cell.Shape.Select(p => new PointF(p.X, p.Y)).ToArray())
                                .DrawLines(new Pen(color, 8f), new PointF(cell.Center.X, cell.Center.Y), new PointF(cell.Center.X, cell.Center.Y))
                            );
                        }
                    }
                }

                // Coast

                foreach (var cell in map.Cells.Values)
                {
                    if(cell.Shape.Count > 2 && cell.IsCoast)
                    {
                        // var color = new Rgba32() { R = 150, B = 25, G = 150, A = 255 };
                        var color = Color.Red;

                        image.Mutate(x => x
                            //.FillPolygon(color, cell.Shape.Select(p => new PointF(p.X, p.Y)).ToArray())
                            .DrawLines(new Pen(color, 2f), new PointF(cell.Center.X, cell.Center.Y), new PointF(cell.Center.X, cell.Center.Y))
                        );
                    }
                }
                */



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
