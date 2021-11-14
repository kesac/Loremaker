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
        private IGenerator<MapPoint[]> PointsGenerator;
        private IGenerator<VoronoiMap> VoronoiMapGenerator;

        public MapGenerator() : this(1000, 1000, 0.30f) { }

        public MapGenerator(int width, int height, float landThreshold)
        {
            this.UsingDimension(width, height);
            this.UsingLandThreshold(landThreshold);

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

            return this;
        }

        public MapGenerator UsingLandThreshold(float threshold)
        {
            this.ForProperty<float>(x => x.LandThreshold, threshold);
            return this;
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
            var hmap = this.HeightMapGenerator.Next();
            // map.HeightMap = hmap;

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

                        map.MapPoints.Add(mpoint);
                        map.MapPointsById[mpoint.Id] = mpoint;

                    }

                    mcell.MapPoints.Add(mpoint);
                    mcell.MapPointIds.Add(mpoint.Id);

                }

                mcell.Center = mcell.MapPoints.Average();
                mcell.Elevation = hmap[mcell.Center.X][mcell.Center.Y];
                mcell.Elevation = Math.Round(mcell.Elevation, 4); // To make the number shorter when serialized

                map.MapCellsById.Add(mcell.Id, mcell); // Using Add() purposely on dictionary so it throws exception if same ID used twice
                map.MapCells.Add(mcell);

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
            foreach(var cell in map.MapCellsById.Values)
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
            foreach (var landCell in map.MapCellsById.Values)
            {

                bool isCoast = landCell.IsLand && landCell.AdjacentMapCellIds.Any(id => map.MapCellsById[id].IsWater);

                if(isCoast)
                {
                    landCell.Attributes.Add(MapAttribute.Coast);
                }

            }

        }
    
    }
}
