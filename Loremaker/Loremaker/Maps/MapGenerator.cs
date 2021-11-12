using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class MapGenerator : Generator<Map>
    {
        private IGenerator<double[,]> HeightMapGenerator;
        private IGenerator<VoronoiMap> VoronoiMapGenerator;

        public MapGenerator() : this(1000, 1000, 0.30) { }

        public MapGenerator(int width, int height, double landThreshold)
        {
            var points = new MapPointsGenerator(width, height, 25);
            this.VoronoiMapGenerator = new VoronoiMapGenerator(points);
            this.HeightMapGenerator = new IslandHeightMapGenerator(width, height);

            this.UsingDimension(width, height);
            this.UsingLandThreshold(landThreshold);

            this.ForEach(x =>
            {
                this.PopulateMap(x);
                this.PopulateMapAttributes(x);
            });
        }

        public MapGenerator UsingDimension(int width, int height)
        {
            this.ForProperty<int>(x => x.Width, width);
            this.ForProperty<int>(x => x.Height, height);
            return this;
        }

        public MapGenerator UsingLandThreshold(double threshold)
        {
            this.ForProperty<double>(x => x.LandThreshold, threshold);
            return this;
        }

        private void PopulateMap(Map map)
        {
            var pointRelationships = new Dictionary<(int, int), List<MapCell>>();

            // Construct voronoi map which is a 2D space
            // divided into discrete polygons or cells
            var vmap = this.VoronoiMapGenerator.Next();

            // Construct a height map which will be used
            // to assign elevation to each of the map cells
            var hmap = this.HeightMapGenerator.Next();
            map.HeightMap = hmap;

            // Transform each voronoi cell into preferred structure
            // to make it easier to manipulate and draw
            foreach (var vcell in vmap.GetVoronoiCells())
            {
                var mcell = new MapCell();
                mcell.Id = (uint)vcell.Index;
                mcell.Shape = vcell.Points.ToMapPoints();
                mcell.Center = mcell.Shape.Average();
                mcell.Elevation = hmap[mcell.Center.X, mcell.Center.Y];

                map.Cells.Add(mcell.Id, mcell); // Using Add() purposely so it throws exception if same ID used twice

                // Build a lookup table between vornoi points to
                // map cells so we can figure out which cells are
                // adjacent to each other later. The number
                // of points per shape is usually less than 10.
                for (int i = 0; i < mcell.Shape.Count; i++)
                {
                    var mp = mcell.Shape[i];
                    var key = (mp.X, mp.Y);

                    if (pointRelationships.ContainsKey(key))
                    {
                        pointRelationships[key].Add(mcell);
                    }
                    else
                    {
                        pointRelationships[key] = new List<MapCell>() { mcell };
                    }
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
    
        private void PopulateMapAttributes(Map map)
        {
            foreach(var cell in map.Cells.Values)
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
            foreach (var landCell in map.Cells.Values)
            {

                bool isCoast = landCell.IsLand && landCell.AdjacentMapCellIds.Any(id => map.Cells[id].IsWater);

                if(isCoast)
                {
                    landCell.Attributes.Add(MapAttribute.Coast);
                }

            }

        }
    
    }
}
