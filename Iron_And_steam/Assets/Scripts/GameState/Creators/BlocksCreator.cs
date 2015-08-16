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
                BlockBounds region = splitRegions.First(splitRegion => splitRegion.Contains(block.Bounds));
                SplitBoundsBranch splitBoundsBranch = groupBranch.GetSplitBoundsBranch(region);

                AttachGameObject(block, splitBlocks, splits, splitBoundsBranch.BlocksLeaf, prefabs.BlockPrefab, adjacencyCalculator);
            }
        }

        private MeshBlock[] SplitMeshBlocks(MeshBlock[] meshBlocks, IList<Split> splits)
        {
            return meshBlocks.SelectMany(block =>
            {
                var splitTree = new SplitTree(block.Bounds);
                splitTree.Split(splits);

                return splitTree.GatherSplitBounds()
                    .Select(bounds => block.CopyOf(bounds));
            }).ToArray();
        }

        private void AttachGameObject(MeshBlock block, IList<MeshBlock> occlusions, Split[] splits, BaseTree branch, GameObject prefab, AdjacencyCalculator adjacencyCalculator)
        {
            var meshBuilder = new MeshBuilder();
            adjacencyCalculator.SetupNext(occlusions, splits, block);

            for (var x = (int)block.Bounds.MinX; x < block.Bounds.MaxX; x++)
            {
                for (var y = (int)block.Bounds.MinY; y < block.Bounds.MaxY; y++)
                {
                    for (var z = (int)block.Bounds.MinZ; z < block.Bounds.MaxZ; z++)
                    {
                        AdjacencyMatrix adjacencyMatrix = adjacencyCalculator.CalculateAdjacency(x, y, z);
                        if (!adjacencyMatrix.IsVisible()) continue;

                        var clipBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
                        clipBounds.ClipToBounds(splits);
                        block.MeshSource.Build(new Vector3(x, y, z) - block.Bounds.Position, adjacencyMatrix, block, meshBuilder, clipBounds);
                    }
                }
            }

            GameObject blockGameObject = Object.Instantiate(prefab);
            blockGameObject.name = block.Id;
            blockGameObject.transform.localPosition = block.Bounds.Position;
            branch.Attach(blockGameObject);

            Mesh mesh = meshBuilder.DoneCreateMesh();
            blockGameObject.GetComponent<MeshFilter>().mesh = mesh;
            //return new InstanceWrapper(blockGameObject, block.rotatedBlockBounds);
        }
    }
}
