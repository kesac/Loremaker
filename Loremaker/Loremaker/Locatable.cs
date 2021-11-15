using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker
{
    /// <summary>
    /// Represents any entity with a locatable center
    /// in 2D space.
    /// </summary>
    public interface Locatable
    {
        /// <summary>
        /// The X coordinate representing the center of
        /// a two dimensional entity.
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// The Y coordinate representing the center of
        /// a two dimensional entity.
        /// </summary>
        int Y { get; set; }
    }
}
