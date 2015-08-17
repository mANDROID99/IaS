using System;
using UnityEngine;

namespace IaS.WorldBuilder.XML.mappers
{
    public class AxisMapper : IXmlValueMapper<Vector3>
    {
        public const string AxisX = "x";
        public const string AxisY = "y";
        public const string AxisZ = "z";

        public Vector3 Map(string from)
        {
            switch (from.ToLower())
            {
                case AxisX:
                    return Vector3.right;
                case AxisY:
                    return Vector3.up;
                case AxisZ:
                    return Vector3.forward;
                default:
                    throw new Exception(string.Format("Value '{0}' cannot be converted to an Axis", from));
            }
        }
    }
}