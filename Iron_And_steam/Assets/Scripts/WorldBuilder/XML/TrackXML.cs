using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class TrackXML : IXmlReferenceable
    {
        public readonly string Id;
        public readonly TrackNodeXML[] NodesXml;
        public readonly Vector3 Down;
        public readonly Vector3? StartDir;

        public TrackXML(string id, Vector3 down, Vector3? startDir, TrackNodeXML[] nodesXml)
        {
            Id = id;
            Down = down;
            NodesXml = nodesXml;
            StartDir = startDir;
        }


        public Vector3 EndForwardDTO
        {
            get
            {
                TrackNodeXML lastNode = NodesXml[NodesXml.Length - 1];
                return (lastNode.Position - lastNode.Previous.Position).normalized;
            }
        }

        public Vector3 StartForwardDTO
        {
            get
            {
                TrackNodeXML firstNode = NodesXml[0];
                return (firstNode.Next.Position - firstNode.Position);
            }
        }


        public string GetId()
        {
            return Id;
        }
    }
}
