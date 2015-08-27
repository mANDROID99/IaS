using UnityEngine;

namespace IaS.World.Rotations
{
    public struct AxisPivot
    {
        public Vector3 Pivot;
        public AxisConstraint.AxisType Axis;

        public AxisPivot(AxisConstraint.AxisType axis, Vector3 pivot)
        {
            Axis = axis;
            Pivot = pivot;
        }

        public static AxisPivot PivotX(Vector3 pivot)
        {
            return new AxisPivot(AxisConstraint.AxisType.X, pivot);
        }

        public static AxisPivot PivotY(Vector3 pivot)
        {
            return new AxisPivot(AxisConstraint.AxisType.Y, pivot);
        }

        public static AxisPivot PivotZ(Vector3 pivot)
        {
            return new AxisPivot(AxisConstraint.AxisType.Z, pivot);
        }
    }
}
