using IaS.Domain.WorldTree;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.GameState.Rotation
{
    public class SplitSide
    {
        private readonly Split _split;
        public readonly bool Lhs;
        public readonly GroupBranch Group;
        public readonly RotateableBranch[] AttachedRotateables;

        internal SplitSide(LevelTree levelTree, Split split, bool lhs)
        {
            _split = split;
            Lhs = lhs;
            Group = split.GetGroup(levelTree);
            AttachedRotateables = split.GetAttachedGroups(levelTree, lhs);
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
