using System;
using UnityEngine;

namespace IaS.WorldBuilder
{
    public class AdjacencyMatrix
    {
        public static readonly int[] DIRECTION_LEFT = new int[] { 0, 1, 1 };
        public static readonly int[] DIRECTION_RIGHT = new int[] { 2, 1, 1 };
        public static readonly int[] DIRECTION_FORWARD = new int[] { 1, 1, 0 };
        public static readonly int[] DIRECTION_BACK = new int[] { 1, 1, 2 };
        public static readonly int[] DIRECTION_UP = new int[] { 1, 2, 1 };
        public static readonly int[] DIRECTION_DOWN = new int[] { 1, 0, 1 };
        public static readonly int[][] OCC_DIR_ALL = new int[][]{
            DIRECTION_LEFT, DIRECTION_RIGHT, DIRECTION_FORWARD, DIRECTION_BACK, DIRECTION_UP, DIRECTION_DOWN
        };

        private const int OCCLUDES_BIT = 0;
        private const int INTERNAL_BIT = 1;
        private const int TYPE_BIT = 2;

        private int[, ,] adjacencies;
        public AdjacencyRotationMatrix rotationMatrix { get; private set; }

        public AdjacencyMatrix()
        {
            adjacencies = new int[3, 3, 3];
            rotationMatrix = new AdjacencyRotationMatrix();
        }

        public void Reset(Quaternion rotation)
        {
            Array.Clear(adjacencies, 0, adjacencies.Length);
            rotationMatrix.Set(rotation);
        }

        public void SetRotation(Quaternion rotation)
        {
            rotationMatrix.Set(rotation);
        }

        private int Get(int x, int y, int z, int updatedBit)
        {
            return this.adjacencies[x, y, z] &= (int)Math.Pow(2, updatedBit);
        }

        private void Set(int x, int y, int z, bool mask, int updatedBit)
        {
            if (mask)
            {
                this.adjacencies[x, y, z] |= (int)Math.Pow(2, updatedBit);
            }
            else
            {
                this.adjacencies[x, y, z] ^= this.adjacencies[x, y, z] & (int)Math.Pow(2, updatedBit);
            }
        }

        public void SetAllBits(int x, int y, int z, bool occludes, bool isInternal, int blockType)
        {
            SetIsInternal(x, y, z, isInternal);
            SetOccludes(x, y, z, occludes);
            AddAdjacency(x, y, z, blockType);
        }

        public void SetOccludes(int x, int y, int z, bool occludes)
        {
            Set(x, y, z, occludes, OCCLUDES_BIT);
        }

        public void SetIsInternal(int x, int y, int z, bool isInternal)
        {
            Set(x, y, z, isInternal, INTERNAL_BIT);
        }

        public void AddAdjacency(int x, int y, int z, int blockType)
        {
            this.adjacencies[x, y, z] |= (int)Math.Pow(2, TYPE_BIT + blockType);
        }

        public bool IsOccluded(int x, int y, int z)
        {
            return Get(x, y, z, OCCLUDES_BIT) > 0;
        }

        public bool IsInternal(int x, int y, int z)
        {
            return Get(x, y, z, INTERNAL_BIT) > 0;
        }

        public bool HasType(int x, int y, int z, int blockType)
        {
            return Get(x, y, z, TYPE_BIT + blockType) > 0;
        }

        public static int[] InvertDirectionMask(int[] visibilityDirection)
        {
            return new int[]{
                2 - visibilityDirection[0],
                2 - visibilityDirection[1],
                2 - visibilityDirection[2],
            };
        }

        internal bool IsVisible()
        {
            return adjacencies[1, 1, 1] == 0;
        }


        public RotatedPoint RPt(int x, int y, int z)
        {
            int[] rotPt = rotationMatrix.Transform(x, y, z);
            return new RotatedPoint { x = rotPt[0], y = rotPt[1], z = rotPt[2], adjacencyMatrix = this };
        }

        public RotatedPoint RPtInverse(int x, int y, int z)
        {
            int[] rotPt = rotationMatrix.TransformInverse(x, y, z);
            return new RotatedPoint { x = rotPt[0], y = rotPt[1], z = rotPt[2], adjacencyMatrix = this };
        }

        public struct RotatedPoint
        {
            public int x, y, z;
            internal AdjacencyMatrix adjacencyMatrix;

            public bool Occluded { get { return adjacencyMatrix.IsOccluded(x, y, z); } }
            public bool Internal { get { return adjacencyMatrix.IsInternal(x, y, z); } }
            public bool HasType(int blockType)
            {
                return adjacencyMatrix.HasType(x, y, z, blockType);
            }


            internal int[] toDirection()
            {
                return new int[] { x, y, z };
            }
        }
    }

    public class AdjacencyRotationMatrix
    {
		public Quaternion quaternion { get; private set; }
		public int[, , ,] matrix { get; private set; }
        private int[, , ,] inverseMatrix;

        public AdjacencyRotationMatrix()
        {
            matrix = new int[3, 3, 3, 3];
            inverseMatrix = new int[3, 3, 3, 3];
        }

        public void Set(Quaternion quaternion)
        {
			this.quaternion = quaternion;	
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    for (int z = 0; z < 3; z++)
                    {
                        Vector3 rotated = quaternion * new Vector3(x - 1, y - 1, z - 1);

                        int xIdx = Mathf.RoundToInt(rotated.x) + 1;
                        int yIdx = Mathf.RoundToInt(rotated.y) + 1;
                        int zIdx = Mathf.RoundToInt(rotated.z) + 1;

                        matrix[x, y, z, 0] = xIdx;
                        matrix[x, y, z, 1] = yIdx;
                        matrix[x, y, z, 2] = zIdx;

                        inverseMatrix[xIdx, yIdx, zIdx, 0] = x;
                        inverseMatrix[xIdx, yIdx, zIdx, 1] = y;
                        inverseMatrix[xIdx, yIdx, zIdx, 2] = z;
                    }
        }

        public int[] Transform(int x, int y, int z)
        {
            return new int[]{
                matrix[x, y, z, 0],
                matrix[x, y, z, 1],
                matrix[x, y, z, 2],
            };
        }

        public int[] TransformInverse(int x, int y, int z)
        {
            return new int[]{
                inverseMatrix[x, y, z, 0],
                inverseMatrix[x, y, z, 1],
                inverseMatrix[x, y, z, 2],
            };
        }
    }
}
