using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class TrackDTO : IXmlReferenceable
    {
        public readonly string Id;
        public readonly TrackNodeDTO[] NodesDto;
        public readonly Vector3 Down;
        public readonly Vector3? StartDir;

        public TrackDTO(string id, Vector3 down, Vector3? startDir, TrackNodeDTO[] nodesDto)
        {
            Id = id;
            Down = down;
            NodesDto = nodesDto;
            StartDir = startDir;
        }

        public string GetId()
        {
            return Id;
        }
    }
}
