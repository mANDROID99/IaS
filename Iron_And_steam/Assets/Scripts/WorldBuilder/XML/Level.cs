using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IaS.WorldBuilder.Xml
{
    public class Level
    {
        public List<LevelGroup> Groups { get; private set; }

        public Level()
        {
            this.Groups = new List<LevelGroup>();
        }
    }
}
