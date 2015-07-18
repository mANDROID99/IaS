using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class DirectionParser
    {
        public const string DIR_STRING_FORWARD = "forward";
        public const string DIR_STRING_BACK = "back";
        public const string DIR_STRING_RIGHT = "right";
        public const string DIR_STRING_LEFT = "left";
        public const string DIR_STRING_UP = "up";
        public const string DIR_STRING_DOWN = "down";

        public static Vector3 ParseDirection(String directionStr)
        {
            switch (directionStr)
            {
                case DIR_STRING_FORWARD:
                    return Vector3.forward;
                case DIR_STRING_BACK:
                    return Vector3.back;
                case DIR_STRING_LEFT:
                    return Vector3.left;
                case DIR_STRING_RIGHT:
                    return Vector3.right;
                case DIR_STRING_UP:
                    return Vector3.up;
                case DIR_STRING_DOWN:
                    return Vector3.down;
            }
            throw new Exception(String.Format("Cannot parse direction. Invalid direction: {0}", directionStr));
        }
    }

    public class AxisParser
    {
        public const string AXIS_X = "x";
        public const string AXIS_Y = "y";
        public const string AXIS_Z = "z";

        public static Vector3 ParseAxis(String axis)
        {
            switch (axis.ToLower())
            {
                case AXIS_X:
                    return Vector3.right;
                case AXIS_Y:
                    return Vector3.up;
                case AXIS_Z:
                    return Vector3.forward;
            }
            throw new Exception(String.Format("Cannot parse axis. Invalid axis: {0}", axis));
        }
    }

    public class PositionParser
    {
        public static Vector3 ParsePosition(String str)
        {
            String[] strSplit = str.Split(',');
            return new Vector3(float.Parse(strSplit[0]), float.Parse(strSplit[1]), float.Parse(strSplit[2]));
        }
    }

}
