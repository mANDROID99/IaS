using System;
using IaS.Domain;
using UnityEngine;

namespace IaS.World.Rotations
{
    public struct AxisConstraint
    {
        public enum AxisType { X, Y, Z }

        public readonly AxisType Axis;
        public readonly int Value;

        public AxisConstraint(int value, AxisType axisType)
        {
            Value = value;
            Axis = axisType;
        }

        public Quaternion Rotation(int clockwiseTurns)
        {
            switch (Axis)
            {
                case AxisType.X:
                    return Quaternion.Euler(new Vector3(90 * clockwiseTurns, 0, 0));
                case AxisType.Y:
                    return Quaternion.Euler(new Vector3(0, 90 * clockwiseTurns, 0));
                default:
                    return Quaternion.Euler(new Vector3(0, 0, 90 * clockwiseTurns));
            }
        }

        public bool IntersectedByBlockBounds(IntBlockBounds blockBounds)
        {
            switch (Axis)
            {
                case AxisType.X:
                    return blockBounds.MinX < Value && blockBounds.MaxX > Value;
                case AxisType.Y:
                    return blockBounds.MinY < Value && blockBounds.MaxY > Value;
                default:
                    return blockBounds.MinZ < Value && blockBounds.MaxZ > Value;
            }
        }

        public bool Encapsulates(IntBlockBounds blockBounds, bool left)
        {
            switch(Axis)
            {
                case AxisType.X:
                    return left ? blockBounds.MaxX <= Value : blockBounds.MinX >= Value;
                case AxisType.Y:
                    return left ? blockBounds.MaxY <= Value : blockBounds.MinY >= Value;
                default:
                    return left ? blockBounds.MaxZ <= Value : blockBounds.MaxZ >= Value; 
            }
        }


        public bool ContainsBounds(IntBlockBounds intBlockBounds)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Axis + ":" + Value;
        }

        public static AxisConstraint X(int value)
        {
            return new AxisConstraint(value, AxisType.X);
        }

        public static AxisConstraint Y(int value)
        {
            return new AxisConstraint(value, AxisType.Y);
        }

        public static AxisConstraint Z(int value)
        {
            return new AxisConstraint(value, AxisType.Z);
        }
    }
}
