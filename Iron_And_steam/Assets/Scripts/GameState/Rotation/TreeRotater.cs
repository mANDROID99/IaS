using System.Collections.Generic;
using Assets.Scripts.GameState.Rotation;
using IaS.World.WorldTree;
using IaS.Helpers;
using IaS.Scripts.Domain;
using IaS.Domain;
using UnityEngine;

namespace IaS.GameState.Rotation
{
    public class TreeRotater
    {
        public enum Direction
        {
            Clockwise, CounterClockwise

        }

        private EventRegistry _eventRegistry;
        private LevelTree _levelTree;

        public TreeRotater(LevelTree levelTree, EventRegistry eventRegistry)
        {
            _eventRegistry = eventRegistry;
            _levelTree = levelTree;
        }

        public static Direction DirectionFromInt(int i)
        {
            return i == 1 ? Direction.Clockwise : Direction.CounterClockwise;
        }

        public IList<RotationAnimator> Rotate(SplitSide splitSide, Direction direction)
        {
            float directionMult = direction == Direction.Clockwise ? 1 : -1;
            Quaternion rotationDelta = Quaternion.Euler(splitSide.Axis * 90 * directionMult);

            GroupBranch group = splitSide.Group;

            List<RotationAnimator> animators = new List<RotationAnimator>();
            foreach (SplitBoundsBranch rotateable in group.SplitBoundsBranches)
            {
                RotationState rotationState = rotateable.RotationState;

                float distance;
                Split.ConstraintResult constraintResult = splitSide.Constrains(rotationState.RotatedBounds, out distance);
                switch (constraintResult)
                {
                    case Split.ConstraintResult.Blocked:
                        return new RotationAnimator[0];
                    case Split.ConstraintResult.Included:
                        Transformation transformation;
                        RotationAnimator animator = CreateRotationAnimation(splitSide, rotateable, rotationDelta, out transformation);
                        _eventRegistry.Notify(new BlockRotationEvent(group.Group, transformation, rotateable.BlockBounds));
                        animators.Add(animator);
                        break;
                }
            }

            foreach(GroupBranch attachedGroup in splitSide.AttachedRotateables)
            {
                Transformation transformation;
                RotationAnimator animator = CreateRotationAnimation(splitSide, attachedGroup, rotationDelta, out transformation);
                _eventRegistry.Notify(new GroupRotationEvent(group.Group, transformation));
                animators.Add(animator);
            }

            return animators;
        }

        private RotationAnimator CreateRotationAnimation(SplitSide splitSide, RotateableBranch rotateable, Quaternion rotationDelta, out Transformation transformation)
        {
            BlockBounds rotatedBounds = rotateable.RotationState.RotatedBounds;
            Quaternion startRot = rotatedBounds.Rotation;
            Quaternion endRot = startRot * rotationDelta;

            rotatedBounds.SetToRotationFrom(endRot, splitSide.Pivot, rotateable.Bounds);
            transformation = new RotateAroundPivotTransform(splitSide.Pivot, endRot);
            return new RotationAnimator(splitSide, startRot, endRot, rotateable);
        }
    }

    public struct GroupRotationEvent : IEvent
    {

        public readonly Group Group;
        public readonly Transformation Transformation;

        public GroupRotationEvent(Group group, Transformation transformation)
        {
            Group = group;
            Transformation = transformation;
        }
    }

    public struct BlockRotationEvent : IEvent
    {

        public readonly Group Group;
        public readonly Transformation Transformation;
        public readonly BlockBounds Block;

        public BlockRotationEvent(Group group, Transformation transformation, BlockBounds block)
        {
            Group = group;
            Transformation = transformation;
            Block = block;
        }
    }
}
