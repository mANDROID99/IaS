using System;
using System.Collections.Generic;
using IaS.Helpers;
using UnityEngine;

namespace IaS.WorldBuilder
{
    [Serializable]
    public class BlockBounds
    {
        public static readonly float MaxWorldSize = 100000;
        public static readonly BlockBounds Unbounded = new BlockBounds(-MaxWorldSize, -MaxWorldSize, -MaxWorldSize, MaxWorldSize, MaxWorldSize, MaxWorldSize);
        public const float SPLIT_GAP = 0.05f;

        public float MinX;
        public float MinY;
        public float MinZ;
        public float MaxX;
        public float MaxY;
        public float MaxZ;

        public Vector3 CenterPos { get { return Position + Size / 2f; } }
        public Vector3 Position { get { return new Vector3(MinX, MinY, MinZ); } }
        public Vector3 Size { get { return new Vector3(MaxX - MinX, MaxY - MinY, MaxZ - MinZ); } }
        public Quaternion? _rotation = null;

        public Quaternion Rotation
        {
            get { return _rotation ?? Quaternion.identity; }
        }

        public BlockBounds(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MaxX = maxX;
            MaxY = maxY;
            MaxZ = maxZ;
        }

        public BlockBounds(Vector3 position, Vector3 size)
        {
            MinX = position.x;
            MinY = position.y;
            MinZ = position.z;

            MaxX = MinX + size.x;
            MaxY = MinY + size.y;
            MaxZ = MinZ + size.z;
        }

        public BlockBounds UnionWith(BlockBounds bounds)
        {
            MinX = Math.Min(bounds.MinX, MinX);
            MinY = Math.Min(bounds.MinY, MinY);
            MinZ = Math.Min(bounds.MinZ, MinZ);

            MaxX = Math.Max(bounds.MaxX, MaxX);
            MaxY = Math.Max(bounds.MaxY, MaxY);
            MaxZ = Math.Max(bounds.MaxZ, MaxZ);
            return this;
        }

        public BlockBounds SetToRotationFrom(Quaternion rotation, Vector3 pivot, BlockBounds originalBounds = null)
        {
			if (originalBounds == null) {
				originalBounds = this;
			}
            _rotation = rotation;
            Vector3 r1 = MathHelper.RotateAroundPivot(new Vector3(originalBounds.MinX, originalBounds.MinY, originalBounds.MinZ), pivot, rotation);
            Vector3 r2 = MathHelper.RotateAroundPivot(new Vector3(originalBounds.MaxX, originalBounds.MaxY, originalBounds.MaxZ), pivot, rotation);

            MinX = MathHelper.RoundToDp(Mathf.Min(r1.x, r2.x), 4);
			MinY = MathHelper.RoundToDp(Mathf.Min(r1.y, r2.y), 4);
			MinZ = MathHelper.RoundToDp(Mathf.Min(r1.z, r2.z), 4);
			MaxX = MathHelper.RoundToDp(Mathf.Max(r1.x, r2.x), 4);
			MaxY = MathHelper.RoundToDp(Mathf.Max(r1.y, r2.y), 4);	
			MaxZ = MathHelper.RoundToDp(Mathf.Max(r1.z, r2.z), 4);
			return this;
        }

		public BlockBounds ClipToBounds(IList<Split> splits)
		{
			foreach (Split split in splits) 
			{
				bool lhs = split.DistanceFromSplit(this.Position) < 0;
				if(split.Axis.x > 0)
				{
					MinX = lhs ? MinX : Mathf.Max(split.Value + SPLIT_GAP, MinX);
					MaxX = !lhs ? MaxX : Mathf.Min(split.Value - SPLIT_GAP, MaxX);
				}
				else if(split.Axis.y > 0)
				{
					MinY = lhs ? MinY : Mathf.Max(split.Value + SPLIT_GAP, MinY);
					MaxY = !lhs ? MaxY : Mathf.Min(split.Value - SPLIT_GAP, MaxY);
				}
				else if(split.Axis.z > 0)
				{
					MinZ = lhs ? MinZ : Mathf.Max(split.Value + SPLIT_GAP, MinZ);
					MaxZ = !lhs ? MaxZ : Mathf.Min(split.Value - SPLIT_GAP, MaxZ);
				}
			}
			return this;
		}

		public Vector3 ClipVec3(Vector3 vec3)
		{
			vec3.x = Mathf.Min (this.MaxX, Mathf.Max (vec3.x, this.MinX));
			vec3.y = Mathf.Min (this.MaxY, Mathf.Max (vec3.y, this.MinY));
			vec3.z = Mathf.Min (this.MaxZ, Mathf.Max (vec3.z, this.MinZ));
			return vec3;
		}
        
        public Bounds ToAxisAlignedBounds()
        {
            return new Bounds(CenterPos, Size);
        }

        public BlockBounds Copy()
        {
            return new BlockBounds(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
        }

        public bool AproxEqual(BlockBounds bounds)
        {
            return MathHelper.VectorsEqualWError(Position, bounds.Position) && MathHelper.VectorsEqualWError(Size, bounds.Size);
        }

        public override string ToString()
        {
            return string.Format("[pos: {0}, size: {1}]", Position, Size);
        }

        

        public bool Contains(int x, int y, int z)
        {
            return x >= MinX && x < MaxX && 
                y >= MinY && y < MaxY && 
                z >= MinZ && z < MaxZ;
        }

        public bool Contains(Vector3 pos)
        {
            return Contains(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
        }

        public bool Contains(BlockBounds bounds)
        {
            return bounds.MinX >= MinX && bounds.MaxX <= MaxX &&
                   bounds.MinY >= MinY && bounds.MaxY <= MaxY &&
                   bounds.MinZ >= MinZ && bounds.MaxZ <= MaxZ;
        }
    }
}
