using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameState.Rotation;
using IaS.Domain.WorldTree;
using IaS.Helpers;
using IaS.Scripts.Domain;
using IaS.WorldBuilder;
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

        public RotationAnimator[] Rotate(SplitSide splitSide, Direction direction)
        {
            GroupBranch group = splitSide.Group;
            List<RotateableBranch> branchesToRotate = new List<RotateableBranch>();

            foreach (RotateableBranch rotateable in group.RotateableBranches.Values)
            {
                RotationState rotationState = rotateable.RotationState;

                float distance;
                Split.ConstraintResult constraintResult = splitSide.Constrains(rotationState.RotatedBounds, out distance);
                switch (constraintResult)
                {
                    case Split.ConstraintResult.Blocked:
                        return new RotationAnimator[0];
                    case Split.ConstraintResult.Included:
                        branchesToRotate.Add(rotateable);
                        break;
                }
            }

            branchesToRotate.AddRange(splitSide.AttachedRotateables);

            float directionMult = direction == Direction.Clockwise ? 1 : -1;
            Quaternion rotationDelta = Quaternion.Euler(splitSide.Axis * 90 * directionMult);
            return CreateRotationAnimations(splitSide, branchesToRotate, rotationDelta);
        }

        private RotationAnimator[] CreateRotationAnimations(SplitSide split, List<RotateableBranch> rotateables, Quaternion rotationDelta)
        {
            List<RotationAnimator> animators = new List<RotationAnimator>();
            foreach (RotateableBranch rotateable in rotateables)
            {
                BlockBounds originalBounds = rotateable.OriginalBounds;
                BlockBounds rotatedBounds = rotateable.RotationState.RotatedBounds;
                Quaternion startRot = rotatedBounds.Rotation;
                Quaternion endRot = startRot * rotationDelta;

                Transformation transform = new RotateAroundPivotTransform(split.Pivot, endRot);
                _eventRegistry.Notify(new BlockRotationEvent(originalBounds, transform, BlockRotationEvent.EventType.BeforeRotation));

                rotatedBounds.SetToRotationFrom(endRot, split.Pivot, originalBounds);
                animators.Add(new RotationAnimator(split, startRot, endRot, rotateable));
            }
            return animators.ToArray();
        }
    }

    public struct BlockRotationEvent : IEvent
    {
        public enum EventType
        {
            Update, BeforeRotation, AfterRotation
        }

        public readonly BlockBounds RotatedBounds;
        public readonly Transformation Transformation;
        public readonly EventType Type;

        public BlockRotationEvent(BlockBounds rotatedBounds, Transformation transformation, EventType type)
        {
            RotatedBounds = rotatedBounds;
            Transformation = transformation;
            Type = type;
        }
    }
}
