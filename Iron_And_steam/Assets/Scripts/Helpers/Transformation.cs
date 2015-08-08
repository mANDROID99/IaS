using UnityEngine;

namespace IaS.Helpers
{
    public abstract class Transformation
    {
        public static Transformation None = new RotateAroundPivotTransform(new Vector3(), Quaternion.identity);

        public abstract Vector3 Transform(Vector3 pt);
        public abstract Vector3 TransformVector(Vector3 vec);
    }

    public class RotateAroundPivotTransform : Transformation
    {
        private readonly Vector3 _pivot;
        private readonly Quaternion _quat;

        public RotateAroundPivotTransform(Vector3 pivot, Quaternion quat)
        {
            this._pivot = pivot;
            this._quat = quat;
        }

        public override Vector3 Transform(Vector3 pt)
        {
            return MathHelper.RotateAroundPivot(pt, _pivot, _quat);
        }

        public override Vector3 TransformVector(Vector3 vec)
        {
            return _quat * vec;
        }
    }
}
