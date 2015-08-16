using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using IaS.XML;
using UnityEngine;

namespace IaS.WorldBuilder.XML
{
    public class XmlValueResult<T>
    {
        public const string DirectionForward = "forward";
        public const string DirectionBack = "back";
        public const string DirectionRight = "right";
        public const string DirectionLeft = "left";
        public const string DirectionUp = "up";
        public const string DirectionDown = "down";

        public const string AxisX = "x";
        public const string AxisY = "y";
        public const string AxisZ = "z";

        private readonly string _valueString;
        private readonly T _value;
        private readonly bool _present;
        private readonly string _attribName;
        private readonly string _elementName;

        public XmlValueResult(string valueString, T value, string attribName, string elementName, bool present)
        {
            _valueString = valueString;
            _value = value;
            _present = present;
            _attribName = attribName;
            _elementName = elementName;
        }

        private static XmlValueResult<string> SimpleValueResult(string value, string attribName, string elementName,bool present)
        {
            return new XmlValueResult<string>(value, value, attribName, elementName, present);
        }   

        public static XmlValueResult<string> FromAttribute(XElement element, string attributeName)
        {
            string elementName = element.Name.LocalName;
            XAttribute attribute = element.Attribute(attributeName);
            return attribute != null
                ? SimpleValueResult(attribute.Value, attributeName, elementName, true)
                : SimpleValueResult("", attributeName, elementName, false);
        }

        public static XmlValueResult<string> FromElementValue(XElement parent, string elementName)
        {
            XElement element = parent.Element(elementName);
            return element != null
                ? SimpleValueResult(element.Value, "value", elementName, true)
                : SimpleValueResult("", "value", elementName, false);
        }

        public T MandatoryValue()
        {
            if (!_present)
            {
                throw new Exception(string.Format("Required attribute '{0}' from element '{1}' could not be found", _attribName, _elementName));
            }
            return _value;
        }

        public T OptionalValue(T defaultValue)
        {
            return _present ? _value : defaultValue;
        }

        public TS? OptionalStruct<TS>() where TS : struct
        {
            if (!_present)
                return default(TS);

            return _value as TS?;
        }

        public XmlValueResult<string> AsText()
        {
            return SimpleValueResult(_valueString, _attribName, _elementName, _present);
        }

        public XmlValueResult<float> AsFloat()
        {
            if (!_present)
            {
                return new XmlValueResult<float>(_valueString, 0, _attribName, _elementName, false);
            }

            float floatValue;
            if (!float.TryParse(_valueString, out floatValue))
            {
                throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}' cannot be converted to a float", _valueString, _elementName, _attribName));
            }
            return new XmlValueResult<float>(_valueString, floatValue, _attribName, _elementName, true);
        }

        public XmlValueResult<Vector3> AsVector3()
        {
            if (!_present)
            {
                return new XmlValueResult<Vector3>(_valueString, default(Vector3), _attribName, _elementName, false);
            }

            string[] splitString = _valueString.Split(',');
            try
            {
                Vector3 positionValue = new Vector3(float.Parse(splitString[0].Trim()), float.Parse(splitString[1].Trim()), float.Parse(splitString[2].Trim()));
                return new XmlValueResult<Vector3>(_valueString, positionValue, _attribName, _elementName, true);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}' could not be converted to a Position", _valueString, _elementName, _attribName));
            }
        }

        public XmlValueResult<Vector3> AsAxis()
        {
            switch (_valueString.ToLower())
            {
                case AxisX:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.right, _attribName, _elementName, true);
                case AxisY:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.up, _attribName, _elementName, true);
                case AxisZ:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.forward, _attribName, _elementName, true);
                default:
                    throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}' could not be converted to an Axis", _valueString, _elementName, _attribName));
            }
        }

        public XmlValueResult<Vector3> AsDirection()
        {
            if (!_present)
            {
                return new XmlValueResult<Vector3>(_valueString, default(Vector3), _attribName, _elementName, false);
            }
            
            switch (_valueString)
            {
                case DirectionForward:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.forward, _attribName, _elementName, true);
                case DirectionBack:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.back, _attribName, _elementName, true);
                case DirectionLeft:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.left, _attribName, _elementName, true);
                case DirectionRight:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.right, _attribName, _elementName, true);
                case DirectionUp:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.up, _attribName, _elementName, true);
                case DirectionDown:
                    return new XmlValueResult<Vector3>(_valueString, Vector3.down, _attribName, _elementName, true);
                default:
                    throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}' could not be converted to a Direction", _valueString, _elementName, _attribName));
            }
        } 

        public XmlValueResult<E> AsEnum<E>() where E : struct
        {
            if (!_present)
            {
                return new XmlValueResult<E>(_valueString, default(E), _attribName, _elementName, false);
            }

            try
            {
                E enumValue = (E)Enum.Parse(typeof(E), _valueString);
                return new XmlValueResult<E>(_valueString, enumValue, _attribName, _elementName, true);
            }
            catch (ArgumentException)
            {
                throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}' could not be converted to an Enum", _valueString, _elementName, _attribName));
            }
        }

        public XmlValueResult<R> AsReference<R>(IEnumerable<R> references) where R : IXmlReferenceable
        {
            if(!_present)
                return new XmlValueResult<R>(_valueString, default(R), _attribName, _elementName, false);

            R reference = XmlReferenceHelper.GetReference(_valueString, references);
            if (reference == null)
            {
                throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}'. Reference could not be found.", _valueString, _elementName, _attribName));
            }
            return new XmlValueResult<R>(_valueString, reference, _attribName, _elementName, true);
        }

        public XmlValueResult<string> AsMultiChoice(params string[] choices)
        {
            if(_present && !choices.Contains(_valueString))
                throw new Exception(string.Format("Value '{0}' for attribute '{2}' from element '{1}'. Option not found in multi-choice.", _valueString, _elementName, _attribName));
            return SimpleValueResult(_valueString, _attribName, _elementName, _present);
        }

        public string AsIdValue(string type, Dictionary<string, int> counts)
        {
            if (!_present)
            {
                int count;
                if (!counts.TryGetValue(type, out count))
                {
                    counts.Add(type, -1);
                }
                count ++;
                counts[type] = count;
                return string.Format("{0}_{1}", type, count);
            }
            return _valueString;
        }
    }
}
