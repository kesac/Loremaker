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
            var generator = new DefaultHeightMapGenerator();

            for (int run = 0; run < 10; run++)
            {
                var start = DateTime.Now;
                var map = generator.Next(800, 600);
                var imageName = "test" + run + ".png";

                using (var image = new Image<Rgba32>(map.GetLength(0), map.GetLength(1)))
                {
                    for (int i = 0; i < map.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.GetLength(1); j++)
                        {
                            var mapval = map[i, j];

                            if (mapval < 0)
                            {
                                mapval = 0;
                            }
                            else if (mapval > 1)
                            {
                                mapval = 1;
                            }

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
