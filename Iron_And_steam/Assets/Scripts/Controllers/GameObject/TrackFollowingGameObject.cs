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
        private readonly Quaternion _prefabRotation;

        public TrackFollowingGameObject(GroupBranch groupBranch, GameObject gameObject, TrackRunner trackRunner, Vector3 prefabForward)
        {;
            _groupBranch = groupBranch;
            GameObject = gameObject;
            _trackRunner = trackRunner;

            _prefabRotation = Quaternion.FromToRotation(prefabForward, Vector3.forward);
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
            Quaternion rotation = Quaternion.LookRotation(_trackRunner.CurrentForward, _trackRunner.CurrentUp) * _prefabRotation;
            GameObject.transform.localRotation = rotation;
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
