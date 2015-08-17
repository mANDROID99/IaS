using System.Collections.Generic;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.WorldBuilder
{
    public class BlockRotation
    {
        public const string ElementBlockRotation = "rot";
        private const string AttrRotDirection = "dir";
        private const string AttrRotAmount = "r";
        public const string DIRECTION_DEFAULT = "u";

        private static Dictionary<string, Vector3> faceRotationMapping = new Dictionary<string, Vector3>() { 
            {"l", new Vector3(0, 0, 90)}, // left
            {"r", new Vector3(0, 0, 270)}, // right
            {"u", new Vector3(0, 0, 0)}, // up
            {"d", new Vector3(180, 0, 0)}, // down
            {"f", new Vector3(270, 0, 0)}, // front
            {"b", new Vector3(90, 0, 0)} // back
        };

        public Quaternion quaternion { get; private set; }

        public static BlockRotation FromElement(XElement element)
        {
            if(element == null) return new BlockRotation();

            string direction = XmlValueMapper.FromAttribute(element, AttrRotDirection).AsText().MandatoryValue();
            float amount = XmlValueMapper.FromAttribute(element, AttrRotAmount).AsFloat().MandatoryValue();
            return new BlockRotation(direction, amount);
        }

        public BlockRotation()
        {
            this.quaternion = CreateQuaternion(DIRECTION_DEFAULT, 0);
        }

        public BlockRotation(string direction, float value)
        {
            this.quaternion = CreateQuaternion(direction, value);
        }

        private Quaternion CreateQuaternion(string direction, float value)
        {
            Vector3 faceRot = faceRotationMapping[direction];
            return Quaternion.Euler(faceRot) * Quaternion.Euler(0, value, 0);
        }

        
    }
}
