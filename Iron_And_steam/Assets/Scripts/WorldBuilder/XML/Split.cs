using UnityEngine;

namespace IaS.WorldBuilder
{
    public class Split
    {
        public enum ConstraintResult
        {
            Unconstrained, Blocked, Constrained
        }

        public readonly Vector3 Axis;
        public readonly Vector3 Pivot;
        public readonly float Value;
        public readonly string Id;

        public Split(string id, Vector3 axis, Vector3 pivot, float value)
        {
            Axis = axis;
            Pivot = pivot;
            Value = value;
            Id = id;
        }


        public float DistanceFromSplit(Vector3 min)
        {
            if (Axis.x > 0)
            {
                return min.x - Value;
            }
            else if (Axis.y > 0)
            {
                return min.y - Value;
            }
            else
            {
                return min.z - Value;
            }
        }

        public float Constrains(bool lhs, BlockBounds bounds, out ConstraintResult constraintResult)
        {
            Vector3 splitAxisVal = Axis * Value;
            float splitVal, min, max;
            if (splitAxisVal.x > 0)
            {
                splitVal = splitAxisVal.x;
                min = bounds.MinX;
                max = bounds.MaxX;
            }
            else if (splitAxisVal.y > 0)
            {
                splitVal = splitAxisVal.y;
                min = bounds.MinY;
                max = bounds.MaxY;
            }
            else
            {
                splitVal = splitAxisVal.z;
                min = bounds.MinZ;
                max = bounds.MaxZ;
            }

            if (BlockedByBounds(splitVal, min, max))
            {
                constraintResult = ConstraintResult.Blocked;
                return -1;
            }

            float splitDistance = GetDistFromSplit(lhs, splitVal, min, max);
            constraintResult = splitDistance >= 0 ? ConstraintResult.Constrained : ConstraintResult.Unconstrained;

            return splitDistance;

        }

        private bool BlockedByBounds(float split, float min, float max)
        {
            return split > min && split < max;
        }

        private float GetDistFromSplit(bool lhs, float split, float min, float max)
        {
            return lhs ? split - max : min - split;
        }

        public Split Copy()
        {
            return new Split(Id, Axis, Pivot, Value);
        }
    }
}

