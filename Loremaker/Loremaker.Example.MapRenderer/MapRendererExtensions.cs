using DelaunatorSharp;
using SixLabors.ImageSharp;
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
}
