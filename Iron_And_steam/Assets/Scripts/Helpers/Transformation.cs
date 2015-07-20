using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace IaS.Helpers
{
    public interface Transformation
    {
        Vector3 Transform(Vector3 pt);
        Vector3 TransformVector(Vector3 vec);
    }

    public class IdentityTransform : Transformation
    {
        public static readonly Transformation IDENTITY = new IdentityTransform();

        private IdentityTransform() { }

        public Vector3 Transform(Vector3 pt)
        {
            return pt;
        }

        public Vector3 TransformVector(Vector3 vec)
        {
            return vec;
        }
    }

    public class RotateAroundPivotTransform : Transformation
    {
        private Vector3 pivot;
        public Quaternion quat { get; private set; }

        public RotateAroundPivotTransform(Vector3 pivot, Quaternion quat)
        {
            this.pivot = pivot;
            this.quat = quat;
        }

        public Vector3 Transform(Vector3 pt)
        {
            return MathHelper.RotateAroundPivot(pt, pivot, quat);
        }

        public Vector3 TransformVector(Vector3 vec)
        {
            return quat * vec;
        }
    }
}
