using UnityEngine;

namespace IaS.Domain.Meshes
{
    public class BlockOuterEdgeMeshGenerator : IProcMeshGenerator
    {
        public void BuildMesh(PartType partType, AdjacencyMatrix adjMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {
            SlopedMeshBuilder slopeBuilder = new SlopedMeshBuilder(meshBuilder);
	
			if (PartType.OuterEdgeFront == partType) {

                slopeBuilder.ConstructSlopedFront(slopeBuilder.GetOuterSlopePoints());
			}else if (PartType.OuterEdgeLeft == partType) 
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(-1, 1, 1));
                slopeBuilder.ConstructSlopedSide(false, mat, slopeBuilder.GetOuterSlopePoints());
			}else if (PartType.OuterEdgeRight == partType) 
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
                slopeBuilder.ConstructSlopedSide(true, mat, slopeBuilder.GetOuterSlopePoints());
			}
        }
    }
}
