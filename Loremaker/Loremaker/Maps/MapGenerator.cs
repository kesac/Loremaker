using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class MapGenerator : Generator<Map>
    {
        private IGenerator<float[][]> HeightMapGenerator;
        private IGenerator<float[][]> ConditionalHeightMapGenerator;
        private IGenerator<MapPoint[]> PointsGenerator;
        private IGenerator<VoronoiMap> VoronoiMapGenerator;

        private float LandThreshold { get; set; }

        public MapGenerator() : this(1000, 1000, 0.30f) { }

        public MapGenerator(int width, int height, float landThreshold)
        {
            this.UsingDimension(width, height);
            this.UsingLandThreshold(this.LandThreshold);

            this.ForEach(x =>
            {
                this.DefineMap(x);
                this.DefineMapAttributes(x);
            });
        }

        public MapGenerator UsingDimension(int width, int height)
        {
            this.ForProperty<int>(x => x.Width, width);
            this.ForProperty<int>(x => x.Height, height);

            this.PointsGenerator = new MapPointsGenerator(width, height, 25);
            this.VoronoiMapGenerator = new VoronoiMapGenerator(this.PointsGenerator);
            this.HeightMapGenerator = new IslandHeightMapGenerator(width, height);
            this.ConditionalHeightMapGenerator = new ConditionalGenerator<float[][]>(this.HeightMapGenerator)
                                                    .WithCondition(x => {
                                                        // return IsValidHeightMap(x); 
                                                        return true; // TODO: this was causing an infinite loop
                                                    });

            return this;
        }

        public MapGenerator UsingLandThreshold(float threshold)
        {
            this.LandThreshold = threshold;
            this.ForProperty<float>(x => x.LandThreshold, threshold);
            return this;
        }

        private bool IsValidHeightMap(float[][] heightMap)
        {
            var percentage = this.PercentageAboveThreshold(heightMap, this.LandThreshold, 3);
            return percentage >= 0.10 && percentage <= 0.33;
        }

        /// <summary>
        /// Determines the percentage of heightmap cells that are above the specified
        /// threshold. The percentage is expressed as a float between 0 and 1.
        /// </summary>
        private float PercentageAboveThreshold(float[][] map, float threshold, int skip = 1)
        {
            int aboveThreshold = 0;

            for (int x = 0; x < map.Length; x += skip)
            {
                for (int y = 0; y < map[0].Length; y += skip)
                {
                    if (map[x][y] > threshold)
                    {
                        aboveThreshold++;
                    }
                }
            }

            return (float)aboveThreshold / (map.Length * map[0].Length) * skip * skip;
        }

        private void DefineMap(Map map)
        {
            var points = new Dictionary<(int, int), MapPoint>();
            var pointRelationships = new Dictionary<uint, List<MapCell>>();

            // Construct voronoi map which is a 2D space
            // divided into discrete polygons or cells
            var vmap = this.VoronoiMapGenerator.Next();

            // Construct a height map which will be used
            // to assign elevation to each of the map cells
            float[][] hmap = null;

            try
            {
                hmap = this.ConditionalHeightMapGenerator.Next();
            }
            catch(Exception e)
            {
                hmap = this.HeightMapGenerator.Next();
            }

            uint pointCounter = 0;

            // Transform each voronoi cell into preferred structure
            // to make it easier to manipulate and draw
            foreach (var vcell in vmap.GetVoronoiCells())
            {
                var mcell = new MapCell();
                mcell.Id = (uint)vcell.Index;

                // mcell.Shape = vcell.Points.ToMapPoints();
                foreach(var ipoint in vcell.Points)
                {
                    var mpoint = new MapPoint((int)ipoint.X, (int)ipoint.Y);
                    var key = (mpoint.X, mpoint.Y);

                    if (points.ContainsKey(key))
                    {
                        mpoint = points[key];
                    }
                    else
                    {
                        mpoint.Id = pointCounter++;
                        points[key] = mpoint;
                        pointRelationships[mpoint.Id] = new List<MapCell>();

                        // map.MapPoints.Add(mpoint);
                        // map.MapPointsById[mpoint.Id] = mpoint;
                        map.MapPoints.Add(mpoint.Id, mpoint);

                    }

                    mcell.MapPoints.Add(mpoint);
                    mcell.MapPointIds.Add(mpoint.Id);

                }

                var average = mcell.MapPoints.Average();

                mcell.X = average.X;
                mcell.Y = average.Y;
                mcell.Elevation = hmap[mcell.X][mcell.Y];
                mcell.Elevation = (float)Math.Round(mcell.Elevation, 4); // To make the number shorter when serialized

                map.MapCells.Add(mcell.Id, mcell); // Using Add() purposely on dictionary so it throws exception if same ID used twice
                // map.MapCells.Add(mcell);

                // Build a lookup table between voronoi points to
                // map cells so we can figure out which cells are
                // adjacent to each other later. The number
                // of points per shape is usually less than 10.
                for (int i = 0; i < mcell.MapPoints.Count; i++)
                {
                    var mp = mcell.MapPoints[i];
                    pointRelationships[mp.Id].Add(mcell);
                }
            }

            // Finally, use lookup table to link adjacent cells
            // together
            foreach (var members in pointRelationships.Values)
            {
                // We expect only 1-3 cells per members list
                for (int i = 0; i < members.Count - 1; i++)
                {
                    for (int j = 1; j < members.Count; j++)
                    {
                        var a = members[j];
                        var b = members[i];

                        if (!members[i].AdjacentMapCellIds.Contains(a.Id))
                        {
                            members[i].AdjacentMapCellIds.Add(a.Id);
                            members[i].AdjacentMapCells.Add(a);
                        }

                        if (!members[j].AdjacentMapCellIds.Contains(b.Id))
                        {
                            members[j].AdjacentMapCellIds.Add(b.Id);
                            members[j].AdjacentMapCells.Add(b);
                        }
                    }
                }
            }
        }
    
        private void DefineMapAttributes(Map map)
        {
            foreach(var cell in map.MapCells.Values)
            {
                if(cell.Elevation < map.LandThreshold)
                {
                    cell.Attributes.Add(MapAttribute.Water);
                }
                else
                {
                    cell.Attributes.Add(MapAttribute.Land);
                }
            }

            //foreach(var landCell in map.Cells.Values.Where(x => x.IsLand))
            foreach (var landCell in map.MapCells.Values)
            {

                bool isCoast = landCell.IsLand && landCell.AdjacentMapCellIds.Any(id => map.MapCells[id].IsWater);

                if(isCoast)
                {
                    landCell.Attributes.Add(MapAttribute.Coast);
                }

            }

        }
    
    }
}
