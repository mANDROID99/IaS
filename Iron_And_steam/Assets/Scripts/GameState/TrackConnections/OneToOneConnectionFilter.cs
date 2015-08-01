using System.Linq;
using IaS.Domain;
using IaS.GameState.TrackConnections;
using IaS.Helpers;
using UnityEngine;

namespace IaS.GameState.TrackConnections
{
    public class OneToOneConnectionFilter
    {
        public class StartFilter : IStartConnectionFilter
        {
            protected readonly SubTrackGroup TrackGroup;
            protected Vector3 StartForward;
            protected Vector3 StartPos;

            public StartFilter(SubTrackGroup trackGroup)
            {
                TrackGroup = trackGroup;
                Rotate(IdentityTransform.IDENTITY);
            }

            public SubTrackGroup GetSubTrackGroup()
            {
                return TrackGroup;
            }

            public Vector3 GetStartPos()
            {
                return StartPos;
            }

            public Vector3 GetStartForward()
            {
                return StartForward;
            }

            public void Rotate(Transformation transform)
            {
                StartPos = transform.Transform(TrackGroup.StartBezierPos);
                StartForward = transform.TransformVector(TrackGroup.StartForward);
            }

            public virtual bool AllowPrevious(IEndConnectionFilter previous)
            {
                return (Vector3.Distance(StartPos, previous.GetEndPos()) < 0.1f) && (Vector3.Angle(StartForward, previous.GetEndForward()) < 0.1f);
            }
        }

        public class EndFilter : IEndConnectionFilter
        {
            protected readonly SubTrackGroup TrackGroup;
            protected Vector3 EndForward;
            protected Vector3 EndPos;

            public EndFilter(SubTrackGroup trackGroup)
            {
                TrackGroup = trackGroup;
                Rotate(IdentityTransform.IDENTITY);
            }

            public SubTrackGroup GetSubTrackGroup()
            {
                return TrackGroup;
            }

            public Vector3 GetEndPos()
            {
                return EndPos;
            }

            public Vector3 GetEndForward()
            {
                return EndForward;
            }

            public void Rotate(Transformation transform)
            {
                EndPos = transform.Transform(TrackGroup.EndBezierPos);
                EndForward = transform.TransformVector(TrackGroup.EndForward);
            }

            public virtual bool AllowNext(IStartConnectionFilter next)
            {
                return (Vector3.Distance(EndPos, next.GetStartPos()) < 0.1f) && (Vector3.Angle(EndForward, next.GetStartForward()) < 0.1f);
            }
        }
    }
}
