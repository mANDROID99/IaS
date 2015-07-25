using UnityEngine;

namespace IaS.WorldBuilder.Meshes
{
    public class BlockOuterEdgeMeshGenerator : IProcMeshGenerator
    {
        public void BuildMesh(string partName, AdjacencyMatrix adjMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {
            SlopedMeshBuilder slopeBuilder = new SlopedMeshBuilder(meshBuilder);
	
			if (MeshPartsData.PART_OUTER_EDGE_FRONT.Equals (partName)) {

                slopeBuilder.ConstructSlopedFront(slopeBuilder.GetOuterSlopePoints());
			}else if (MeshPartsData.PART_OUTER_EDGE_LEFT.Equals (partName)) 
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(-1, 1, 1));
                slopeBuilder.ConstructSlopedSide(false, mat, slopeBuilder.GetOuterSlopePoints());
			}else if (MeshPartsData.PART_OUTER_EDGE_RIGHT.Equals (partName)) 
            {
                Matrix4x4 mat = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
                slopeBuilder.ConstructSlopedSide(true, mat, slopeBuilder.GetOuterSlopePoints());
			}
        }
    }
}
