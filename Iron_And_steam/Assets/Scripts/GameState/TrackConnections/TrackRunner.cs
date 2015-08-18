using IaS.Domain;
using IaS.Domain.WorldTree;
using IaS.GameState;
using IaS.WorldBuilder.Splines;
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

        private TrackConnectionResolver.TrackConnection _trackConnection;
        private bool _reversed;
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
            _trackConnection = ConnectionResolver.GetFirst(initialTrackGroup);
            _reversed = initialReverse;
            _interpolator = new BezierSpline.LinearInterpolator(initialTrackGroup.Spline, initialReverse);
            _interpolator.Step(0);
            _currentPt = _interpolator.Value();
            SetStartingValues();
        }

        private void NextInterpolation()
        {
            bool nextReversed;
            _trackConnection = ConnectionResolver.GetNext(_trackConnection, _reversed, out nextReversed);
            _reversed = nextReversed;

            if (_trackConnection == null)
            {
                ReachedEnd = true;
                _interpolator = null;
                return;
            }

            NextConnection = true;
            SetStartingValues();

            _interpolator = new BezierSpline.LinearInterpolator(_trackConnection.TrackGroup.Spline, _reversed);
            _interpolator.Step(0);
        }

        private void SetStartingValues()
        {
            SubTrackNode startNode = _trackConnection.TrackGroup.FirstNode(_reversed);
            CurrentForward = !_reversed ? startNode.Forward : -startNode.Forward;
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
                return _trackConnection.TrackGroup;
            }
        }

        public GroupBranch CurrentGroupBranch
        {
            get { return CurrentSubTrackGroup.SubTrack.SplitTrack.GroupBranch; }
        }
    }
}
