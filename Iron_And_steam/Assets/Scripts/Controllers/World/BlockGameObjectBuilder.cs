using System.Collections.Generic;
using System.Linq;
using IaS.GameObjects;
using IaS.GameState;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Meshes;
using UnityEngine;

namespace IaS.Controllers.World
{
    class BlockGameObjectBuilder
    {

        public InstanceWrapper[] BuildGameObjects(GroupContext groupCtx, Transform container, GameObject blockPrefab)
        {
            var adjacencyCalculator = new AdjacencyCalculator();
            return
                groupCtx.MeshBlocks.Select(
                    block =>
                        BuildGameObj(block, groupCtx.MeshBlocks, groupCtx.Splits, container.transform, blockPrefab, adjacencyCalculator)
                ).ToArray();
        }

        private InstanceWrapper BuildGameObj(MeshBlock block, IList<MeshBlock> occlusions, Split[] splits, Transform parent, GameObject prefab, AdjacencyCalculator adjacencyCalculator)
            {
                var meshBuilder = new MeshBuilder();
                adjacencyCalculator.SetupNext(occlusions, splits, block);

                for (var x = (int)block.bounds.minX; x < block.bounds.maxX; x++)
                {
                    for (var y = (int)block.bounds.minY; y < block.bounds.maxY; y++)
                    {
                        for (var z = (int)block.bounds.minZ; z < block.bounds.maxZ; z++)
                        {
                            AdjacencyMatrix adjacencyMatrix = adjacencyCalculator.CalculateAdjacency(x, y, z);
                            if (!adjacencyMatrix.IsVisible()) continue;

                            var clipBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
                            clipBounds.ClipToBounds(splits);
                            block.meshSource.Build(new Vector3(x, y, z) - block.bounds.Position, adjacencyMatrix, block, meshBuilder, clipBounds);
                        }
                    }
                }

                GameObject blockGameObj = GameObjectUtils.AsChildOf(parent, block.bounds.Position, GameObject.Instantiate(prefab));
                blockGameObj.name = block.id;

                Mesh mesh = meshBuilder.DoneCreateMesh();
                blockGameObj.GetComponent<MeshFilter>().mesh = mesh;
                return new InstanceWrapper(blockGameObj, block.rotatedBlockBounds);
            }
    }
}
