using System.Linq;
using IaS.World.WorldTree;
using UnityEngine;

namespace IaS.Domain
{
    public class SplitSide
    {
        private readonly Split _split;
        public readonly bool Lhs;
        public readonly GroupBranch Group;
        public readonly GroupBranch[] AttachedRotateables;

        internal SplitSide(LevelTree levelTree, Split split, bool lhs)
        {
            _split = split;
            Lhs = lhs;
            Group = levelTree.GetGroupBranch(split.Group);
            AttachedRotateables = split.AttachedGroups
                .Where(attached => attached.Lhs == lhs)
                .Select(attached => levelTree.GetGroupBranch(attached.AttachedGroup))
                .ToArray();
        }

        public Vector3 Axis { get { return _split.Axis; } }

        public float Value { get { return _split.Value; } }

        public Vector3 Pivot { get { return _split.Pivot; } }

        public Split.ConstraintResult Constrains(BlockBounds blockBounds, out float distance)
        {
            return _split.Constrains(Lhs, blockBounds, out distance);
        }
    }
}
