using System;
using UnityEngine;

namespace IaS.Helpers
{
    class MathHelper
    {
        public const float STANDARD_ERROR = 0.001f;

        public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion quat)
        {
            Vector3 distance = point - pivot;
            Vector3 result = quat * distance;
            return result + pivot;
        }

        public static Vector3 RoundVector3ToDp(Vector3 vec, int dp)
        {
            return new Vector3(
                RoundToDp(vec.x, dp),
                RoundToDp(vec.y, dp),
                RoundToDp(vec.z, dp));
        }

		public static float RoundToDp(float val, int dp)
		{
			double div = Math.Pow (10, dp);
            return (float)((Math.Round((double)val * div) / div));
		}

        public static bool VectorsEqualWError(Vector3 a, Vector3 b, float error = STANDARD_ERROR)
        {
            Vector3 vec = a - b;
            vec.x *= vec.x;
            vec.y *= vec.y;
            vec.z *= vec.z;

            float errorSquared = error * error;
            return vec.x < errorSquared && 
                vec.y < errorSquared && 
                vec.z < errorSquared;
        }

        public static void SetMatrixToIdentity(Matrix4x4 mat)
        {
            for(int x = 0; x < 4; x++){
                for(int y = 0; y < 4; y++){
                    mat[x, y] = x == y ? 1 : 0;
                }
            }
        }

    }
}
