using IaS.WorldBuilder.Meshes;
using UnityEngine;

namespace IaS.WorldBuilder
{
	public interface MeshSource{
		void Build(Vector3 pos, AdjacencyMatrix adjacencyMatrix, MeshBlock block, MeshBuilder meshBuilder, BlockBounds clipBounds);

        int GetShapeToOcclude(int[] direction);

        bool OccludesShape(int[] direction, int occludedShape);
	}
}
