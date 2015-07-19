using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Meshes;
using IaS.WorldBuilder;
using IaS.GameState;

namespace IaS.GameObjects
{
    public class BlockController : MonoBehaviour
    {

        public class BlockGameObjectBuilder
        {

            private MeshBlock block;
            private IList<MeshBlock> occlusions;
            private IList<Split> splits;
            private Transform parent;
            private GameObject prefab;

            public BlockGameObjectBuilder With(MeshBlock block, IList<MeshBlock> occlusions, Split[] splits, Transform parent, GameObject prefab)
            {
                this.block = block;
                this.occlusions = occlusions;
                this.splits = splits;
                this.parent = parent;
                this.prefab = prefab;
                return this;
            }

            public GameObject BuildGameObj(AdjacencyCalculator adjacencyCalculator)
            {
                MeshBuilder meshBuilder = new MeshBuilder();
                adjacencyCalculator.SetupNext(occlusions, splits, block);

                for (int x = (int)block.bounds.minX; x < block.bounds.maxX; x++)
                {
                    for (int y = (int)block.bounds.minY; y < block.bounds.maxY; y++)
                    {
                        for (int z = (int)block.bounds.minZ; z < block.bounds.maxZ; z++)
                        {
                            AdjacencyMatrix adjacencyMatrix = adjacencyCalculator.CalculateAdjacency(x, y, z);
                            if (adjacencyMatrix.IsVisible())
                            {
                                BlockBounds clipBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
                                clipBounds.ClipToBounds(splits);
                                block.meshSource.Build(new Vector3(x, y, z) - block.bounds.Position, adjacencyMatrix, block, meshBuilder, clipBounds);
                            }
                        }
                    }
                }

                GameObject blockGameObj = GameObjectUtils.AsChildOf(parent, block.bounds.Position, GameObject.Instantiate(prefab));
                //blockGameObj.AddComponent<BlockController>();
                blockGameObj.name = block.id;

                Mesh mesh = meshBuilder.DoneCreateMesh();
                blockGameObj.GetComponent<MeshFilter>().mesh = mesh;
                return blockGameObj;
            }

            public InstanceWrapper Build(AdjacencyCalculator adjacencyCalculator)
            {
                GameObject instance = BuildGameObj(adjacencyCalculator);
                return new InstanceWrapper(instance, block.rotatedBlockBounds);
            }

            
        }


        void Start()
        {
            // not used
        } 
    }
}
