using IaS.Domain;
using IaS.World.WorldTree;
using IaS.GameState;
using IaS.Domain.Splines;
using UnityEngine;

namespace Assets.Scripts.GameState.TrackConnections
{
    public class TrackRunner
    {
        public readonly TrackConnectionResolver ConnectionResolver;
        public bool ReachedEnd { get; private set;}
        public bool NextConnection { get; private set; }
        public Vector3 CurrentForward { get; private set; }
        public Vector3 CurrentUp { get; private set; }
        public Vector3 CurrentPos { get; private set; }

        private TrackConnectionResolver.ConnectionContext _connectionCtx;
        private BezierSpline.LinearInterpolator _interpolator;
        private BezierSpline.BezierPtInfo? _currentPt = null;

        public TrackRunner(TrackConnectionResolver connectionResolver, SubTrackGroup initialTrackGroup, bool initialReverse)
        {
            ConnectionResolver = connectionResolver;
            Setup(initialTrackGroup, initialReverse);
        }

        private void Setup(SubTrackGroup initialTrackGroup, bool initialReverse)
        {
            NextConnection = true;
            _connectionCtx = ConnectionResolver.FirstContext(initialTrackGroup, initialReverse);
            _interpolator = new BezierSpline.LinearInterpolator(initialTrackGroup.Spline, initialReverse);
            _interpolator.Step(0);
            _currentPt = _interpolator.Value();
            SetStartingValues();
        }

        private void NextInterpolation()
        {
            _connectionCtx = ConnectionResolver.GetNext(_connectionCtx);

            if (_connectionCtx == null)
            {
                ReachedEnd = true;
                _interpolator = null;
                return;
            }

            NextConnection = true;
            SetStartingValues();

            _interpolator = new BezierSpline.LinearInterpolator(_connectionCtx.TrackGroup.Spline, _connectionCtx.Reversed);
            _interpolator.Step(0);
        }

        private void SetStartingValues()
        {
            bool reversed = _connectionCtx.Reversed;
            SubTrackNode startNode = _connectionCtx.TrackGroup.FirstNode(reversed);
            CurrentForward = !reversed ? startNode.Forward : -startNode.Forward;
            CurrentUp = -startNode.Down;
            CurrentPos = startNode.Position;
        }

        private void UpdateValues()
        {
            if (_currentPt.HasValue)
            {
                Vector3 nextForward = _currentPt.Value.firstDerivative.normalized;
                Quaternion rot = Quaternion.FromToRotation(CurrentForward, nextForward);
                CurrentForward = rot * CurrentForward;
                CurrentUp = rot * CurrentUp;
                CurrentPos = _currentPt.Value.pt;
            }
        }

        public void Step(float amt)
        {
            NextConnection = false;
            if (ReachedEnd) return;

            _interpolator.Step(amt);

            while (!ReachedEnd)
            {
                _currentPt = _interpolator.Value();
                if (!_currentPt.HasValue)
                {
                    NextInterpolation();
                }
                else
                {
                    break;
                }
            }

            UpdateValues();
        }

        public SubTrackGroup CurrentSubTrackGroup
        {
            get
            {
                return _connectionCtx.TrackGroup;
            }
        }

        public GroupBranch CurrentGroupBranch
        {
            get { return CurrentSubTrackGroup.SubTrack.SplitTrack.GroupBranch; }
        }
    }
}
