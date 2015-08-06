using System;
using IaS.WorldBuilder.Splines;
using UnityEngine;

namespace IaS.GameState.TrackConnections
{
    public class InterpolateableConnection
    {
        internal readonly TrackConnectionResolver.TrackConnection WrappedConnection;
        public readonly bool Reversed;

        private readonly BezierSpline.LinearInterpolator _interpolator;
        private BezierSpline.BezierPtInfo? _currentPt;

        public InterpolateableConnection(TrackConnectionResolver.TrackConnection wrappedConnection, bool reverse)
        {
            WrappedConnection = wrappedConnection;
            Reversed = reverse;
            _interpolator = new BezierSpline.LinearInterpolator(wrappedConnection.TrackGroup.Spline, reverse);
        }

        public void Step(float amt)
        {
            _interpolator.Step(amt);
            _currentPt = _interpolator.Value();
        }

        public bool ReachedEnd
        {
            get { return _interpolator.ReachedEnd(); }
        }

        public Vector3 CurrentPos
        {
            get
            {
                if (!_currentPt.HasValue)
                    throw new Exception("Invalid operation. Reached end of the WrappedConnection.");
                return _currentPt.Value.pt;
            }
        }

        public Vector3 CurrentForward
        {
            get
            {
                if (!_currentPt.HasValue)
                    throw new Exception("Invalid operation. Reached end of the WrappedConnection.");
                return _currentPt.Value.firstDerivative.normalized;
            }
        }
    }
}
