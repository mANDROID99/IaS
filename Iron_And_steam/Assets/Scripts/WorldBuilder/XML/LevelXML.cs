using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;
using IaS.WorldBuilder.XML.mappers;
using UnityEditor.Callbacks;

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
        public readonly GroupXML[] Groups;
        public readonly TrackXML StartTrack;
        public readonly TrackXML EndTrack;

        public static LevelXML FromElement(XElement element)
        {
            if (element == null) throw new Exception(string.Format("Element {0} was not found", ElementLevel));

            Dictionary<string, int> counts = new Dictionary<string, int>();
            GroupXML[] groups = element.Element(ElementGroups).Elements(GroupXML.ElementGroup).Select(xGroup => GroupXML.FromElement(xGroup, counts)).ToArray();
            TrackXML startTrack = XmlValueMapper.FromElementValue(element, ElementStartTrack).AsReference(allTracks(groups)).MandatoryValue();
            TrackXML endTrack = XmlValueMapper.FromElementValue(element, ElementEndTrack).AsReference(allTracks(groups)).MandatoryValue();
            string id = XmlValueMapper.FromAttribute(element, AttributeLevelId).MandatoryValue();

            PostProcess(groups);
            return new LevelXML(id, groups, startTrack, endTrack);
        }

        private static void PostProcess(GroupXML[] groups)
        {
            foreach (GroupXML group in groups)
            {
                if (group.SplitAttachmentStr != null)
                {
                    SplitAttachMapper.Create(group.Id, group.SplitAttachmentStr, groups.SelectMany(g => g.Splits));
                }
            }
            
        }


        private static IEnumerable<TrackXML> allTracks(GroupXML[] groups)
        {
            return groups.SelectMany(g => g.Tracks);
        } 

        public LevelXML(string levelId, GroupXML[] groups, TrackXML startTrack, TrackXML endTrack)
        {
            LevelId = levelId;
            StartTrack = startTrack;
            EndTrack = endTrack;
            Groups = groups;
        }
    }
}
