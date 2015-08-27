
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

        private void AttachGameObject(MeshBlock meshBlock, GroupBranch groupBranch, AdjacencyCalculator adjacencyCalculator)
        {
            SplitBoundsBranch splitBoundsBranch = groupBranch.SplitBoundsBranchContaining(meshBlock.SplittedRegion);

            var meshBuilder = new MeshBuilder();
            adjacencyCalculator.SetupNext(groupBranch.SplittedMeshBlocks, groupBranch.Splits, meshBlock);

            for (var x = (int)meshBlock.OriginalBounds.MinX; x < meshBlock.OriginalBounds.MaxX; x++)
            {
                for (var y = (int)meshBlock.OriginalBounds.MinY; y < meshBlock.OriginalBounds.MaxY; y++)
                {
                    for (var z = (int)meshBlock.OriginalBounds.MinZ; z < meshBlock.OriginalBounds.MaxZ; z++)
                    {
                        AdjacencyMatrix adjacencyMatrix = adjacencyCalculator.CalculateAdjacency(x, y, z);
                        if (!adjacencyMatrix.IsVisible()) continue;

                        var clipBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
                        clipBounds.ClipToBounds(groupBranch.Splits);
                        meshBlock.MeshSource.Build(new Vector3(x, y, z) - meshBlock.OriginalBounds.Position, adjacencyMatrix, meshBlock, meshBuilder, clipBounds);
                    }
                }
            }

            GameObject blockGameObject = Object.Instantiate(groupBranch.Level.Prefabs.BlockPrefab);
            blockGameObject.name = meshBlock.Id;
            blockGameObject.transform.localPosition = meshBlock.OriginalBounds.Position;
            splitBoundsBranch.BlocksLeaf.Attach(blockGameObject);

            Mesh mesh = meshBuilder.DoneCreateMesh();
            blockGameObject.GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
