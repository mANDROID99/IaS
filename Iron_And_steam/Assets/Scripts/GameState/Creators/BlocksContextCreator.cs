﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.GameObjects;
using IaS.GameState.Creators;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Meshes;
using UnityEngine;

namespace IaS.GameState
{
    public class BlocksContextCreator
    {

        public BlocksContext CreateBlocksContext(MeshBlock[] meshBlocks, Split[] splits)
        {
            MeshBlock[] splitBlocks = SplitMeshBlocks(meshBlocks, splits);
            return new BlocksContext(splitBlocks);
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

        public void CreateBlockControllers(BlocksContext blockContext, BlockRotaterController blockRotaterController, List<Controller> controllers, Prefabs prefabs, Transform container)
        {
            var adjacencyCalculator = new AdjacencyCalculator();
            InstanceWrapper[] instances = blockContext.Blocks
                .Select(block => BuildGameObj(block, blockContext.Blocks, blockContext.GroupContext.Splits, container, prefabs.BlockPrefab, adjacencyCalculator))
                .ToArray();

            blockRotaterController.AddInstancesToRotate(instances);
        }

        public InstanceWrapper[] BuildGameObjects(GroupContext groupCtx, Transform container, GameObject blockPrefab)
        {
            var adjacencyCalculator = new AdjacencyCalculator();
            return
                groupCtx.BlockContext.Blocks.Select(
                    block =>
                        BuildGameObj(block, groupCtx.BlockContext.Blocks, groupCtx.Splits, container.transform, blockPrefab, adjacencyCalculator)
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