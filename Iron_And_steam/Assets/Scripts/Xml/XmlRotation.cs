using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain.XML;
using IaS.Xml;
using UnityEngine;

namespace IaS.Xml
{
    public class XmlRotation
    {
        public const string ElementBlockRotation = "rot";
        private const string AttrRotDirection = "dir";
        private const string AttrRotAmount = "r";
        public const string DirectionDefault = "u";

        private static Dictionary<string, Vector3> faceRotationMapping = new Dictionary<string, Vector3>() { 
            {"l", new Vector3(0, 0, 90)}, // left
            {"r", new Vector3(0, 0, 270)}, // right
            {"u", new Vector3(0, 0, 0)}, // up
            {"d", new Vector3(180, 0, 0)}, // down
            {"f", new Vector3(270, 0, 0)}, // front
            {"b", new Vector3(90, 0, 0)} // back
        };

        private readonly string _direction;
        private readonly float _value;

        public static XmlRotation FromElement(XElement element)
        {
            if(element == null) return new XmlRotation(DirectionDefault, 0);

            string direction = XmlValueMapper.FromAttribute(element, AttrRotDirection).AsText().MandatoryValue();
            float amount = XmlValueMapper.FromAttribute(element, AttrRotAmount).AsFloat().MandatoryValue();
            return new XmlRotation(direction, amount);
        }

        public XmlRotation(string direction, float value)
        {
            _direction = direction;
            _value = value;
        }

        public Quaternion CreateQuaternion()
        {
            Vector3 faceRot = faceRotationMapping[_direction];
            return Quaternion.Euler(faceRot) * Quaternion.Euler(0, _value, 0);
        }

        
    }
}
