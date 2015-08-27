using System;
using System.Collections.Generic;
using System.Linq;
using IaS.Domain.Meshes;
using UnityEngine;

namespace IaS.Domain
{
    public enum PartType
    {
        BlockFront,
        BlockBack,
        BlockRight,
        BlockLeft,
        BlockTop,
        BlockBottom,
        OuterEdgeFront,
        OuterEdgeRight,
        OuterEdgeLeft,
        OuterCorner,
        InnerEdgeFront,
        InnerEdgeLeft,
        InnerEdgeRight
    }

    public class ProceduralMeshSource : IMeshSource {
        private static readonly int SHAPE_NO_OCCLUSION = 0;
        private static readonly int SHAPE_QUAD = 1;
        private static readonly int SHAPE_EDGE_SIDE = 2;
        private static readonly int SHAPE_INNER_EDGE_SIDE = 3;

        private static readonly Dictionary<int, int[]> DIC_SHAPE_OCCLUSIONS = new Dictionary<int, int[]>()
        {
            {SHAPE_NO_OCCLUSION, new int[0]},
            {SHAPE_QUAD, new[]{SHAPE_QUAD, SHAPE_EDGE_SIDE, SHAPE_INNER_EDGE_SIDE}},
            {SHAPE_EDGE_SIDE, new[]{SHAPE_EDGE_SIDE}},
            {SHAPE_INNER_EDGE_SIDE, new[]{SHAPE_INNER_EDGE_SIDE}}
        };

        private static readonly Dictionary<int, Part[]> DIC_PROCEDURAL_MESHES = new Dictionary<int, Part[]>(){
            {MeshBlock.TypeCuboid, 
                new[]{
                    new Part(typeof(BlockSideMeshGenerator), PartType.BlockFront, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_FORWARD),
				    new Part(typeof(BlockSideMeshGenerator), PartType.BlockBack, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_BACK),
				    new Part(typeof(BlockSideMeshGenerator), PartType.BlockRight, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_RIGHT),
				    new Part(typeof(BlockSideMeshGenerator), PartType.BlockLeft, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_LEFT),
                    new Part(typeof(BlockSideMeshGenerator), PartType.BlockTop, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_UP),
				    new Part(typeof(BlockSideMeshGenerator), PartType.BlockBottom, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_DOWN),
                }
            },
            {MeshBlock.TypeEdge, 
                new[]{
                    new Part(typeof(BlockOuterEdgeMeshGenerator), PartType.OuterEdgeFront, SHAPE_NO_OCCLUSION, null),
				    new Part(typeof(BlockOuterEdgeMeshGenerator), PartType.OuterEdgeRight, SHAPE_EDGE_SIDE, AdjacencyMatrix.DIRECTION_RIGHT),
				    new Part(typeof(BlockOuterEdgeMeshGenerator), PartType.OuterEdgeLeft, SHAPE_EDGE_SIDE, AdjacencyMatrix.DIRECTION_LEFT),
				    new Part(typeof(BlockSideMeshGenerator), PartType.BlockBottom, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_DOWN),
				    new Part(typeof(BlockSideMeshGenerator), PartType.BlockBack, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_BACK)
                }
            },
            {MeshBlock.TypeCorner, 
                new[]{
                    new Part(typeof(BlockCornerMeshGenerator), PartType.OuterCorner, SHAPE_NO_OCCLUSION, null)
                }
            },
            {MeshBlock.TypeSlope, 
                new[]{
                    new Part(typeof(BlockInnerEdgeMeshGenerator), PartType.InnerEdgeFront, SHAPE_NO_OCCLUSION, null),
                    new Part(typeof(BlockInnerEdgeMeshGenerator), PartType.InnerEdgeLeft, SHAPE_INNER_EDGE_SIDE, AdjacencyMatrix.DIRECTION_LEFT),
                    new Part(typeof(BlockInnerEdgeMeshGenerator), PartType.InnerEdgeRight, SHAPE_INNER_EDGE_SIDE, AdjacencyMatrix.DIRECTION_RIGHT),
                    new Part(typeof(BlockSideMeshGenerator), PartType.BlockBack, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_BACK),
                    new Part(typeof(BlockSideMeshGenerator), PartType.BlockBottom, SHAPE_QUAD, AdjacencyMatrix.DIRECTION_DOWN)
                }
            }
        };

        private readonly Part[] _blockParts;
        private readonly Dictionary<Type, IProcMeshGenerator> _meshGenerators = new Dictionary<Type,IProcMeshGenerator>();

        private ProceduralMeshSource(Part[] blockParts)
        {
            _blockParts = blockParts;
        }

        public static ProceduralMeshSource GetInstance(int proceduralBlockType)
        {
            if (!DIC_PROCEDURAL_MESHES.ContainsKey(proceduralBlockType))
                throw new Exception(string.Format("Couldn't find procedural mesh with id {0}", proceduralBlockType));

            Part[] parts = DIC_PROCEDURAL_MESHES[proceduralBlockType];
            return new ProceduralMeshSource(parts);
        }

        public int GetShapeToOcclude(int[] direction)
        {
            Part occludingPart = _blockParts.FirstOrDefault(part => (part.Direction != null) && 
                (part.Direction[0] == direction[0]) && (part.Direction[1] == direction[1]) && (part.Direction[2] == direction[2]));
            return occludingPart == null ? SHAPE_NO_OCCLUSION : occludingPart.OcclusionShape;
        }

        public bool OccludesShape(int[] direction, int occludedShape)
        {
            Part occludedPart = _blockParts.FirstOrDefault(part => (part.Direction != null) &&
                (part.Direction[0] == direction[0]) && (part.Direction[1] == direction[1]) && (part.Direction[2] == direction[2]));

            if (occludedPart == null)
                return false;

            return DIC_SHAPE_OCCLUSIONS[occludedPart.OcclusionShape].Contains(occludedShape);
        }

        public void Build(Vector3 pos, AdjacencyMatrix adjacencyMatrix, MeshBlock block, MeshBuilder meshBuilder, BlockBounds clipBounds)
        {
            Quaternion rot = block.RotationQuat;
			Matrix4x4 transform = Matrix4x4.TRS(pos + new Vector3(0.5f, 0.5f, 0.5f), rot, new Vector3(1, 1, 1));
			BlockBounds localClipBounds = new BlockBounds (clipBounds.Position - block.OriginalBounds.Position - pos, clipBounds.Size);
			localClipBounds.SetToRotationFrom (Quaternion.Inverse(block.RotationQuat), new Vector3(0.5f, 0.5f, 0.5f));
			meshBuilder.BeforeNext (transform, new Vector3(-0.5f, -0.5f, -0.5f), localClipBounds);

            foreach (Part part in _blockParts)
            {
                if ((part.Direction == null) || (!adjacencyMatrix.RPt(part.Direction[0], part.Direction[1], part.Direction[2]).Occluded))
                {
                    IProcMeshGenerator meshGenerator;
					if(!_meshGenerators.TryGetValue(part.MeshGenerator, out meshGenerator)){
						_meshGenerators[part.MeshGenerator] = (meshGenerator = (IProcMeshGenerator)Activator.CreateInstance(part.MeshGenerator));
					}

                    meshGenerator.BuildMesh(part.PartType, adjacencyMatrix, meshBuilder, localClipBounds);
                }
            }
        }

        internal class Part
        {
            internal PartType PartType;
            internal int OcclusionShape;
            internal int[] Direction;
			internal Type MeshGenerator;

			internal Part(Type meshGenerator, PartType partType, int occlusionShape, int[] direction)
            {
                PartType = partType;
                OcclusionShape = occlusionShape;
                Direction = direction;
				MeshGenerator = meshGenerator;
            }
        }
	}
}
