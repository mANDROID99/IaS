namespace IaS.WorldBuilder.Meshes
{
	public interface IProcMeshGenerator
	{
		void BuildMesh(PartType partType, AdjacencyMatrix adjacencyMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds);
	}

//	public class ProcMeshGeneratorFactory
//	{
//		private static Dictionary<Type, IProcMeshGenerator> instances = new Dictionary<Type, IProcMeshGenerator>();
//
//		public static void ClearInstances(){
//			instances = new Dictionary<Type, IProcMeshGenerator>();
//		}
//
//		public static IProcMeshGenerator Get(Type t)
//		{
//			if (instances.ContainsKey (t)) {
//				return instances[t];
//			}
//			
//			IProcMeshGenerator instance = null;
//			if (t == typeof(BlockOuterEdgeMeshGenerator)) 
//            {
//				instance = new BlockOuterEdgeMeshGenerator ();
//			} else if (t == typeof(BlockSideMeshGenerator)) 
//            {
//				instance = new BlockSideMeshGenerator ();
//			} else if (t == typeof(BlockCornerMeshGenerator))
//            {
//				instance = new BlockCornerMeshGenerator ();
//            }
//            else if (t == typeof(BlockInnerEdgeMeshGenerator)) {
//                instance = new BlockInnerEdgeMeshGenerator();
//            }
//
//			instances.Add (t, instance);
//			return instance;
//		}
//	}
}

