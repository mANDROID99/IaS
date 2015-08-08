using Assets.Scripts.GameState.TrackConnections;
using IaS.Domain;
using IaS.GameState.WorldTree;
using UnityEngine;

namespace IaS.Controllers.GO
{
    public class TrackFollowingGameObject
    {
        public readonly GameObject GameObject;
        private readonly TrackRunner _trackRunner;
        private readonly GroupBranch _groupBranch;
        
        private Quaternion _splineRotation = Quaternion.identity;
        private Vector3 _lastSplineForward;

        public TrackFollowingGameObject(GroupBranch groupBranch, GameObject gameObject, TrackRunner trackRunner, Vector3 forward)
        {
            _lastSplineForward = forward;
            _groupBranch = groupBranch;
            GameObject = gameObject;
            _trackRunner = trackRunner;
            AttachToCurrentGroup();
            SetCurrentRotationAndPosition();
        }

        private void AttachToCurrentGroup()
        {
            SubTrack subTrack = _trackRunner.CurrentSubTrackGroup.SubTrack;
            BaseTree branch = _groupBranch.GetSplitBoundsBranch(subTrack.SplitBounds).OthersLeaf;
            branch.Attach(GameObject, true);
        }

        private void SetCurrentRotationAndPosition()
        {
            Vector3 splineForward = _trackRunner.CurrentForward;
            _splineRotation = Quaternion.FromToRotation(_lastSplineForward, splineForward) * _splineRotation;
            _lastSplineForward = splineForward;

            GameObject.transform.localRotation = _splineRotation;
            GameObject.transform.localPosition = _trackRunner.CurrentPos;
        }

        public void MoveUpdate(float stepAmt)
        {
            _trackRunner.Step(stepAmt);

            if (_trackRunner.NextConnection)
            {
               AttachToCurrentGroup();
            }

            SetCurrentRotationAndPosition();
        }
    }
}
