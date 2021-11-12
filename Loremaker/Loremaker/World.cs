using Loremaker.Maps;
using System;
using System.Collections.Generic;

namespace Loremaker
{
    public class World : Identifiable
    {
        public uint Id { get; set; }
        public string Name { get;  set; }
        public string Description { get; set; }
        public Map Map { get; set; }

        public World()
        {
            
        }

    }
}
