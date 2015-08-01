using System;
using System.Linq;
using System.Runtime.Serialization;
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

        public static bool TryGetAttributeValue(XElement element, string attribName, out string value)
        {
            XAttribute attrib = element.Attribute(attribName);
            value = attrib != null ? attrib.Value : null;
            return attrib != null;
        }

        private static Exception CouldntFindRequiredAttribException(string elementName, string attribName, string attribType)
        {
            return new Exception(string.Format("Couldn't find required attribute '{0}' of type '{1}' at '{2}'", attribName, attribType, elementName));
        }

        private static Exception MalformedAttribException(string elementName, string attribName, string attribType)
        {
            return new Exception(string.Format("Attribute '{0}' of type '{1}' at '{2}' is malformed", attribName, attribType, elementName));
        }

        private static Exception UnresolvedReferenceException(string elementName, string attribName, string refId)
        {
            return new Exception(string.Format("Reference '{0}' could not be resolved for attribute '{1}' on '{2}'", refId, elementName, attribName));
        }

        private static Exception UnparseableEnumException(string elementName, string attribName, string value)
        {
            return new Exception(string.Format("Enum '{0}' at '{1}' for attribute '{2}' could not be converted to a matching enum", value, elementName, attribName));
        }

        public static XmlAttributeValue<T> ParseGenericAttrib<T>(XElement element, string attributeName, string attributeType,
            Func<string, T> evaluator, T defaultValue, bool optional)
        {
            string strValue;
            bool hasValue;
            if ((hasValue = TryGetAttributeValue(element, attributeName, out strValue)) || optional)
            {
                T value = hasValue ? evaluator.Invoke(strValue) : defaultValue;
                return new XmlAttributeValue<T>(hasValue, value);
            }

            throw CouldntFindRequiredAttribException(element.Name.LocalName, attributeName, attributeType);
        }

        public static XmlAttributeValue<Vector3> ParseAxisAttrib(XElement element, string attribName, bool optional = false)
        {
            Func<string, Vector3> func = s =>
            {
                switch (s.ToLower())
                {
                    case AxisX:
                        return Vector3.right;
                    case AxisY:
                        return Vector3.up;
                    case AxisZ:
                        return Vector3.forward;
                    default:
                        throw MalformedAttribException(element.Name.LocalName, attribName, "axis");
                }
            };
            return ParseGenericAttrib(element, attribName, "axis", func, new Vector3(), optional);
        }


        public static XmlAttributeValue<Vector3> ParsePositionAttrib(XElement element, string attribName, bool optional = false)
        {
            Func<string, Vector3> func = s =>
            {
                string[] strSplit = s.Split(',');
                return new Vector3(float.Parse(strSplit[0]), float.Parse(strSplit[1]), float.Parse(strSplit[2]));
            };
            return ParseGenericAttrib(element, attribName, "position", func, new Vector3(), optional);
        }

        public static XmlAttributeValue<Vector3> ParseDirectionAttrib(XElement element, string attribName, bool optional = false)
        {
            Func<string, Vector3> func = s =>
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
                    default:
                        throw MalformedAttribException(element.Name.LocalName, attribName, "direction");
                }
            };
            return ParseGenericAttrib(element, attribName, "direction", func, new Vector3(), optional);
        }


        public static XmlAttributeValue<float> ParseFloatAttrib(XElement element, string attribName, bool optional = false)
        {
            Func<string, float> func = s => float.Parse(s);
            return ParseGenericAttrib(element, attribName, "float", func, 0, optional);
        }

        public static XmlAttributeValue<string> ParseTextAttrib(XElement element, string attribName, bool optional = false)
        {
            Func<string, string> func = s => s;
            return ParseGenericAttrib(element, attribName, "text", func, "", optional);
        }

        public static XmlAttributeValue<T> ParseReference<T>(XElement element, string attribName, T[] references, bool optional = false) where T : IXmlReferenceable
        {
            XmlAttributeValue<string> refId = ParseTextAttrib(element, attribName, optional);

            if (!refId.HasValue) return XmlAttributeValue<T>.None();

            T refObj = references.FirstOrDefault(reference => ToRefId(reference.GetId()).Equals(refId.Value));
            
            if (refObj == null) throw UnresolvedReferenceException(element.Name.LocalName, attribName, refId.Value);
            return XmlAttributeValue<T>.Of(refObj);
        }

        public static XmlAttributeValue<T> ParseEnumAttrib<T>(XElement element, string attribName) where T : struct, IComparable
        {
            Func<string, T> func = s =>
            {
                try
                {
                    return (T) Enum.Parse(typeof (T), s);
                }
                catch (ArgumentException)
                {
                    throw UnparseableEnumException(element.Name.LocalName, attribName, s);
                }
            };
            return ParseGenericAttrib(element, attribName, "enum", func, default(T), false);
        }

        private static string ToRefId(string id)
        {
            return "@" + id;
        }
    }

    public struct XmlAttributeValue<T>
    {
        public readonly bool HasValue;
        public readonly T Value;

        public XmlAttributeValue(bool hasValue, T value)
        {
            HasValue = hasValue;
            Value = value;
        }

        public static XmlAttributeValue<T> None()
        {
            return new XmlAttributeValue<T>(false, default(T));
        }

        public static XmlAttributeValue<T> Of(T value)
        {
            return new XmlAttributeValue<T>(true, value);
        }

        public TS? ToOptional<TS>() where TS : struct
        {
            if (!HasValue)
                return default(TS);

            return Value as TS?;
        }
    }
}
