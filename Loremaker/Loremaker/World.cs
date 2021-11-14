using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Loremaker
{
    public class World : Identifiable
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Map Map { get; set; }
        public Dictionary<uint, Landmass> Landmasses { get; set; }
        public Dictionary<uint, PopulationCenter> PopulationCenters { get; set; }
        public Dictionary<uint, Territory> Territories { get; set; }

        public World()
        {
            this.Landmasses = new Dictionary<uint, Landmass>();
            this.PopulationCenters = new Dictionary<uint, PopulationCenter>();
            this.Territories = new Dictionary<uint, Territory>();
        }

        private MapCell FindMapCell(PopulationCenter p)
        {
            return this.Map.MapCells[p.MapCellId];
        }

        private Landmass FindLandmass(PopulationCenter p)
        {
            foreach(var landmass in Landmasses.Values)
            {
                if(landmass.MapCellIds.Contains(p.MapCellId))
                {
                    return landmass;
                }
            }

            return null;
        }

        private Territory FindTerritory(PopulationCenter p)
        {
            throw new NotImplementedException();
        }

        public static void Serialize(World world, string filepath)
        {
            var json = JsonSerializer.Serialize(world);
            File.WriteAllText(filepath, json);
        }

        public static World Deserialize(string filepath)
        {
            var json = File.ReadAllText(filepath);
            var world = JsonSerializer.Deserialize<World>(json);

            // Restore back references
            foreach (var cell in world.Map.MapCells.Values)
            {
                var points = cell.MapPointIds.Select(id => world.Map.MapPoints[id]);
                cell.MapPoints.AddRange(points);

                var cells = cell.AdjacentMapCellIds.Select(id => world.Map.MapCells[id]);
                cell.AdjacentMapCells.AddRange(cells);
            }

            foreach (var landmass in world.Landmasses.Values)
            {
                var cells = landmass.MapCellIds.Select(id => world.Map.MapCells[id]);
                landmass.MapCells.AddRange(cells);
            }

            foreach (var pop in world.PopulationCenters.Values)
            {
                pop.MapCell = world.FindMapCell(pop);
                pop.Landmass = world.FindLandmass(pop);
                // pop.Territory = world.FindTerritory(pop);
            }

            foreach (var territory in world.Territories.Values)
            {
                var cells = territory.MapCellIds.Select(id => world.Map.MapCells[id]);
                territory.MapCells.AddRange(cells);
            }
            

            return world;
        }

    }
}
