using System.Linq;
using IaS.Domain;
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


        public Vector3 EndForwardDTO
        {
            get
            {
                TrackNodeDTO lastNode = NodesDto[NodesDto.Length - 1];
                return (lastNode.Position - lastNode.Previous.Position).normalized;
            }
        }

        public Vector3 StartForwardDTO
        {
            get
            {
                TrackNodeDTO firstNode = NodesDto[0];
                return (firstNode.Next.Position - firstNode.Position);
            }
        }


        public string GetId()
        {
            return Id;
        }
    }
}
