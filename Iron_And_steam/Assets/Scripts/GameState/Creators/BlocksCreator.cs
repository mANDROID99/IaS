using System.Collections.Generic;
using System.Linq;
using IaS.GameState.Creators;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Meshes;
using UnityEngine;

namespace IaS.GameState
{
    public class BlocksCreator
    {

        public void CreateBlockGameObjects(Split[] splits, MeshBlock[] meshes, GroupBranch groupBranch, Prefabs prefabs)
        {
            BlockBounds[] splitRegions = groupBranch.SplitBounds.ToArray();
            MeshBlock[] splitBlocks = SplitMeshBlocks(meshes, splits);

            var adjacencyCalculator = new AdjacencyCalculator();
            foreach (MeshBlock block in splitBlocks)
            {
                BlockBounds region = splitRegions.First(splitRegion => splitRegion.Contains(block.bounds));
                SplitBoundsBranch splitBoundsBranch = groupBranch.GetSplitBoundsBranch(region);

                AttachGameObject(block, splitBlocks, splits, splitBoundsBranch.BlocksLeaf, prefabs.BlockPrefab, adjacencyCalculator);
            }
        }

        private MeshBlock[] SplitMeshBlocks(MeshBlock[] meshBlocks, IList<Split> splits)
        {
            return meshBlocks.SelectMany(block =>
            {
                var splitTree = new SplitTree(block.bounds);
                splitTree.Split(splits);

                return splitTree.GatherSplitBounds()
                    .Select(bounds => block.CopyOf(bounds));
            }).ToArray();
        }

        private void AttachGameObject(MeshBlock block, IList<MeshBlock> occlusions, Split[] splits, BaseTree branch, GameObject prefab, AdjacencyCalculator adjacencyCalculator)
        {
            var meshBuilder = new MeshBuilder();
            adjacencyCalculator.SetupNext(occlusions, splits, block);

            for (var x = (int)block.bounds.MinX; x < block.bounds.MaxX; x++)
            {
                for (var y = (int)block.bounds.MinY; y < block.bounds.MaxY; y++)
                {
                    for (var z = (int)block.bounds.MinZ; z < block.bounds.MaxZ; z++)
                    {
                        AdjacencyMatrix adjacencyMatrix = adjacencyCalculator.CalculateAdjacency(x, y, z);
                        if (!adjacencyMatrix.IsVisible()) continue;

                        var clipBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
                        clipBounds.ClipToBounds(splits);
                        block.meshSource.Build(new Vector3(x, y, z) - block.bounds.Position, adjacencyMatrix, block, meshBuilder, clipBounds);
                    }
                }
            }

            GameObject blockGameObject = Object.Instantiate(prefab);
            blockGameObject.name = block.id;
            blockGameObject.transform.localPosition = block.bounds.Position;
            branch.Attach(blockGameObject);

            Mesh mesh = meshBuilder.DoneCreateMesh();
            blockGameObject.GetComponent<MeshFilter>().mesh = mesh;
            //return new InstanceWrapper(blockGameObject, block.rotatedBlockBounds);
        }
    }
}
