using System;
using System.Linq;
using IaS.Domain;
using IaS.GameState.TrackConnections;
using IaS.Helpers;
using UnityEngine;

namespace Assets.Scripts.GameState.TrackConnections
{
    public class OneToOneConnectionFilter : ConnectionFilter
    {
        private readonly TrackConnection _connection;
        private Vector3 _startForward;
        private Vector3 _endForward;
        private Vector3 _startPos;
        private Vector3 _endPos;

        public OneToOneConnectionFilter(TrackConnection connection)
        {
            _connection = connection;
            Rotate(IdentityTransform.IDENTITY);
        }

        public void Rotate(Transformation transform)
        {
            SubTrackGroup trackGroup = _connection.TrackGroup;
            _startPos = transform.Transform(trackGroup.spline.pts[0].startPos);
            _endPos = transform.Transform(trackGroup.spline.pts.Last().endPos);
            _startForward = transform.TransformVector(trackGroup[0].forward);
            _endForward = transform.TransformVector(trackGroup.spline.pts.Last().endPos);
        }

        public bool Filter(TrackConnection connection, bool matchAgainstNext)
        {
            if (!matchAgainstNext)
            {
                return (Vector3.Distance(_startPos, connection.EndPos) < 0.1f) && (Vector3.Angle(_startForward, connection.EndForward) < 0.1f);
            }
            
            return (Vector3.Distance(_endPos, connection.StartPos) < 0.1f) && (Vector3.Angle(_endForward, connection.StartForward) < 0.1f);
        }
    }
}
