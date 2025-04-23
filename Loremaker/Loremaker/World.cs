using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Loremaker
{
    public class World : IEntity
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Map Map { get; set; }
        public Dictionary<uint, Continent> Continents { get; set; }
        public Dictionary<uint, City> Cities { get; set; }
        public Dictionary<uint, Region> Regions { get; set; }

        public World()
        {
            this.Continents = new Dictionary<uint, Continent>();
            this.Cities = new Dictionary<uint, City>();
            this.Regions = new Dictionary<uint, Region>();
        }

        private MapCell FindMapCell(City p)
        {
            return this.Map.MapCells[p.MapCellId];
        }

        private Continent FindLandmass(City p)
        {
            foreach(var landmass in Continents.Values)
            {
                if(landmass.MapCellIds.Contains(p.MapCellId))
                {
                    return landmass;
                }
            }

            return null;
        }

        private Region FindTerritory(City p)
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

            foreach (var landmass in world.Continents.Values)
            {
                var cells = landmass.MapCellIds.Select(id => world.Map.MapCells[id]);
                landmass.MapCells.AddRange(cells);
            }

            foreach (var pop in world.Cities.Values)
            {
                pop.MapCell = world.FindMapCell(pop);
                pop.Landmass = world.FindLandmass(pop);
                // pop.Territory = world.FindTerritory(pop);
            }

            foreach (var territory in world.Regions.Values)
            {
                var cells = territory.MapCellIds.Select(id => world.Map.MapCells[id]);
                territory.MapCells.AddRange(cells);
            }
            

            return world;
        }

    }
}
