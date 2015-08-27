using IaS.World.Rotations;
using IaS.Helpers;
using UnityEngine;

namespace IaS.Domain
{
    public struct IntBlockBounds
    {
        public int MinX, MinY, MinZ, MaxX, MaxY, MaxZ;
        private Quaternion? _rotation;

        public IntBlockBounds(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MaxX = maxX;
            MaxY = maxY;
            MaxZ = maxZ;
            _rotation = null;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}, {2}:{3}, {4}:{5}", MinX, MaxX, MinY, MaxY, MinZ, MaxZ);
        }

        public Vector3 Min {
            get { return new Vector3(MinX, MinY, MinZ); }
            set {
                MinX = Mathf.RoundToInt(value.x);
                MinY = Mathf.RoundToInt(value.y);
                MinZ = Mathf.RoundToInt(value.z);
            }
        }

        public Vector3 Max
        {
            get { return new Vector3(MaxX, MaxY, MaxZ); }
            set
            {
                MaxX = Mathf.RoundToInt(value.x);
                MaxY = Mathf.RoundToInt(value.y);
                MaxZ = Mathf.RoundToInt(value.z);
            }
        }

        public Quaternion Rotation
        {
            get {
                return (_rotation ?? (_rotation = Quaternion.identity)).Value;
            }
        }

        internal void Rotate(Quaternion delta, AxisPivot pivot)
        {
            _rotation = delta * Rotation;
            Vector3 rotatedMin = MathHelper.RotateAroundPivot(Min, pivot.Pivot, delta);
            Vector3 rotatedMax = MathHelper.RotateAroundPivot(Max, pivot.Pivot, delta);

            Min = new Vector3(
                Mathf.Min(rotatedMin.x, rotatedMax.x), 
                Mathf.Min(rotatedMin.y, rotatedMax.y), 
                Mathf.Min(rotatedMin.z, rotatedMax.z));

            Max = new Vector3(
                Mathf.Max(rotatedMin.x, rotatedMax.x),
                Mathf.Max(rotatedMin.y, rotatedMax.y),
                Mathf.Max(rotatedMin.z, rotatedMax.z));
        }
    }
}
