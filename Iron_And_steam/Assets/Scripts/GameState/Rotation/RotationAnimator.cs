using IaS.Domain.WorldTree;
using IaS.GameState.Rotation;
using UnityEngine;

namespace Assets.Scripts.GameState.Rotation
{
    public struct RotationAnimator
    {
        public readonly Quaternion From;
        public readonly Quaternion To;
        public readonly RotateableBranch Rotateable;
        public readonly SplitSide SplitSide;
        public int Delta;

        public RotationAnimator(SplitSide splitSide, RotateableBranch rotateable, Quaternion rotationDelta)
        {
            SplitSide = splitSide;
            Rotateable = rotateable;
            From = rotateable.RotationState.RotatedBounds.Rotation;
            To = rotationDelta * From;
            Delta = 0;
        }

        public void UpdateTransform(Quaternion lerpRotation, Vector3 localPosition)
        {
            Rotateable.Node.transform.localRotation = lerpRotation;
            Rotateable.Node.transform.localPosition = localPosition;
        }
    }
}
