using System;
using System.Linq;
using UnityEngine;

namespace IaS.Domain.Splines
{
	public class BezierSpline {

        public class LinearInterpolator
        {
            private const int LUT_INTERVALS = 1000;
            private float[] lut = new float[LUT_INTERVALS];
            private BezierSpline _spline;

            private int _curveIdx;
            private float _totalDistance = 0;
            private float _lastDistance = 0;
            private float _dist;
            private BezierPoint _bezierPt;
            private bool _reverse;

            public LinearInterpolator(BezierSpline spline, bool reverse = false)
            {
                this._spline = spline;
                this.UpdateSpline(spline, reverse);
            }

            public void UpdateSpline(BezierSpline spline, bool reverse)
            {
                _spline = spline;
                _curveIdx = -1;
                _reverse = reverse;
                _totalDistance = 0;
                _dist = 0;

            }

            public void Step(float amt)
            {
                _dist += amt;
            }

            public bool ReachedEnd()
            {
                return _dist >= _totalDistance;
            }

            private int ReversedCurveIndex()
            {
                return _reverse ? _spline.pts.Length - (1 + _curveIdx) : _curveIdx;
            }

            private float ReversedDistance()
            {
                return _reverse ? _totalDistance - (_dist - _lastDistance) : _dist;
            }

            public BezierPtInfo? Value()
            {
                if ((_curveIdx == -1) || (_dist >= _totalDistance))
                {
                    _curveIdx += 1;

                    if (_curveIdx >= _spline.pts.Length)
                    {
                        return null;
                    }

                    _bezierPt = _spline.pts[ReversedCurveIndex()];
                    _lastDistance = _totalDistance;
                    _totalDistance = UpdateLUT(_totalDistance, _bezierPt);
                }

                BezierPtInfo linearEntry = GetPointAtLinear(ReversedDistance(), lut, _bezierPt);
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
                Vector3 firstDerivitive = _spline.GetFirstDerivative(t, bezierPt);
                if (_reverse) firstDerivitive *= -1f;
                return new BezierPtInfo
                {
                    firstDerivative = firstDerivitive,
                    cumulativeDist = distance,
                    pt = _spline.GetPoint(t, bezierPt),
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
                    Vector3 pt2 = _spline.GetPoint(t, pt);
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
            return string.Join("\n", pts.Select(pt => pt.ToString()).ToArray());
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
                return string.Format("(p:{0}, p2:{1}, a1:{2}, a2:{3})", startPos, endPos, anchor1, anchor2);
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
