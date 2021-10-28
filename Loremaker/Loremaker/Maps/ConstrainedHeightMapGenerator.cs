using System;
using System.Collections.Generic;
using System.Text;

namespace Loremaker.Maps
{
    /// <summary>
    /// This is an experimental class for trying to learn how to tune or control the output of the DefaultHeightMapGenerator
    /// </summary>
    public class ConstrainedHeightMapGenerator : HeightMapGenerator
    {
        public double HeightThreshold { get; set; }
        public double DesiredMinimumPercentageBelowThreshold { get; set; }

        public int MaximumAttemps { get; set; }

        public ConstrainedHeightMapGenerator()
        {
            this.HeightThreshold = 0.5;
            this.DesiredMinimumPercentageBelowThreshold = 0.25;
            this.MaximumAttemps = 100;
        }

        public override double[,] Next(int width, int height)
        {
            int attempts = 0;

            while (attempts++ < this.MaximumAttemps)
            {
                var map = base.Next(width, height);

                int totalBelowThreshold = 0;
                for(int i = 0; i < width; i++)
                {
                    for(int j = 0; j < height; j++)
                    {
                        if(map[i,j] < this.HeightThreshold)
                        {
                            totalBelowThreshold++;
                        }
                    }
                }

                if(((double)totalBelowThreshold/(width*height)) >= DesiredMinimumPercentageBelowThreshold)
                {
                    return map;
                }
            }

            throw new InvalidOperationException("Maximum attempts reached without producing a valid map. Increase the attempts or lower the constraints.");

        }

    }
}
