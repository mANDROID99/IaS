using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class XmlAttributeHelper
    {
        public const string DirStringForward = "forward";
        public const string DirStringBack = "back";
        public const string DirStringRight = "right";
        public const string DirStringLeft = "left";
        public const string DirStringUp = "up";
        public const string DirStringDown = "down";

        public const string AxisX = "x";
        public const string AxisY = "y";
        public const string AxisZ = "z";

        public static bool TryFindAttribute(XAttribute attrib, out string value)
        {
            value = attrib == null ? null : attrib.Value;
            return value != null;
        }

        private static Exception CouldntFindRequiredAttribException(string attribName)
        {
            return new Exception(string.Format("Couln't find required attribute of attribType '{0}'", attribName));
        }

        private static Exception MalformedAttribException(string attribName)
        {
            return new Exception(string.Format("Attribute of attribType '{0}' is malformed", attribName));
        }

        public static T ParseGeneric<T>(XAttribute attrib, Func<string, T> evaluator, Func<T> defaultEvaluator, String attribType, bool optional = true)
        {
            string value;
            if (!TryFindAttribute(attrib, out value) && !optional)
            {
                throw CouldntFindRequiredAttribException(attribType);
            }

            return value == null ? defaultEvaluator.Invoke() : evaluator.Invoke(value);
        }

        public static Vector3 ParseAxis(XAttribute attrib, bool optional = true, Vector3 defaultVal = new Vector3())
        {
            return ParseGeneric(attrib, s =>
            {
                switch (s.ToLower())
                {
                    case AxisX:
                        return Vector3.right;
                    case AxisY:
                        return Vector3.up;
                    case AxisZ:
                        return Vector3.forward;
                }
                throw MalformedAttribException("axis");
            }, () => defaultVal, "axis", optional);
        }


        public static Vector3 ParsePosition(XAttribute attrib, bool optional = true, Vector3 defaultVal = new Vector3())
        {
            return ParseGeneric(attrib, s =>
            {
                string[] strSplit = s.Split(',');
                return new Vector3(float.Parse(strSplit[0]), float.Parse(strSplit[1]), float.Parse(strSplit[2]));
            }, () => defaultVal, "position", optional);
        }

        public static Vector3 ParseDirection(XAttribute attrib, bool optional = true, Vector3 defaultVal = new Vector3())
        {
            return ParseGeneric(attrib, s =>
            {
                switch (s)
                {
                    case DirStringForward:
                        return Vector3.forward;
                    case DirStringBack:
                        return Vector3.back;
                    case DirStringLeft:
                        return Vector3.left;
                    case DirStringRight:
                        return Vector3.right;
                    case DirStringUp:
                        return Vector3.up;
                    case DirStringDown:
                        return Vector3.down;
                }
                throw MalformedAttribException("direction");
            }, () => defaultVal, "direction", optional);
        }

        public static float ParseFloat(XAttribute attrib, bool optional = true, float defaultVal = 0)
        {
            return ParseGeneric(attrib, s => float.Parse(s), () => defaultVal, "float", optional);
        }

        public static string Parse(XAttribute attrib, bool optional = true, string defaultVal = "")
        {
            return ParseGeneric(attrib, s => s, () => defaultVal, "string", optional);
        }
    }
}
