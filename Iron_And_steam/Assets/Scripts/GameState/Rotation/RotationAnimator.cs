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

        public RotationAnimator(SplitSide splitSide, Quaternion from, Quaternion to, RotateableBranch rotateable)
        {
            SplitSide = splitSide;
            Rotateable = rotateable;
            From = from;
            To = to;
            Delta = 0;
        }

        public void UpdateTransform(Quaternion lerpRotation, Vector3 localPosition)
        {
            Rotateable.Node.transform.localRotation = lerpRotation;
            Rotateable.Node.transform.localPosition = localPosition;
        }
    }
}
