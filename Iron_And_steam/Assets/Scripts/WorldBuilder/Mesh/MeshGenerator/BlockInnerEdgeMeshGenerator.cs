using UnityEngine;

namespace IaS.WorldBuilder.Meshes
{
	public class BlockInnerEdgeMeshGenerator : IProcMeshGenerator {

        public void BuildMesh(PartType partType, AdjacencyMatrix adjMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {
            SlopedMeshBuilder slopeBuilder = new SlopedMeshBuilder(meshBuilder);

            if (partType == PartType.InnerEdgeFront)
            {
                slopeBuilder.ConstructSlopedFront(slopeBuilder.GetInnerSlopePoints());
            }else if (partType == PartType.InnerEdgeRight)
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
                slopeBuilder.ConstructSlopedSide(true, mat, slopeBuilder.GetInnerSlopePoints());
            }else if (partType == PartType.InnerEdgeLeft)
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(-1, 1, 1));
                slopeBuilder.ConstructSlopedSide(false, mat, slopeBuilder.GetInnerSlopePoints());
            }
        }
	}
}