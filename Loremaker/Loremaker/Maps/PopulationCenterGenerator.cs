﻿using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class PopulationCenterGenerator : IGenerator<List<City>>
    {
        private List<Continent> Landmasses;
        private IGenerator<string> NameGenerator;

        public PopulationCenterGenerator(List<Continent> landmasses, IGenerator<string> nameGenerator)
        {
            this.Landmasses = new List<Continent>();
            this.Landmasses.AddRange(landmasses);
            this.NameGenerator = nameGenerator;
        }

        private List<City> NextPopulation(Func<Continent,bool> landmassCondition, Func<Continent,int> maxSize, Func<MapCell,bool> mapCellCondition)
        {
            var result = new List<City>();

            var offlimits = new List<uint>();
            var maxTries = 100;

            foreach (var landmass in this.Landmasses.Where(x => landmassCondition(x)))
            {
                var targetCells = landmass.MapCells.Where(x => mapCellCondition(x)).ToList();
                
                if(targetCells.Count == 0)
                {
                    targetCells = landmass.MapCells.Where(x => x.IsLand).ToList();
                }
                
                var pops = maxSize(landmass);

                for (int i = 0; i < pops; i++)
                {
                    var homecell = targetCells.GetRandom();
                    var tries = 1;

                    while (offlimits.Contains(homecell.Id) && tries < maxTries)
                    {
                        homecell = targetCells.GetRandom();
                        tries++;
                    }

                    if (tries >= maxTries) continue;

                    // ID of PopulationCenters are done inside Next()
                    var pc = new City()
                    {
                        MapCell = homecell,
                        MapCellId = homecell.Id,
                        Name = this.NameGenerator.Next(),
                        Type = CityType.Town,
                        Landmass = landmass, // backreference
                    };

                    offlimits.Add(homecell.Id);
                    offlimits.AddRange(homecell.AdjacentMapCellIds);
                    offlimits.AddRange(homecell.AdjacentMapCells.SelectMany(x => x.AdjacentMapCellIds));
                    result.Add(pc);
                }

            }

            return result;
        }

        // TODO: Simplify
        public List<City> Next()
        {
            var result = new List<City>();

            var coastalPopulations = this.NextPopulation(
                    landmass => landmass.Size > 2,
                    landmass => (int)Math.Max(1, landmass.Size/200 + Math.Log(landmass.Size)/2),
                    mapcell => mapcell.IsCoast);

            var landlockedPopulations = this.NextPopulation(
                    landmass => landmass.Size > 10,
                    landmass => (int)Math.Max(0, landmass.Size/150),
                    mapcell => mapcell.IsLand && !mapcell.IsCoast);

            uint id = 0;

            foreach(var p in coastalPopulations)
            {
                p.Id = id++;
            }

            foreach(var p in landlockedPopulations)
            {
                p.Id = id++;
            }

            result.AddRange(coastalPopulations);
            result.AddRange(landlockedPopulations);

            return result;
        }
    }
}
