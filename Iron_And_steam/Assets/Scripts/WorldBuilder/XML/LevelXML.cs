using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;

namespace IaS.WorldBuilder.Xml
{
    public class LevelXML
    {
        public const string ElementLevel = "level";
        private const string ElementGroups = "groups";
        private const string ElementStartTrack = "start";
        private const string ElementEndTrack = "end";
        private const string AttributeLevelId = "id";

        public readonly string LevelId;
        public readonly LevelGroupXML[] Groups;
        public readonly TrackXML StartTrack;
        public readonly TrackXML EndTrack;

        public static LevelXML FromElement(XElement element)
        {
            if (element == null) throw new Exception(string.Format("Element {0} was not found", ElementLevel));

            Dictionary<string, int> counts = new Dictionary<string, int>();
            LevelGroupXML[] groups = element.Element(ElementGroups).Elements(LevelGroupXML.ElementGroup).Select(xGroup => LevelGroupXML.FromElement(xGroup, counts)).ToArray();
            TrackXML startTrack = XmlValueMapper.FromElementValue(element, ElementStartTrack).AsReference(allTracks(groups)).MandatoryValue();
            TrackXML endTrack = XmlValueMapper.FromElementValue(element, ElementEndTrack).AsReference(allTracks(groups)).MandatoryValue();
            string id = XmlValueMapper.FromAttribute(element, AttributeLevelId).MandatoryValue();
            return new LevelXML(id, groups, startTrack, endTrack);
        }

        private static IEnumerable<TrackXML> allTracks(LevelGroupXML[] groups)
        {
            return groups.SelectMany(g => g.Tracks);
        } 

        public LevelXML(string levelId, LevelGroupXML[] groups, TrackXML startTrack, TrackXML endTrack)
        {
            LevelId = levelId;
            StartTrack = startTrack;
            EndTrack = endTrack;
            Groups = groups;
        }
    }
}
