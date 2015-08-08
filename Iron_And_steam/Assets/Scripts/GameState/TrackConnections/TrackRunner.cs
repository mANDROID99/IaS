using System;
using IaS.Domain;
using IaS.GameState;
using IaS.Helpers;
using IaS.WorldBuilder.Splines;
using UnityEngine;

namespace Assets.Scripts.GameState.TrackConnections
{
    public class TrackRunner
    {
        public readonly TrackConnectionResolver ConnectionResolver;
        public bool ReachedEnd { get; private set;}
        public bool NextConnection { get; private set; }

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
            _interpolator = new BezierSpline.LinearInterpolator(_trackConnection.TrackGroup.Spline, _reversed);
            _interpolator.Step(0);
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
        }

        public Vector3 CurrentPos
        {
            get
            {
                if (!_currentPt.HasValue)
                    throw new Exception("Invalid operation. Reached end of the track or something went wrong.");
                return _currentPt.Value.pt;
            }
        }

        public Vector3 CurrentForward
        {
            get
            {
                if (!_currentPt.HasValue)
                    throw new Exception("Invalid operation. Reached end of the track or something went wrong.");
                return _currentPt.Value.firstDerivative.normalized;
            }
        }

        public SubTrackGroup CurrentSubTrackGroup
        {
            get
            {
                if (!_currentPt.HasValue)
                    throw new Exception("Invalid operation. Reached end of the track or something went wrong.");
                return _trackConnection.TrackGroup;
            }
        }
    }
}
