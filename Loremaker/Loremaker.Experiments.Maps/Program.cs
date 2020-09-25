using Loremaker.Maps;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;

namespace Loremaker.Experiments.Maps
{
    public class Program
    {

        private static FontCollection DefaultFontCollection = new FontCollection();
        private static FontFamily DefaultFontFamily = DefaultFontCollection.Install("GlacialIndifference-Regular.ttf");
        private static Font DefaultFont = DefaultFontFamily.CreateFont(30, FontStyle.Regular); 

        public static void Main(string[] args)
        {
            var worldGenerator = new WorldGenerator()
                .SetTotalContinents(2, 5)
                .SetHeightMapGenerator(
                    new IslandHeightMapGenerator()
                    {
                        Margin = 2,
                        VarianceDropModifier = 0.4
                    }
                )
                .SetNameGenerator(
                    new CustomNameGenerator("CustomNames.xml")
                );

            var world = worldGenerator.Next();
            RenderWorld(world, "world.png");

        }

        private static void RenderWorld(World world, string filename)
        {
            var images = new Image[world.Continents.Count];

            for (int i = 0; i < world.Continents.Count; i++)
            {
                images[i] = GenerateContinentImage(world.Continents[i]);
            }

            using (var masterImage = new Image<Rgba32>(images.Sum(x => x.Width), images.Max(x => x.Height)))
            {
                int xIndex = 0;
                foreach (var image in images)
                {
                    masterImage.Mutate(x => x.DrawImage(image, new Point(xIndex, 0), 1));
                    xIndex += image.Width;
                }

                masterImage.Mutate(x => x.DrawText("World of " + world.Name, DefaultFont, Color.White, new PointF(10, 10)));

                using (var filestream = new FileStream(filename, FileMode.Create))
                {
                    masterImage.SaveAsPng(filestream);
                }

            }
        }

        private static Image GenerateContinentImage(Continent continent)
        {
            var map = continent.HeightMap;
            var image = new Image<Rgba32>(map.GetLength(0), map.GetLength(1));
            
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    var mapval = map[i, j];
                    var color = Convert.ToByte(Math.Ceiling(mapval * 255));
                    image[i, j] = new Rgba32() { R = color, G = color, B = color, A = 255 };
                }
            }


            var label = "Continent of " + continent.Name;
            var dimension = TextMeasurer.Measure(label, new RendererOptions(DefaultFont));

            image.Mutate(x => x.DrawText(label, DefaultFont, Color.White, new PointF(map.GetLength(0) / 2 - dimension.Width / 2, map.GetLength(1) / 2)));

            return image;
            
        }


    }
}
