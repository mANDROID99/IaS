using System.Collections.Generic;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class TrackNodeXML
    {
        public const string ElementTrackNode = "node";
        private const string AttrTrackNodeId = "id";
        private const string AttrTrackNodePosition = "p";

        public Vector3 Position { get; private set; }
        public string Id { get; private set; }
        public TrackNodeXML Previous { get; private set; }
        public TrackNodeXML Next { get; private set; }

        public static TrackNodeXML FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueResult<string>.FromAttribute(element, AttrTrackNodeId).AsIdValue("t_node", counts);
            Vector3 position = XmlValueResult<string>.FromAttribute(element, AttrTrackNodePosition).AsVector3().MandatoryValue();
            return new TrackNodeXML(id, position);
        }

        public TrackNodeXML(string id, Vector3 position)
        {
            Id = id;
            Position = position;
        }
       
    }
}
