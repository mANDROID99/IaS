using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using IaS.WorldBuilder.Meshes;

namespace IaS.WorldBuilder
{
	public interface MeshSource{
		void Build(Vector3 pos, AdjacencyMatrix adjacencyMatrix, MeshBlock block, MeshBuilder meshBuilder, BlockBounds clipBounds);

        int GetShapeToOcclude(int[] direction);

        bool OccludesShape(int[] direction, int occludedShape);
	}
}
