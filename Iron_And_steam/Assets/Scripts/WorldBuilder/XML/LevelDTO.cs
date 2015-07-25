using System.Collections.Generic;

namespace IaS.WorldBuilder.Xml
{
    public class LevelDTO
    {
        public List<LevelGroupDTO> Groups { get; private set; }

        public LevelDTO()
        {
            Groups = new List<LevelGroupDTO>();
        }
    }
}
