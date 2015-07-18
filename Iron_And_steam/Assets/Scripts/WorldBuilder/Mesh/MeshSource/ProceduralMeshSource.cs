using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder.MeshLoader;
using IaS.WorldBuilder.Meshes;

namespace IaS.WorldBuilder
{

	public class ProceduralMeshSource : MeshSource {

        private static readonly int SHAPE_NO_OCCLUSION = 0;
        private static readonly int SHAPE_QUAD = 1;
        private static readonly int SHAPE_EDGE_SIDE = 2;
        private static readonly int SHAPE_INNER_EDGE_SIDE = 3;

        private static readonly Dictionary<int, int[]> DIC_SHAPE_OCCLUSIONS = new Dictionary<int, int[]>()
        {
            {SHAPE_NO_OCCLUSION, new int[0]},
            {SHAPE_QUAD, new int[]{SHAPE_QUAD, SHAPE_EDGE_SIDE, SHAPE_INNER_EDGE_SIDE}},
            {SHAPE_EDGE_SIDE, new int[]{SHAPE_EDGE_SIDE}},
            {SHAPE_INNER_EDGE_SIDE, new int[]{SHAPE_INNER_EDGE_SIDE}}
        };

        private static readonly Dictionary<int, Part[]> DIC_PROCEDURAL_MESHES = new Dictionary<int, Part[]>(){
            {MeshBlock.TYPE_CUBOID, 
                new Part[]{
                    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_FRONT, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_FORWARD),
				    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_BACK, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_BACK),
				    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_RIGHT, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_RIGHT),
				    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_LEFT, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_LEFT),
                    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_TOP, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_UP),
				    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_BOTTOM, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_DOWN),
                }
            },
            {MeshBlock.TYPE_EDGE, 
                new Part[]{
                    new Part(typeof(BlockOuterEdgeMeshGenerator), MeshPartsData.PART_OUTER_EDGE_FRONT, SHAPE_NO_OCCLUSION, null),
				    new Part(typeof(BlockOuterEdgeMeshGenerator), MeshPartsData.PART_OUTER_EDGE_RIGHT, SHAPE_EDGE_SIDE, AdjacencyMatrix.DIRECTION_RIGHT),
				    new Part(typeof(BlockOuterEdgeMeshGenerator), MeshPartsData.PART_OUTER_EDGE_LEFT, SHAPE_EDGE_SIDE, AdjacencyMatrix.DIRECTION_LEFT),
				    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_BOTTOM, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_DOWN),
				    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_BACK, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_BACK)
                }
            },
            {MeshBlock.TYPE_CORNER, 
                new Part[]{
                    new Part(typeof(BlockCornerMeshGenerator), MeshPartsData.PART_OUTER_CORNER, SHAPE_NO_OCCLUSION, null)
                }
            },
            {MeshBlock.TYPE_SLOPE, 
                new Part[]{
                    new Part(typeof(BlockInnerEdgeMeshGenerator), MeshPartsData.PART_INNER_EDGE_FRONT, SHAPE_NO_OCCLUSION, null),
                    new Part(typeof(BlockInnerEdgeMeshGenerator), MeshPartsData.PART_INNER_EDGE_LEFT, SHAPE_INNER_EDGE_SIDE, AdjacencyMatrix.DIRECTION_LEFT),
                    new Part(typeof(BlockInnerEdgeMeshGenerator), MeshPartsData.PART_INNER_EDGE_RIGHT, SHAPE_INNER_EDGE_SIDE, AdjacencyMatrix.DIRECTION_RIGHT),
                    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_BACK, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_BACK),
                    new Part(typeof(BlockSideMeshGenerator), MeshPartsData.PART_BLOCK_BOTTOM, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_DOWN)
                }
            }
        };

        private Part[] blockParts;
        private Dictionary<Type, ProcMeshGenerator> meshGenerators = new Dictionary<Type,ProcMeshGenerator>();

        private ProceduralMeshSource(Part[] blockParts)
        {
            this.blockParts = blockParts;
        }

        public static ProceduralMeshSource GetInstance(int proceduralBlockType)
        {
            if (!DIC_PROCEDURAL_MESHES.ContainsKey(proceduralBlockType))
                throw new Exception(String.Format("Couldn't find procedural mesh with id {0}", proceduralBlockType));

            Part[] parts = DIC_PROCEDURAL_MESHES[proceduralBlockType];
            return new ProceduralMeshSource(parts);
        }

        public int GetShapeToOcclude(int[] direction)
        {
            Part occludingPart = blockParts.FirstOrDefault(part => (part.direction != null) && 
                (part.direction[0] == direction[0]) && (part.direction[1] == direction[1]) && (part.direction[2] == direction[2]));
            return occludingPart == null ? SHAPE_NO_OCCLUSION : occludingPart.occlusionShape;
        }

        public bool OccludesShape(int[] direction, int occludedShape)
        {
            Part occludedPart = blockParts.FirstOrDefault(part => (part.direction != null) &&
                (part.direction[0] == direction[0]) && (part.direction[1] == direction[1]) && (part.direction[2] == direction[2]));

            if (occludedPart == null)
                return false;

            return DIC_SHAPE_OCCLUSIONS[occludedPart.occlusionShape].Contains(occludedShape);
        }

        public void Build(Vector3 pos, AdjacencyMatrix adjacencyMatrix, MeshBlock block, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {
			Quaternion rot = block.rotation.quaternion;
			Matrix4x4 transform = Matrix4x4.TRS(pos + new Vector3(0.5f, 0.5f, 0.5f), rot, new Vector3(1, 1, 1));
			BlockBounds localClipBounds = new BlockBounds (clipBounds.Position - block.bounds.Position - pos, clipBounds.Size);
			localClipBounds.SetToRotationFrom (Quaternion.Inverse(block.rotation.quaternion), new Vector3(0.5f, 0.5f, 0.5f));
			meshBuilder.BeforeNext (transform, new Vector3(-0.5f, -0.5f, -0.5f), localClipBounds);

            foreach (Part part in blockParts)
            {
                if ((part.direction == null) || (!adjacencyMatrix.RPt(part.direction[0], part.direction[1], part.direction[2]).Occluded))
                {
                    ProcMeshGenerator meshGenerator;
					if(!meshGenerators.TryGetValue(part.meshGenerator, out meshGenerator)){
						meshGenerators[part.meshGenerator] = (meshGenerator = (ProcMeshGenerator)Activator.CreateInstance(part.meshGenerator));
					}

                    meshGenerator.BuildMesh(part.partName, adjacencyMatrix, meshBuilder, localClipBounds);
                }
            }
        }

        internal class Part
        {
            internal String partName;
            internal int occlusionShape;
            internal int[] direction;
			internal Type meshGenerator;

			internal Part(Type meshGenerator, String partName, int occlusionShape, int[] direction)
            {
                this.partName = partName;
                this.occlusionShape = occlusionShape;
                this.direction = direction;
				this.meshGenerator = meshGenerator;
            }
        }
	}
}
