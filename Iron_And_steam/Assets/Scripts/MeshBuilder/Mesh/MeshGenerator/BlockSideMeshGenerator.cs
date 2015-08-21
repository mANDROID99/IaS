using UnityEngine;

namespace IaS.Domain.Meshes
{
	public class BlockSideMeshGenerator : IProcMeshGenerator {

		public void BuildMesh(PartType partType, AdjacencyMatrix adjMatrix,  MeshBuilder meshBuilder, BlockBounds clipBounds)
		{
			if (PartType.BlockFront == partType) {
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 0), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 0), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 0), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 0), new Vector2(0, 0), true));;
			} 
			else if (PartType.BlockBack == partType) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 1), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 1), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 1), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 1), new Vector2(0, 0), true));
			} 
			else if (PartType.BlockRight == partType) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 1), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 1), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 0), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 0, 0), new Vector2(0, 0), true));
			} 
			else if (PartType.BlockLeft == partType) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 0), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 0), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 1), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 0, 1), new Vector2(0, 0), true));
			} 
			else if (PartType.BlockTop == partType) 
			{
				meshBuilder.AddTriangleStrip(
					false, 
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 1), new Vector2(1, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(1, 1, 0), new Vector2(1, 0), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 1), new Vector2(0, 1), true),
					meshBuilder.VertAutoNormal(new Vector3(0, 1, 0), new Vector2(0, 0), true));
			} 
			else if (PartType.BlockBottom == partType) 
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
