using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.Domain.WorldTree;
using IaS.GameState.Rotation;
using UnityEngine;

namespace IaS.WorldBuilder
{
    public class Split
    {

        public enum RestrictionType
        {
            Above, Below, Both
        }

        public enum ConstraintResult
        {
            Outside, Blocked, Included
        }

        public readonly SplitAttachment[] AttachedGroups;
        public readonly string GroupId;
        public readonly Vector3 Axis;
        public readonly Vector3 Pivot;
        public readonly float Value;
        public readonly string Id;
        public readonly RestrictionType Restriction;

        public Split(string id, string groupId, SplitAttachment[] attachedGroups, Vector3 axis, Vector3 pivot, float value,RestrictionType restriction)
        {
            Axis = axis;
            Pivot = pivot;
            Value = value;
            Id = id;
            GroupId = groupId;
            Restriction = restriction;
            AttachedGroups = attachedGroups;
        }

        public GroupBranch GetGroup(LevelTree levelTree)
        {
            return levelTree.GetGroupBranch(GroupId);
        }

        public RotateableBranch[] GetAttachedGroups(LevelTree levelTree, bool lhs)
        {
            return AttachedGroups.Where(g => g.Lhs == lhs)
                .Select(g => levelTree.GetGroupBranch(g.AttachedGroupId))
                .Cast<RotateableBranch>().ToArray();
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
    }
}

