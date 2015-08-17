using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class TrackXML : IXmlReferenceable
    {
        public const string ElementTrack = "track";
        private const string AttrTrackId = "id";
        private const string AttrTrackDown = "down";
        private const string AttrTrackStartDirection = "startdir";

        public readonly string Id;
        public readonly TrackNodeXML[] Nodes;
        public readonly Vector3 Down;
        public readonly Vector3? StartDir;

        public static TrackXML FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrTrackId).AsIdValue("track", counts);
            Vector3 down = XmlValueMapper.FromAttribute(element, AttrTrackDown).AsDirection().MandatoryValue();
            Vector3? startDir = XmlValueMapper.FromAttribute(element, AttrTrackStartDirection).AsDirection().OptionalStruct<Vector3>();
            TrackNodeXML[] nodes = element.Elements(TrackNodeXML.ElementTrackNode).Select(xNode => TrackNodeXML.FromElement(xNode, counts)).ToArray();
            return new TrackXML(id, down, startDir, nodes);
        }

        public TrackXML(string id, Vector3 down, Vector3? startDir, TrackNodeXML[] nodes)
        {
            Id = id;
            Nodes = nodes;
            Down = down;
            StartDir = startDir;
        }

        public Vector3 EndForwardDTO
        {
            get
            {
                TrackNodeXML lastNode = Nodes[Nodes.Length - 1];
                return (lastNode.Position - lastNode.Previous.Position).normalized;
            }
        }

        public Vector3 StartForwardDTO
        {
            get
            {
                TrackNodeXML firstNode = Nodes[0];
                return (firstNode.Next.Position - firstNode.Position);
            }
        }


        public string GetId()
        {
            return Id;
        }
    }
}
