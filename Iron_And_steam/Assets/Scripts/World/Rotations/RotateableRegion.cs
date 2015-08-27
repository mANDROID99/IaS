using IaS.Domain;
using UnityEngine;

namespace IaS.World.Rotations
{
    public interface RotateableRegion
    {
        Quaternion Rotation();
        IntBlockBounds RotatedBounds();
        void Rotate(AxisConstraint constraint, AxisPivot pivot, int clockwiseTurns);
    }
}
