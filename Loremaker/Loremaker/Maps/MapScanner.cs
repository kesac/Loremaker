﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class MapScanner
    {
        private Map Map;
        private Random Random;

        public MapScanner(Map map)
        {
            this.Map = map;
            this.Random = new Random();
        }

        public List<Landmass> FindLandmasses()
        {
            var result = new List<Landmass>();

            var unprocessed = new List<MapCell>();
            unprocessed.AddRange(this.Map.Cells.Values.Where(x => x.IsLand));

            while(unprocessed.Count > 0)
            {
                var root = unprocessed.RemoveRandom<MapCell>();
                var scanned = new List<MapCell>() { root };

                var adjacencies = new List<MapCell>();
                adjacencies.AddRange(root.AdjacentCellIds
                            .Select(x => this.Map.Cells[x])
                            .Where(x => x.IsLand));

                while(adjacencies.Count > 0)
                {
                    var landcell = adjacencies[adjacencies.Count - 1];
                    scanned.Add(landcell);
                    adjacencies.Remove(landcell);
                    unprocessed.Remove(landcell);

                    adjacencies.AddRange(landcell.AdjacentCellIds
                                .Select(x => this.Map.Cells[x])
                                .Where(x => x.IsLand && !scanned.Contains(x)));
                }

                scanned.AddRange(adjacencies);

                var landmass = new Landmass() { MapCells = scanned };

                landmass.Center = new MapPoint()
                {
                    X = (int)landmass.MapCells.Average(x => x.Center.X),
                    Y = (int)landmass.MapCells.Average(x => x.Center.Y)
                };

                result.Add(landmass);
            }

            return result;

        }

    }
}
