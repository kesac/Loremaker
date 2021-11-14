using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class TerritoryGenerator : IGenerator<List<Territory>>
    {

        private World World;

        public TerritoryGenerator(World world)
        {
            this.World = world;
        }

        // TODO: Simplify
        public List<Territory> Next()
        {
            
            var unclaimedLand = new List<MapCell>();
            var populatedLandmasses = this.World.PopulationCenters.Values.Select(x => x.Landmass).Distinct();
            unclaimedLand.AddRange(this.World.Map.MapCells.Values.Where(x => populatedLandmasses.SelectMany(y => y.MapCellIds).Contains(x.Id)));

            var roots = GetRootPopulationCenters(populatedLandmasses.ToList());
            var territories = new Dictionary<uint, Territory>();
            var adjacencies = new Dictionary<uint, List<MapCell>>();
            uint territoryCount = 0;

            foreach(var capital in roots)
            {
                var territory = new Territory()
                {
                    Id = territoryCount++,
                    Name = "Territory"
                };

                unclaimedLand.Remove(capital.MapCell);
                territory.MapCells.Add(capital.MapCell);
                territory.MapCellIds.Add(capital.MapCellId);
                territories[territory.Id] = territory;
                adjacencies[territory.Id] = new List<MapCell>();
                adjacencies[territory.Id].AddRange(capital.MapCell.AdjacentMapCells);
            }

            while(unclaimedLand.Count > 0)
            {
                bool expanded = false;

                foreach(var territory in territories.Values)
                {
                    var adjacentCells = adjacencies[territory.Id];

                    if(adjacentCells.Count > 0)
                    {
                        var newAdjacencies = new List<MapCell>();

                        foreach (var adjacentCell in adjacentCells)
                        {
                            // Claim cell if not already claimed by another territory
                            if (unclaimedLand.Contains(adjacentCell))
                            {
                                unclaimedLand.Remove(adjacentCell);
                                territory.MapCells.Add(adjacentCell);
                                territory.MapCellIds.Add(adjacentCell.Id);
                                expanded = true;

                                // Mark more cells for future expansion, but do not claim yet
                                foreach (var adjacentAdjacent in adjacentCell.AdjacentMapCells)
                                {
                                    if (unclaimedLand.Contains(adjacentAdjacent))
                                    {
                                        newAdjacencies.Add(adjacentAdjacent);
                                    }
                                }

                            }
                        }

                        adjacencies[territory.Id] = newAdjacencies;
                    }

                }

                if(!expanded)
                {
                    break;
                }

            }

            return territories.Values.ToList();
        }

        // TODO: Simplify
        private List<PopulationCenter> GetRootPopulationCenters(List<Landmass> landmasses)
        {
            var result = new List<PopulationCenter>();

            foreach(var landmass in landmasses)
            {
                //var populationsByLandmass = this.World.PopulationCenters.Values.Where(x => x.LandmassId == landmass.Id);
                var populationsByLandmass = this.World.PopulationCenters.Values.Where(x => x.Landmass.Id == landmass.Id);

                var count = populationsByLandmass.Count();

                if (count > 2)
                {
                    var list = populationsByLandmass.ToList();

                    for(int i = 0; i < 3; i++)
                    {
                        result.Add(list.RemoveRandom<PopulationCenter>());
                    }
                }
                else
                {
                    result.Add(populationsByLandmass.First());
                }
            }

            return result;
        }
    }
}
