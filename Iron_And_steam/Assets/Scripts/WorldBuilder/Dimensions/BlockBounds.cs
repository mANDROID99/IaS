using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.Helpers;
using IaS.GameObjects;

namespace IaS.WorldBuilder
{
    [Serializable]
    public class BlockBounds
    {
		public const float SPLIT_GAP = 0.05f;

        public float minX;
        public float minY;
        public float minZ;
        public float maxX;
        public float maxY;
        public float maxZ;

        public Vector3 CenterPos { get { return Position + Size / 2f; } }
        public Vector3 Position { get { return new Vector3(minX, minY, minZ); } }
        public Vector3 Size { get { return new Vector3(maxX - minX, maxY - minY, maxZ - minZ); } }

        public BlockBounds(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            this.minX = minX;
            this.minY = minY;
            this.minZ = minZ;
            this.maxX = maxX;
            this.maxY = maxY;
            this.maxZ = maxZ;
        }

        public BlockBounds(Vector3 position, Vector3 size)
        {
            minX = position.x;
            minY = position.y;
            minZ = position.z;

            maxX = minX + size.x;
            maxY = minY + size.y;
            maxZ = minZ + size.z;
        }

        public BlockBounds UnionWith(BlockBounds bounds)
        {
            minX = Math.Min(bounds.minX, minX);
            minY = Math.Min(bounds.minY, minY);
            minZ = Math.Min(bounds.minZ, minZ);

            maxX = Math.Max(bounds.maxX, maxX);
            maxY = Math.Max(bounds.maxY, maxY);
            maxZ = Math.Max(bounds.maxZ, maxZ);
            return this;
        }

        public BlockBounds SetToRotationFrom(Quaternion rotation, Vector3 pivot, BlockBounds originalBounds=null)
        {
			if (originalBounds == null) {
				originalBounds = this;
			}
            Vector3 r1 = MathHelper.RotateAroundPivot(new Vector3(originalBounds.minX, originalBounds.minY, originalBounds.minZ), pivot, rotation);
            Vector3 r2 = MathHelper.RotateAroundPivot(new Vector3(originalBounds.maxX, originalBounds.maxY, originalBounds.maxZ), pivot, rotation);

            this.minX = MathHelper.RoundToDp(Mathf.Min(r1.x, r2.x), 4);
			this.minY = MathHelper.RoundToDp(Mathf.Min(r1.y, r2.y), 4);
			this.minZ = MathHelper.RoundToDp(Mathf.Min(r1.z, r2.z), 4);
			this.maxX = MathHelper.RoundToDp(Mathf.Max(r1.x, r2.x), 4);
			this.maxY = MathHelper.RoundToDp(Mathf.Max(r1.y, r2.y), 4);	
			this.maxZ = MathHelper.RoundToDp(Mathf.Max(r1.z, r2.z), 4);
			return this;
        }

		public BlockBounds ClipToBounds(IList<Split> splits)
		{
			foreach (Split split in splits) 
			{
				bool lhs = split.DistanceFromSplit(this.Position) < 0;
				if(split.axis.x > 0)
				{
					minX = lhs ? minX : Mathf.Max(split.value + SPLIT_GAP, minX);
					maxX = !lhs ? maxX : Mathf.Min(split.value - SPLIT_GAP, maxX);
				}
				else if(split.axis.y > 0)
				{
					minY = lhs ? minY : Mathf.Max(split.value + SPLIT_GAP, minY);
					maxY = !lhs ? maxY : Mathf.Min(split.value - SPLIT_GAP, maxY);
				}
				else if(split.axis.z > 0)
				{
					minZ = lhs ? minZ : Mathf.Max(split.value + SPLIT_GAP, minZ);
					maxZ = !lhs ? maxZ : Mathf.Min(split.value - SPLIT_GAP, maxZ);
				}
			}
			return this;
		}

		public Vector3 ClipVec3(Vector3 vec3)
		{
			vec3.x = Mathf.Min (this.maxX, Mathf.Max (vec3.x, this.minX));
			vec3.y = Mathf.Min (this.maxY, Mathf.Max (vec3.y, this.minY));
			vec3.z = Mathf.Min (this.maxZ, Mathf.Max (vec3.z, this.minZ));
			return vec3;
		}
        
        public Bounds ToAxisAlignedBounds()
        {
            return new Bounds(CenterPos, Size);
        }

        public BlockBounds Copy()
        {
            return new BlockBounds(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public override string ToString()
        {
            return String.Format("[pos: {0}, size: {1}]", Position, Size);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(BlockBounds))
            {
                return base.Equals(obj);
            }
            BlockBounds testBB = (BlockBounds) obj;
            return MathHelper.VectorsEqualWError(Position, testBB.Position) && MathHelper.VectorsEqualWError(Size, testBB.Size);
        }

        public bool Contains(int x, int y, int z)
        {
            return x >= minX && x < maxX && 
                y >= minY && y < maxY && 
                z >= minZ && z < maxZ;
        }

        public bool Contains(Vector3 pos)
        {
            return Contains(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
        }
    }
}
