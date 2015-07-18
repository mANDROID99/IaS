using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IaS.WorldBuilder.Meshes
{
	public class BlockInnerEdgeMeshGenerator : ProcMeshGenerator {

        public void BuildMesh(String partName, AdjacencyMatrix adjMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {
            SlopedMeshBuilder slopeBuilder = new SlopedMeshBuilder(meshBuilder);

            if (MeshPartsData.PART_INNER_EDGE_FRONT.Equals(partName))
            {
                slopeBuilder.ConstructSlopedFront(slopeBuilder.GetInnerSlopePoints());
            }else if (MeshPartsData.PART_INNER_EDGE_RIGHT.Equals(partName))
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
                slopeBuilder.ConstructSlopedSide(true, mat, slopeBuilder.GetInnerSlopePoints());
            }else if (MeshPartsData.PART_INNER_EDGE_LEFT.Equals(partName))
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(-1, 1, 1));
                slopeBuilder.ConstructSlopedSide(false, mat, slopeBuilder.GetInnerSlopePoints());
            }
        }
	}
}