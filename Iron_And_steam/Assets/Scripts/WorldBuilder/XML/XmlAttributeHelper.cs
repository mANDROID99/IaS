using System;
using System.Linq;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;
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

        public static bool TryGetAttributeValue(XAttribute attrib, out string value)
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

        private static Exception UnresolvedReferenceException(string refId)
        {
            return new Exception(string.Format("Reference {0} could not be resolved", refId));
        }

        public static T ParseGeneric<T>(XAttribute attrib, Func<string, T> evaluator, T defaultValue, string attribType, bool optional = true)
        {
            string value;
            if (!TryGetAttributeValue(attrib, out value) && !optional)
            {
                throw CouldntFindRequiredAttribException(attribType);
            }

            return value == null ? defaultValue : evaluator.Invoke(value);
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
            }, defaultVal, "axis", optional);
        }


        public static Vector3 ParsePosition(XAttribute attrib, bool optional = true, Vector3 defaultVal = new Vector3())
        {
            return ParseGeneric(attrib, s =>
            {
                string[] strSplit = s.Split(',');
                return new Vector3(float.Parse(strSplit[0]), float.Parse(strSplit[1]), float.Parse(strSplit[2]));
            }, defaultVal, "position", optional);
        }

        public static Vector3 ParseDirectionMandatory(XAttribute attrib)
        {
            Vector3? direction = ParseDirectionOptional(attrib);
            if (!direction.HasValue)
                throw CouldntFindRequiredAttribException("direction");
            return direction.Value;
        }

        public static Vector3? ParseDirectionOptional(XAttribute attr)
        {
            string value;
            if (!TryGetAttributeValue(attr, out value))
            {
                return null;
            }
            switch (value)
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
        }

        public static float ParseFloat(XAttribute attrib, bool optional = true, float defaultVal = 0)
        {
            return ParseGeneric(attrib, s => float.Parse(s), defaultVal, "float", optional);
        }

        public static string Parse(XAttribute attrib, bool optional = true, string defaultVal = "")
        {
            return ParseGeneric(attrib, s => s, defaultVal, "string", optional);
        }

        public static T ParseReference<T>(XAttribute attrib, T[] references, bool optional = true) where T : IXmlReferenceable
        {
            string refId = Parse(attrib, optional, null);
            if (refId == null) return default(T);

            T refObj = references.FirstOrDefault(reference => ToRefId(reference.GetId()).Equals(refId));
            
            if (refObj == null) throw UnresolvedReferenceException(refId);
            return refObj;
        }

        private static string ToRefId(string id)
        {
            return "@" + id;
        }
    }
}
