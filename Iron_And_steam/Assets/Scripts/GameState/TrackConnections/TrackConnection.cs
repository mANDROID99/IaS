using System.Linq;
using IaS.Domain;
using IaS.Helpers;
using UnityEngine;

namespace IaS.GameState.TrackConnections
{
    public class TrackConnection
    {
        internal readonly SubTrackGroup TrackGroup;
        internal TrackConnection NextConnection = null;
        internal TrackConnection PrevConnection = null;
        internal Vector3 StartPos;
        internal Vector3 EndPos;
        internal Vector3 StartForward;
        internal Vector3 EndForward;
        internal Transformation Transform;

        internal TrackConnection(SubTrackGroup trackGroup)
        {
            TrackGroup = trackGroup;
            StartPos = trackGroup.spline.pts[0].startPos;
            EndPos = trackGroup.spline.pts.Last().endPos;
            StartForward = trackGroup[0].forward;
            EndForward = trackGroup.Last().forward;
            Transform = IdentityTransform.IDENTITY;
        }

        internal void RotateStartAndEndPoints(Transformation transform)
        {
            StartPos = transform.Transform(TrackGroup.spline.pts[0].startPos);
            EndPos = transform.Transform(TrackGroup.spline.pts.Last().endPos);
            StartForward = transform.TransformVector(TrackGroup[0].forward);
            EndForward = transform.TransformVector(TrackGroup.Last().forward);
            Transform = transform;
        }
    }
}
