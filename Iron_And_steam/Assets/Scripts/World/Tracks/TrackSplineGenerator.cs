using System.Collections.Generic;
using IaS.Domain;
using IaS.Domain.Splines;
using IaS.Domain.Tracks;
using UnityEngine;

namespace IaS.World.Tracks
{
    public class TrackSplineGenerator
    {
        private static readonly float AnchorDist = 4 * (Mathf.Sqrt(2) - 1) / 3f;
        private TrackBuilderConfiguration config;

        public TrackSplineGenerator(TrackBuilderConfiguration config)
        {
            this.config = config;
        }

        public BezierSpline GenerateSpline(List<SubTrackNode> nodes)
        {
            int numNodes = nodes.Count;
            List<BezierSpline.BezierPoint> pts = new List<BezierSpline.BezierPoint>();

            if (numNodes > 1)
            {
                for (int i = 0; i < numNodes - 1; i++)
                {
                    SubTrackNode current = nodes[i];
                    SubTrackNode next = current.Next;
                    SubTrackNode previous = current.Previous;

                    Vector3 anchor1;
                    Vector3 anchor2;
                    if (Vector3.Angle(current.Forward, next.Forward) > 0.1f)
                    {
                        // corner
                        Vector3 pt1 = current.Position + GetOffset(current.Forward) + (current.Down * config.curveOffset);
                        Vector3 curveEnd = current.Position + GetOffset(-next.Forward) + (next.Down * config.curveOffset);
                        float radius = GetRadius(pt1, curveEnd);
                        anchor1 = pt1 + current.Forward * (radius * AnchorDist);
                        anchor2 = curveEnd - next.Forward * (radius * AnchorDist);

                        pts.Add(new BezierSpline.BezierPoint(pt1, curveEnd, anchor1, anchor2, 6));
                    }
                    else
                    {
                        bool currentIsEndPartOfCurve = current.Previous != null && (current.Position == previous.Position);
                        Vector3 pt1 = current.Position + GetOffset(currentIsEndPartOfCurve ? -current.Forward : current.Forward) + (current.Down * config.curveOffset);
                        Vector3 pt2 = next.Position + GetOffset(next.Forward) + (next.Down * config.curveOffset);

                        if (Vector3.Distance(pt1, pt2) < 0.1f)
                            continue;

                        GetLinearAnchors(pt1, pt2, out anchor1, out anchor2);
                        pts.Add(new BezierSpline.BezierPoint(pt1, pt2, anchor1, anchor2, 2));
                    }
                }
            }
                
            return new BezierSpline(pts.ToArray());
        }

        private void GetLinearAnchors(Vector3 pt1, Vector3 pt2, out Vector3 anchor1, out Vector3 anchor2)
        {
            anchor1 = pt1 + (pt2 - pt1) * 0.25f;
            anchor2 = pt2 - (pt2 - pt1) * 0.25f;
        }

        private Vector3 GetOffset(Vector3 forward)
        {
            return (new Vector3(0.5f, 0.5f, 0.5f) - forward / 2f);
        }

        private float GetRadius(Vector3 pt1, Vector3 pt2)
        {
            return (Mathf.Abs(pt1.x - pt2.x) + Mathf.Abs(pt1.y - pt2.y) + Mathf.Abs(pt1.z - pt2.z)) / 2f;
        }
    }
}
