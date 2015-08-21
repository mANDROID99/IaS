using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace IaS.Xml
{
    public class XmlTrackNode
    {
        public const string ElementTrackNode = "node";
        private const string AttrTrackNodeId = "id";
        private const string AttrTrackNodePosition = "p";

        public Vector3 Position { get; private set; }
        public string Id { get; private set; }
        public XmlTrackNode Previous { get; private set; }
        public XmlTrackNode Next { get; private set; }

        public static XmlTrackNode FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrTrackNodeId).AsIdValue("t_node", counts);
            Vector3 position = XmlValueMapper.FromAttribute(element, AttrTrackNodePosition).AsVector3().MandatoryValue();
            return new XmlTrackNode(id, position);
        }

        public XmlTrackNode(string id, Vector3 position)
        {
            Id = id;
            Position = position;
        }
       
    }
}
