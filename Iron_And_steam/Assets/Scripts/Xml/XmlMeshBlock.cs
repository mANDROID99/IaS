using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain;
using UnityEngine;

namespace IaS.Xml
{
    public class XmlMeshBlock
    {
        public const string ElementSubMesh = "sub";
        private const string AttrMeshBlockId = "id";
        private const string AttrMeshblockPosition = "p";
        private const string AttrMeshblockSize = "s";
        private const string AttrMeshblockType = "m";

        public readonly string Id;
        public readonly Quaternion Rotation;
        public readonly MeshBlock.Type BlockType;
        public readonly Vector3 Position;
        public readonly Vector3 Size;

        public XmlMeshBlock(string id, MeshBlock.Type blockType, Quaternion rotation, Vector3 position, Vector3 size)
        {
            Id = id;
            BlockType = blockType;
            Rotation = rotation;
            Position = position;
            Size = size;
        }

        public static XmlMeshBlock FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrMeshBlockId).AsIdValue("block", counts);
            Vector3 position = XmlValueMapper.FromAttribute(element, AttrMeshblockPosition).AsVector3().MandatoryValue();
            Vector3 size = XmlValueMapper.FromAttribute(element, AttrMeshblockSize).AsVector3().MandatoryValue();
            MeshBlock.Type blockType = XmlValueMapper.FromAttribute(element, AttrMeshblockType).AsEnum<MeshBlock.Type>().MandatoryValue();
            Quaternion rotation = XmlRotation.FromElement(element.Element(XmlRotation.ElementBlockRotation)).CreateQuaternion();

            return new XmlMeshBlock(id, blockType, rotation, position, size);
        }

    }
}
