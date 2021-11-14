using Loremaker.Maps;
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
        public List<PopulationCenter> PopulationCenters { get; set; }
        public List<Territory> Territories { get; set; }

        public World()
        {
            this.PopulationCenters = new List<PopulationCenter>();
            this.Territories = new List<Territory>();
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

            foreach (var point in world.Map.MapPoints)
            {
                world.Map.MapPointsById[point.Id] = point;
            }

            foreach (var cell in world.Map.MapCells)
            {
                world.Map.MapCellsById[cell.Id] = cell;
                var points = cell.MapPointIds.Select(id => world.Map.MapPointsById[id]);
                cell.MapPoints.AddRange(points);
            }

            // This next loop cannot run until MapCellsById is fully populated
            // in the previous loop
            foreach (var cell in world.Map.MapCellsById.Values)
            {
                var cells = cell.AdjacentMapCellIds.Select(id => world.Map.MapCellsById[id]);
                cell.AdjacentMapCells.AddRange(cells);
            }

            // More backreferences

            foreach (var landmass in world.Map.Landmasses)
            {
                var cells = landmass.MapCellIds.Select(id => world.Map.MapCellsById[id]);
                landmass.MapCells.AddRange(cells);

                world.Map.LandmassesById[landmass.Id] = landmass;
            }

            foreach (var pop in world.PopulationCenters)
            {
                pop.MapCell = world.Map.MapCellsById[pop.MapCellId];
                pop.Landmass = world.Map.LandmassesById[pop.LandmassId];
            }

            foreach (var territory in world.Territories)
            {
                var cells = territory.MapCellIds.Select(id => world.Map.MapCellsById[id]);
                territory.MapCells.AddRange(cells);
            }

            return world;
        }

    }
}
