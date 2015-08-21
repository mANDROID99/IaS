using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.Domain.XML;

namespace IaS.Xml
{
    public class XmlLevel
    {
        public const string ElementLevel = "level";
        private const string ElementGroups = "groups";
        private const string ElementStartTrack = "start";
        private const string ElementEndTrack = "end";
        private const string AttributeLevelId = "id";

        public readonly string LevelId;
        public readonly XmlGroup[] Groups;
        public readonly string StartTrackId;
        public readonly string EndTrackId;

        public static XmlLevel FromElement(XElement element)
        {
            if (element == null) throw new Exception(string.Format("Element {0} was not found", ElementLevel));

            Dictionary<string, int> counts = new Dictionary<string, int>();
            XmlGroup[] groups = element.Element(ElementGroups).Elements(XmlGroup.ElementGroup).Select(xGroup => XmlGroup.FromElement(xGroup, counts)).ToArray();
            string startTrackId = XmlValueMapper.FromElementValue(element, ElementStartTrack).AsText().MandatoryValue();
            string endTrackId = XmlValueMapper.FromElementValue(element, ElementEndTrack).AsText().MandatoryValue();

            string id = XmlValueMapper.FromAttribute(element, AttributeLevelId).MandatoryValue();
            return new XmlLevel(id, groups, startTrackId, endTrackId);
        }

        public XmlLevel(string levelId, XmlGroup[] groups, string startTrackId, string endTrackId)
        {
            LevelId = levelId;
            StartTrackId = startTrackId;
            EndTrackId = endTrackId;
            Groups = groups;
        }
    }
}
