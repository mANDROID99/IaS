using IaS.Domain;
using IaS.World.Rotations;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IaS.World
{
    public class RotateableRegionResolver
    {

        private IList<RotateableRegion> _rotateableRegions;
        private IList<AxisPivot> _pivots;

        public RotateableRegionResolver(IList<RotateableRegion> rotateableRegions, IList<AxisPivot> pivots)
        {
            _rotateableRegions = rotateableRegions;
            _pivots = pivots;
        }

        public IList<RotateableRegion> Rotate(AxisConstraint constraint, bool left, int clockwiseTurns)
        {
            AxisPivot pivot = _pivots.First(p => p.Axis == constraint.Axis);

            List<RotateableRegion> rotatedRegions = new List<RotateableRegion>();
            foreach(RotateableRegion region in _rotateableRegions)
            {
                if(constraint.Encapsulates(region.RotatedBounds(), left))
                {
                    rotatedRegions.Add(region);
                    region.Rotate(constraint, pivot, clockwiseTurns);
                }
            }

            return rotatedRegions;
        }

        public IList<AxisConstraint> GetAvailableSplits()
        {
            HashSet<AxisConstraint> availableSplits = new HashSet<AxisConstraint>();

            AxisConstraint? minX = null, minY = null, minZ = null, maxX = null, maxY = null, maxZ = null;

            foreach(RotateableRegion region in _rotateableRegions)
            {
                IntBlockBounds rotatedBounds = region.RotatedBounds();
                if (HasPivot(AxisConstraint.AxisType.X))
                {
                    availableSplits.Add(UpdateMinMax(ref minX, ref maxX, AxisConstraint.X(rotatedBounds.MinX)));
                    availableSplits.Add(UpdateMinMax(ref minX, ref maxX, AxisConstraint.X(rotatedBounds.MaxX)));
                }
                if (HasPivot(AxisConstraint.AxisType.Y))
                {
                    availableSplits.Add(UpdateMinMax(ref minY, ref maxY, AxisConstraint.Y(rotatedBounds.MinY)));
                    availableSplits.Add(UpdateMinMax(ref minY, ref maxY, AxisConstraint.Y(rotatedBounds.MaxY)));
                }
                if (HasPivot(AxisConstraint.AxisType.Z))
                {
                    availableSplits.Add(UpdateMinMax(ref minZ, ref maxZ, AxisConstraint.Z(rotatedBounds.MinZ)));
                    availableSplits.Add(UpdateMinMax(ref minZ, ref maxZ, AxisConstraint.Z(rotatedBounds.MaxZ)));
                }
            }

            availableSplits.RemoveWhere(axisConstraint =>
                axisConstraint.Equals(minX) ||
                axisConstraint.Equals(maxX) ||
                axisConstraint.Equals(minY) ||
                axisConstraint.Equals(maxY) ||
                axisConstraint.Equals(minZ) ||
                axisConstraint.Equals(maxZ) ||
                _rotateableRegions.Any(region => axisConstraint.IntersectedByBlockBounds(region.RotatedBounds()))
            );

            return availableSplits.ToList();
        }

        private bool HasPivot(AxisConstraint.AxisType axis)
        {
            return _pivots.Any(pivot => pivot.Axis == axis);
        }

        private AxisConstraint UpdateMinMax(ref AxisConstraint? min, ref AxisConstraint? max, AxisConstraint constraint)
        {
            if((!min.HasValue) || (min.Value.Value > constraint.Value))
            {
                min = constraint;
            }

            if((!max.HasValue) || (max.Value.Value < constraint.Value))
            {
                max = constraint;
            }
            return constraint;
        }
    }
}
