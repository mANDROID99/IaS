using IaS.Domain.Meshes;
using UnityEngine;

namespace IaS.Domain
{

	public class TrackMeshSource : IMeshSource {

        public void Build(Vector3 pos, AdjacencyMatrix adjacencyMatrix, MeshBlock block, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {

        }

        public int GetShapeToOcclude(int[] direction)
        {
            return -1;
        }

        public bool OccludesShape(int[] direction, int occludedShape)
        {
            return false;
        }

	}
}
