using IaS.Domain.Meshes;
using UnityEngine;

namespace IaS.Domain
{
	public interface IMeshSource{
		void Build(Vector3 pos, AdjacencyMatrix adjacencyMatrix, MeshBlock block, MeshBuilder meshBuilder, BlockBounds clipBounds);

        int GetShapeToOcclude(int[] direction);

        bool OccludesShape(int[] direction, int occludedShape);
	}
}
