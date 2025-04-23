using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class TerritoryGenerator : IGenerator<List<Region>>
    {

        private World World;
        private IGenerator<string> TerritoryNameGenerator;

        public int MinimumTerritorySize { get; set; }
        public int MaximumTerritorySize { get; set; }

        public TerritoryGenerator(World world, IGenerator<string> nameGenerator)
        {
            this.World = world;
            this.TerritoryNameGenerator = nameGenerator;
            this.MinimumTerritorySize = 10;
            this.MaximumTerritorySize = 500;
        }

        // New iteration of algorithm:
        // Each landmass at or larger than MinimumLandmassSize is automatically its own territory
        // Each landmass below MinimumLandmassSize is assigned to the closest territory
        // For landmasses with a certain size, subdivide into smaller territories using
        //   similar method of growth as last algorithm

        public List<Region> Next()
        {
            var result = new List<Region>();

            // Each landmass of MinimumLandmassSize or more is automatically its own territory, at
            // least initially before any subdivision occurs
            foreach (var landmass in this.World.Continents.Values.Where(x => x.Size >= this.MinimumTerritorySize))
            {
                var territory = new Region();

                territory.Name = this.TerritoryNameGenerator.Next();
                territory.MapCellIds.AddRange(landmass.MapCellIds);
                territory.MapCells.AddRange(landmass.MapCells);
                territory.X = (int)landmass.MapCells.Average(cell => cell.X);
                territory.Y = (int)landmass.MapCells.Average(cell => cell.Y);

                result.Add(territory);
            }

            // Convert really large territories into smaller ones
            while (result.Where(x => x.MapCells.Count > this.MaximumTerritorySize).FirstOrDefault() != null)
            {
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    var territory = result[i];
                    if (territory.MapCells.Count > this.MaximumTerritorySize)
                    {
                        var subdivisions = this.Subdivide(territory);
                        result.Remove(territory);
                        result.AddRange(subdivisions);
                    }
                }
            }

            // Each landmass of size under 3 is assigned to the closest territory
            // Note that if there aren't any landmasses of size 3 or greater this will
            // result in territory-less islands
            foreach (var landmass in this.World.Continents.Values.Where(x => x.Size < this.MinimumTerritorySize))
            {
                var closestTerritory = result.FirstOrDefault();

                if (closestTerritory != null)
                {
                    // var closestDistance = landmass.Center.Distance(closestTerritory.Center);
                    var closestDistance = landmass.MapCells.ShortestCellDistance(closestTerritory.MapCells);

                    foreach (var territory in result)
                    {
                        var distance = landmass.MapCells.ShortestCellDistance(territory.MapCells);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTerritory = territory;
                        }
                    }

                    closestTerritory.MapCellIds.AddRange(landmass.MapCellIds);
                    closestTerritory.MapCells.AddRange(landmass.MapCells);
                }
            }



            uint id = 0;
            foreach(var territory in result)
            {
                // territory.ContainingMapPointIds.AddRange(this.IdentifyContainingMapPointIds(territory));
                territory.Id = id++;
            }

            return result;
        }

        private Region[] Subdivide(Region original, int divisions = 2)
        {

            if(original.MapCells.Count < divisions)
            {
                throw new InvalidOperationException("The number of divisions cannot exceed the number of cells in the territory.");
            }

            var unclaimed = new List<uint>(); // For RemoveRandom()
            unclaimed.AddRange(original.MapCellIds);

            var territories = Enumerable.Range(0, divisions).Select(_ => new Region()).ToArray();
            var adjacencies = Enumerable.Range(0, divisions).Select(_ => new List<uint>()).ToArray();

            for (int i = 0; i < divisions; i++)
            {
                var rootId = unclaimed.RemoveRandom();
                var cell = this.World.Map.MapCells[rootId];
                territories[i].MapCells.Add(cell);
                territories[i].MapCellIds.Add(cell.Id);
                adjacencies[i].AddRange(cell.AdjacentMapCellIds.Where(id => unclaimed.Contains(id)));
            }

            while(unclaimed.Count > 0)
            {
                bool expanded = false;

                for (int i = 0; i < divisions && unclaimed.Count > 0; i++)
                {
                    if(Chance.Roll(0.5))
                    {
                        continue;
                    }

                    if(adjacencies[i].Count > 0)
                    {
                        var newAdjacencies = new List<uint>();

                        foreach(var adjacentId in adjacencies[i])
                        {
                            if (unclaimed.Contains(adjacentId))
                            {
                                unclaimed.Remove(adjacentId);
                                var adjacentCell = this.World.Map.MapCells[adjacentId];

                                territories[i].MapCells.Add(adjacentCell);
                                territories[i].MapCellIds.Add(adjacentId);
                                expanded = true;

                                foreach (var id in adjacentCell.AdjacentMapCellIds)
                                {
                                    if (unclaimed.Contains(id))
                                    {
                                        newAdjacencies.Add(id);
                                    }
                                }
                            }
                        }

                        adjacencies[i] = newAdjacencies;   
                    }
                }

                if(!expanded)
                {
                    // break;
                }

            }


            return territories;
        }



    }
}
