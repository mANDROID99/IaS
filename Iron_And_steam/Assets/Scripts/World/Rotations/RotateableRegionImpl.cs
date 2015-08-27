using System;
using IaS.Domain;
using UnityEngine;

namespace IaS.World.Rotations
{
    public class RotateableRegionImpl : RotateableRegion
    {

        private IntBlockBounds _blockBounds;

        public RotateableRegionImpl(IntBlockBounds blockBounds)
        {
            _blockBounds = blockBounds;
        }

        public void Rotate(AxisConstraint constraint, AxisPivot pivot, int clockwiseTurns)
        {
            Quaternion delta = constraint.Rotation(clockwiseTurns);
            _blockBounds.Rotate(delta, pivot);
        }

        public IntBlockBounds RotatedBounds()
        {
            return _blockBounds;
        }

        public Quaternion Rotation()
        {
            return _blockBounds.Rotation;
        }

        
    }
}
