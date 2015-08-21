using System.Collections.Generic;
using System.Linq;
using IaS.Domain.WorldTree;
using IaS.GameState.Rotation;
using IaS.Xml;
using UnityEngine;

namespace IaS.Domain
{
    public class Split : IReferenceable
    {

        public enum RestrictionType { Above, Below, Both }
        public enum ConstraintResult{ Outside, Blocked, Included }

        public readonly List<SplitAttachment> AttachedGroups = new List<SplitAttachment>();
        public readonly Group Group;
        public readonly Vector3 Axis;
        public readonly Vector3 Pivot;
        public readonly float Value;
        public readonly string Id;
        public readonly RestrictionType Restriction;

        public Split(string id, Group group, Vector3 axis, Vector3 pivot, float value, RestrictionType restriction)
        {
            Axis = axis;
            Pivot = pivot;
            Value = value;
            Id = id;
            Group = group;
            Restriction = restriction;
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

        public ConstraintResult Constrains(bool lhs, BlockBounds bounds, out float distance)
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
                distance = 0;
                return ConstraintResult.Blocked;
            }

            distance = GetDistFromSplit(lhs, splitVal, min, max);
            return distance >= 0 ? ConstraintResult.Included : ConstraintResult.Outside;
        }

        private bool BlockedByBounds(float split, float min, float max)
        {
            return split > min && split < max;
        }

        private float GetDistFromSplit(bool lhs, float split, float min, float max)
        {
            return lhs ? split - max : min - split;
        }

        public IEnumerable<SplitSide> GetActiveSides(LevelTree levelTree)
        {
            switch (Restriction)
            {
                case RestrictionType.Below:
                    return new[] { new SplitSide(levelTree, this, true)  };
                case RestrictionType.Above:
                    return new[] { new SplitSide(levelTree, this, false) };
                default:
                    return new[] { new SplitSide(levelTree, this, true), new SplitSide(levelTree, this, false), };
            }
        }

        public string GetId()
        {
            return Id;
        }
    }
}

