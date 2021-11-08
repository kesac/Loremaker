using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    public struct MapPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MapPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
