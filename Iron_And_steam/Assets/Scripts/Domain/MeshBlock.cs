using System;
using System.Collections.Generic;
using UnityEngine;

namespace IaS.Domain
{

    [Serializable]
    public class MeshBlock
    {
        public enum Type { Block, Edge, Corner, Slope }

        public const int TypeCuboid = 0;
        public const int TypeEdge = 1;
        public const int TypeCorner = 2;
        public const int TypeSlope = 3;
        public const int TypeSplit = 4;
        public const int TypeTrack = 5;

        private static readonly Dictionary<Type, int> TypeToTypeCode = new Dictionary<Type, int>
            {{Type.Block, TypeCuboid}, {Type.Edge, TypeEdge}, {Type.Corner, TypeCorner}, {Type.Slope, TypeSlope}}; 

        public readonly Quaternion RotationQuat;
        public readonly string Id;
        public readonly Type BlockType;
        public readonly int TypeCode;
        public readonly IMeshSource MeshSource;

        public readonly int OccludeOrder;
        public readonly SplittedRegion SplittedRegion;
        public readonly BlockBounds OriginalBounds;
        public readonly BlockBounds RotatedBlockBounds;

        public MeshBlock(string id, Type type, BlockBounds blockBounds, SplittedRegion splittedRegion, Quaternion rotation, int occludeOrder)
        {
            Id = id;
            BlockType = type;
            RotationQuat = rotation;

            OccludeOrder = occludeOrder;
            OriginalBounds = blockBounds;
            SplittedRegion = splittedRegion;
            RotatedBlockBounds = blockBounds.Copy();

            TypeCode = TypeToTypeCode[type];
            MeshSource = ProceduralMeshSource.GetInstance(TypeCode);
        }

        public bool Intersects(MeshBlock test)
        {
            return Intersects1D(OriginalBounds.MinX, OriginalBounds.MinX, OriginalBounds.MaxX, OriginalBounds.MaxX) &&
                Intersects1D(OriginalBounds.MinY, OriginalBounds.MinY, OriginalBounds.MaxY, OriginalBounds.MaxY) &&
                Intersects1D(OriginalBounds.MinZ, OriginalBounds.MinZ, OriginalBounds.MaxZ, OriginalBounds.MaxZ);
        }

        private bool Intersects1D(float min1, float min2, float max1, float max2)
        {
            float smallestMax = Math.Min(max1, max2);
            float smallestMin = Math.Min(min1, min2);
            float biggestMin = Math.Max(min1, min2);
            return ((smallestMax <= smallestMin) || (smallestMax >= biggestMin));
        }

        public MeshBlock CopyOf(BlockBounds blockBounds, SplittedRegion splittedRegion)
        {
            return new MeshBlock(Id, BlockType, blockBounds, splittedRegion, RotationQuat, OccludeOrder);
        }
    }
}
