using System;
using UnityEngine;
using System.Collections.Generic;

namespace IaS.WorldBuilder.Meshes
{
	public interface ProcMeshGenerator
	{
		void BuildMesh(String partName, AdjacencyMatrix adjacencyMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds);
	}

	public class ProcMeshGeneratorFactory
	{
		private static Dictionary<Type, ProcMeshGenerator> instances = new Dictionary<Type, ProcMeshGenerator>();

		public static void ClearInstances(){
			instances = new Dictionary<Type, ProcMeshGenerator>();
		}

		public static ProcMeshGenerator Get(Type t)
		{
			if (instances.ContainsKey (t)) {
				return instances[t];
			}
			
			ProcMeshGenerator instance = null;
			if (t == typeof(BlockOuterEdgeMeshGenerator)) 
            {
				instance = new BlockOuterEdgeMeshGenerator ();
			} else if (t == typeof(BlockSideMeshGenerator)) 
            {
				instance = new BlockSideMeshGenerator ();
			} else if (t == typeof(BlockCornerMeshGenerator))
            {
				instance = new BlockCornerMeshGenerator ();
            }
            else if (t == typeof(BlockInnerEdgeMeshGenerator)) {
                instance = new BlockInnerEdgeMeshGenerator();
            }

			instances.Add (t, instance);
			return instance;
		}
	}
}

