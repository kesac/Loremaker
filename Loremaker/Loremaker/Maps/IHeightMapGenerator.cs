﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    /// <summary>
    /// Generates height map data.
    /// </summary>
    public interface IHeightMapGenerator
    {
        /// <summary>
        /// Given a specified grid width and height, this method returns a 2-dimensional
        /// array representing a height map. Each item in the array is a number between 
        /// 0 and 1 inclusive (the larger the value, the higher that location is).
        /// </summary>
        double[,] Next(int width, int height);
    }
}
