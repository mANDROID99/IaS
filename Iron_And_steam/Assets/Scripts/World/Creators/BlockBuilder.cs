
using System.Collections.Generic;
using IaS.Domain;
using IaS.Domain.Meshes;
using IaS.World.WorldTree;
using UnityEngine;

namespace IaS.World.Creators
{
    public class BlockBuilder
    {
        public void BuildBlocks(GroupBranch groupBranch, List<MeshBlock> meshBlocks)
        {
            var adjacencyCalculator = new AdjacencyCalculator();
            foreach (MeshBlock block in groupBranch.SplittedMeshBlocks)
            {
                AttachGameObject(block, groupBranch, adjacencyCalculator);
            }
        }

        private void AttachGameObject(MeshBlock block, GroupBranch groupBranch, AdjacencyCalculator adjacencyCalculator)
        {
            SplitBoundsBranch splitBoundsBranch = groupBranch.SplitBoundsBranchContaining(block.Bounds);

            var meshBuilder = new MeshBuilder();
            adjacencyCalculator.SetupNext(groupBranch.SplittedMeshBlocks, groupBranch.Splits, block);

            for (var x = (int)block.Bounds.MinX; x < block.Bounds.MaxX; x++)
            {
                for (var y = (int)block.Bounds.MinY; y < block.Bounds.MaxY; y++)
                {
                    for (var z = (int)block.Bounds.MinZ; z < block.Bounds.MaxZ; z++)
                    {
                        AdjacencyMatrix adjacencyMatrix = adjacencyCalculator.CalculateAdjacency(x, y, z);
                        if (!adjacencyMatrix.IsVisible()) continue;

                        var clipBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
                        clipBounds.ClipToBounds(groupBranch.Splits);
                        block.MeshSource.Build(new Vector3(x, y, z) - block.Bounds.Position, adjacencyMatrix, block, meshBuilder, clipBounds);
                    }
                }
            }

            GameObject blockGameObject = Object.Instantiate(groupBranch.Level.Prefabs.BlockPrefab);
            blockGameObject.name = block.Id;
            blockGameObject.transform.localPosition = block.Bounds.Position;
            splitBoundsBranch.BlocksLeaf.Attach(blockGameObject);

            Mesh mesh = meshBuilder.DoneCreateMesh();
            blockGameObject.GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
