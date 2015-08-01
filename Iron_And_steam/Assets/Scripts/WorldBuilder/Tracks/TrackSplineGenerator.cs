using System.Collections.Generic;
using IaS.Domain;
using IaS.WorldBuilder.Splines;
using UnityEngine;

namespace IaS.WorldBuilder.Tracks
{
    public class TrackSplineGenerator
    {
        private static readonly float ANCHOR_DIST = 4 * (Mathf.Sqrt(2) - 1) / 3f;
        private TrackBuilderConfiguration config;

        public TrackSplineGenerator(TrackBuilderConfiguration config)
        {
            this.config = config;
        }

        public BezierSpline[] GenerateSplines(SplitTrack track, SubTrack subTrack)
        {
            List<BezierSpline> splines = new List<BezierSpline>();
            for (int groupIdx = 0; groupIdx < subTrack.TrackGroups.Length; groupIdx ++ )
            {
                SubTrackGroup group = subTrack.TrackGroups[groupIdx];
                if (group.NumTrackNodes < 2)
                    continue;

                List<BezierSpline.BezierPoint> pts = new List<BezierSpline.BezierPoint>();

                Vector3 anchor1, anchor2;

                for (int i = 0; i < group.NumTrackNodes - 1; i++)
                {
                    SubTrackNode current = group[i];
                    SubTrackNode next = current.next;
                    SubTrackNode previous = current.previous;

                    if(Vector3.Angle(current.forward, next.forward) > 0.1f)
                    {
                        // corner
                        Vector3 pt1 = current.position + GetOffset(current.forward) + (current.down * config.curveOffset);
                        Vector3 curveEnd = current.position + GetOffset(-next.forward) + (next.down * config.curveOffset);
                        float radius = GetRadius(pt1, curveEnd);
                        anchor1 = pt1 + current.forward * (radius * ANCHOR_DIST);
                        anchor2 = curveEnd - next.forward * (radius * ANCHOR_DIST);

                        pts.Add(new BezierSpline.BezierPoint(pt1, curveEnd, anchor1, anchor2, 6));
                    }
                    else
                    {
                        bool currentIsEndPartOfCurve = current.previous != null && (current.position == previous.position);
                        Vector3 pt1 = current.position + GetOffset(currentIsEndPartOfCurve ? -current.forward : current.forward) + (current.down * config.curveOffset);
                        Vector3 pt2 = next.position + GetOffset(next.forward) + (next.down * config.curveOffset);

                        if (Vector3.Distance(pt1, pt2) < 0.1f)
                            continue;

                        GetLinearAnchors(pt1, pt2, out anchor1, out anchor2);
                        pts.Add(new BezierSpline.BezierPoint(pt1, pt2, anchor1, anchor2, 2));
                    }
                }

                BezierSpline spline = new BezierSpline(pts.ToArray());
                group.spline = spline;
                splines.Add(spline);
            }

            return splines.ToArray();
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
