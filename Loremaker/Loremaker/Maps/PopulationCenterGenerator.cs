using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loremaker.Maps
{
    public class PopulationCenterGenerator : IGenerator<List<PopulationCenter>>
    {
        private List<Landmass> Landmasses;
        private IGenerator<string> NameGenerator;

        public PopulationCenterGenerator(List<Landmass> landmasses, IGenerator<string> nameGenerator)
        {
            this.Landmasses = new List<Landmass>();
            this.Landmasses.AddRange(landmasses);
            this.NameGenerator = nameGenerator;
        }

        public List<PopulationCenter> Next()
        {
            var result = new List<PopulationCenter>();

            uint count = 0;

            foreach(var landmass in this.Landmasses)
            {
                var coasts = landmass.MapCells.Where(x => x.IsCoast).ToList();
                var pops = Math.Max(1, landmass.MapCells.Count / 100);
                
                for (int i = 0; i < pops; i++)
                {
                    var homecell = coasts.FindRandom<MapCell>();

                    var pc = new PopulationCenter()
                    {
                        MapCell = homecell,
                        MapCellId = homecell.Id,
                        Name = this.NameGenerator.Next(),
                        Type = PopulationCenterType.Town,
                        Id = count++
                    };

                    result.Add(pc);
                }
                
            }

            return result;
        }
    }
}
