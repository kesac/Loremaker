using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    public class MapPoint : IEntity, ILocatable
    {
        public uint Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public MapPoint() : this(0, 0) { }

        public MapPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
