using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.Domain.XML;
using UnityEngine;

namespace IaS.Xml
{
    public class Track : IReferenceable
    {
        public const string ElementTrack = "track";
        private const string AttrTrackId = "id";
        private const string AttrTrackDown = "down";
        private const string AttrTrackStartDirection = "startdir";

        public readonly string Id;
        public readonly XmlTrackNode[] Nodes;
        public readonly Vector3 Down;
        public readonly Vector3? StartDir;

        public static Track FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrTrackId).AsIdValue("track", counts);
            Vector3 down = XmlValueMapper.FromAttribute(element, AttrTrackDown).AsDirection().MandatoryValue();
            Vector3? startDir = XmlValueMapper.FromAttribute(element, AttrTrackStartDirection).AsDirection().OptionalStruct<Vector3>();
            XmlTrackNode[] nodes = element.Elements(XmlTrackNode.ElementTrackNode).Select(xNode => XmlTrackNode.FromElement(xNode, counts)).ToArray();
            return new Track(id, down, startDir, nodes);
        }

        public Track(string id, Vector3 down, Vector3? startDir, XmlTrackNode[] nodes)
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
                XmlTrackNode lastNode = Nodes[Nodes.Length - 1];
                return (lastNode.Position - lastNode.Previous.Position).normalized;
            }
        }

        public Vector3 StartForwardDTO
        {
            get
            {
                XmlTrackNode firstNode = Nodes[0];
                return (firstNode.Next.Position - firstNode.Position);
            }
        }


        public string GetId()
        {
            return Id;
        }
    }
}
