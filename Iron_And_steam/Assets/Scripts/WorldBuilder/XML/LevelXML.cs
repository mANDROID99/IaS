using System.Collections.Generic;

namespace IaS.WorldBuilder.Xml
{
    public class LevelXML
    {
        public readonly string LevelId;
        public readonly List<LevelGroupXML> Groups = new List<LevelGroupXML>();

        public LevelXML(string levelId)
        {
            LevelId = levelId;
        }
    }
}
