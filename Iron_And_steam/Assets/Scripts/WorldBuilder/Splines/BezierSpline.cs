using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace IaS.WorldBuilder.Splines
{
	public class BezierSpline {

        public class LinearInterpolator
        {
            private const int LUT_INTERVALS = 1000;
            private float[] lut = new float[LUT_INTERVALS];
            private BezierSpline spline;

            private int curveIdx;
            private float totalDistance, dist;
            private BezierPoint bezierPt;

            public LinearInterpolator(BezierSpline spline)
            {
                this.spline = spline;
                this.UpdateSpline(spline);
            }

            public void UpdateSpline(BezierSpline spline)
            {
                this.spline = spline;
                this.curveIdx = -1;
                this.totalDistance = 0;
                this.dist = 0;
            }

            public void Step(float amt)
            {
                dist += amt;
            }

            public BezierPtInfo? Value()
            {
                if ((curveIdx == -1) || (dist >= totalDistance))
                {
                    curveIdx += 1;

                    if (curveIdx >= spline.pts.Length)
                    {
                        return null;
                    }

                    bezierPt = spline.pts[curveIdx];
                    totalDistance = UpdateLUT(totalDistance, bezierPt);
                }

                BezierPtInfo linearEntry = GetPointAtLinear(dist, lut, bezierPt);
                return linearEntry;
            }

            private BezierPtInfo GetPointAtLinear(float distance, float[] lut, BezierPoint bezierPt)
            {
                int idx = Array.BinarySearch(lut, distance);
                if (idx < 0)
                {
                    idx = ~idx;
                }

                if (idx == 0)
                {
                    return CreateBezierPtInfo(distance, 0, bezierPt);
                }
                else if (idx == lut.Length)
                {
                    return CreateBezierPtInfo(distance, 1, bezierPt);
                }

                float minDistance = lut[idx - 1];
                float maxDistance = lut[idx];
                float t2 = (distance - minDistance) / (maxDistance - minDistance);

                float oneOverLUTSize = 1 / (float)lut.Length;
                float t = LinearInterpolate((idx - 1) * oneOverLUTSize, (idx) * oneOverLUTSize, t2);
                return CreateBezierPtInfo(distance, t, bezierPt);
            }

            private BezierPtInfo CreateBezierPtInfo(float distance, float t, BezierPoint bezierPt)
            {
                return new BezierPtInfo
                {
                    firstDerivative = spline.GetFirstDerivative(t, bezierPt),
                    cumulativeDist = distance,
                    pt = spline.GetPoint(t, bezierPt),
                    t = t
                };
            }

            private float LinearInterpolate(float a, float b, float i)
            {
                return a + (b - a) * i;
            }

            private Vector3 LinearInterpolate(Vector3 a, Vector3 b, float i)
            {
                return a + (b - a) * i;
            }

            private float UpdateLUT(float distance, BezierPoint pt)
            {
                Vector3 pt1 = pt.startPos;
                lut[0] = distance;

                int lutIntervals = lut.Length;
                for (int i = 1; i < lutIntervals; i++)
                {
                    float t = i / (float)lutIntervals;
                    Vector3 pt2 = spline.GetPoint(t, pt);
                    distance += Vector3.Distance(pt2, pt1);
                    lut[i] = distance;
                    pt1 = pt2;
                }
                return distance;
            }
        }

        public BezierPoint[] pts { get; private set; }

        public BezierSpline(BezierPoint[] pts)
        {
            this.pts = pts;
        }

        public LinearInterpolator linearInterpolator()
        {
            return new LinearInterpolator(this);
        }

        public Vector3 GetPoint(float t, BezierPoint pt)
		{
			t = Mathf.Clamp01 (t);
			float oneMinusT = 1f - t;
			return oneMinusT * oneMinusT * oneMinusT * pt.startPos +
				3f * oneMinusT * oneMinusT * t * pt.anchor1 +
					3f * oneMinusT * t * t * pt.anchor2 + 
					t * t * t * pt.endPos;
		}

		public Vector3 GetFirstDerivative (float t, BezierPoint pt1) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			
			return
				3f * oneMinusT * oneMinusT * (pt1.anchor1 - pt1.startPos) +
					6f * oneMinusT * t * (pt1.anchor2 - pt1.anchor1) +
					3f * t * t * (pt1.endPos - pt1.anchor2);
		}

        public override string ToString() {
            return String.Join("\n", pts.Select(pt => pt.ToString()).ToArray());
        }

		[Serializable]
		public class BezierPoint
		{
			[SerializeField]
			public Vector3 startPos;
            [SerializeField]
            public Vector3 endPos;
			[SerializeField]
			public Vector3 anchor1;
            [SerializeField]
            public Vector3 anchor2;
            [SerializeField]
            public int numSubdivisions;

            public BezierPoint() { }

			public BezierPoint(Vector3 startPos, Vector3 endPos, Vector3 anchor1, Vector3 anchor2, int numSubdivisions)
			{
				this.startPos = startPos;
                this.endPos = endPos;
                this.anchor1 = anchor1;
                this.anchor2 = anchor2;
                this.numSubdivisions = numSubdivisions;
			}

            public override string ToString()
            {
                return String.Format("(p:{0}, p2:{1}, a1:{2}, a2:{3})", startPos, endPos, anchor1, anchor2);
            }
		}

        public struct BezierPtInfo : IComparable<BezierPtInfo>
        {
            public float cumulativeDist;
            public Vector3 firstDerivative;
            public Vector3 pt;
            public float t;

            public int CompareTo(BezierPtInfo obj)
            {
                if (cumulativeDist == obj.cumulativeDist)
                {
                    return 0;
                }
                else
                {
                    return cumulativeDist > obj.cumulativeDist ? 1 : -1;
                }
            }
        }
	}
}
