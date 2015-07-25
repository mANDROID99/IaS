using UnityEngine;

namespace IaS.WorldBuilder.Meshes
{
	public class BlockSideMeshGenerator : IProcMeshGenerator {

		public void BuildMesh(string partName, AdjacencyMatrix adjMatrix,  MeshBuilder meshBuilder, BlockBounds clipBounds)
		{
			if (MeshPartsData.PART_BLOCK_FRONT.Equals (partName)) {
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 0), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 0), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 0), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 0), new Vector2(0, 0), true));;
			} 
			else if (MeshPartsData.PART_BLOCK_BACK.Equals (partName)) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 1), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 1), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 1), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 1), new Vector2(0, 0), true));
			} 
			else if (MeshPartsData.PART_BLOCK_RIGHT.Equals (partName)) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 1), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 1), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 0), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 0), new Vector2(0, 0), true));
			} 
			else if (MeshPartsData.PART_BLOCK_LEFT.Equals (partName)) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 0), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 0), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 1), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 1), new Vector2(0, 0), true));
			} 
			else if (MeshPartsData.PART_BLOCK_TOP.Equals (partName)) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 1), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 0), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 1), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 0), new Vector2(0, 0), true));
			} 
			else if (MeshPartsData.PART_BLOCK_BOTTOM.Equals (partName)) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 0), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 1), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 0), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 1), new Vector2(0, 0), true));
			}
		}
	}
}
