using System;
using System.Collections.Generic;
using System.Xml.Linq;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.WorldBuilder
{

    [Serializable]
    public class MeshBlock
    {
        public const string ElementSubMesh = "sub";
        private const string AttrMeshBlockId = "id";
        private const string AttrMeshblockPosition = "p";
        private const string AttrMeshblockSize = "s";
        private const string AttrMeshblockType = "m";

        public const int TypeCuboid = 0;
        public const int TypeEdge = 1;
        public const int TypeCorner = 2;
        public const int TypeSlope = 3;
        public const int TypeSplit = 4;
        public const int TypeTrack = 5;

        public const string TypeStrCuboid = "block";
        public const string TypeStrEdge = "edge";
        public const string TypeStrCorner= "corner";
        public const string TypeStrSlope = "slope";

        public Quaternion RotationQuat { get; set; }
		public LevelGroupXML GroupXml {get; private set; }
        public string Id { get; private set; }
        public int Type { get; private set; }
		public MeshSource MeshSource { get; private set; }

        public int OccludeOrder { get; private set; }
        public BlockRotation Rotation { get; private set; }
        public BlockBounds Bounds { get; private set; }
        public BlockBounds RotatedBlockBounds { get; private set; }

        public static MeshBlock FromElement(XElement element, Dictionary<string, int> counts, ref int occludeOrderCount)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrMeshBlockId).AsIdValue("block", counts);
            Vector3 position = XmlValueMapper.FromAttribute(element, AttrMeshblockPosition).AsVector3().MandatoryValue();
            Vector3 size = XmlValueMapper.FromAttribute(element, AttrMeshblockSize).AsVector3().MandatoryValue();
            string typeStr = XmlValueMapper.FromAttribute(element, AttrMeshblockType).AsMultiChoice(TypeStrCuboid, TypeStrEdge, TypeStrCorner, TypeStrSlope).MandatoryValue();
            int type = TypeStringToType(typeStr);
            BlockRotation rotation = BlockRotation.FromElement(element.Element(BlockRotation.ElementBlockRotation));

            MeshSource goBuilder;
            if (TypeCuboid.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(TypeCuboid);
            }
            else if (TypeEdge.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(TypeEdge);
            }
            else if (TypeCorner.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(TypeCorner);
            }
            else if (TypeSlope.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(TypeSlope);
            }
            else
            {
                throw new Exception(string.Format("Invalid block type encountered: {0}", type));
            }
            BlockBounds blockBounds = new BlockBounds(position, size);
            occludeOrderCount += 1;
            return new MeshBlock(id, goBuilder, type, blockBounds, rotation, occludeOrderCount);
        }

        public MeshBlock(string id, MeshSource gameObjectBuilder, int type, BlockBounds blockBounds, BlockRotation blockRotation, int occludeOrder)
        {
            Id = id;
			GroupXml = GroupXml;
			MeshSource = gameObjectBuilder;
            Type = type;
            Rotation = blockRotation;
            OccludeOrder = occludeOrder;
            Bounds = blockBounds;
            RotatedBlockBounds = blockBounds.Copy();
            RotationQuat = Quaternion.Euler(0, 0, 0);
        }

        public bool Intersects(MeshBlock test)
        {
            return Intersects1D(Bounds.MinX, Bounds.MinX, Bounds.MaxX, Bounds.MaxX) &&
                Intersects1D(Bounds.MinY, Bounds.MinY, Bounds.MaxY, Bounds.MaxY) &&
                Intersects1D(Bounds.MinZ, Bounds.MinZ, Bounds.MaxZ, Bounds.MaxZ);
        }

        private bool Intersects1D(float min1, float min2, float max1, float max2)
        {
            float smallestMax = Math.Min(max1, max2);
            float smallestMin = Math.Min(min1, min2);
            float biggestMin = Math.Max(min1, min2);
            return ((smallestMax <= smallestMin) || (smallestMax >= biggestMin));
        }

        public MeshBlock CopyOf(BlockBounds blockBounds)
        {
            return new MeshBlock(Id, MeshSource, Type, blockBounds, Rotation, OccludeOrder);
        }

        public static int TypeStringToType(string type)
        {
            if (type == TypeStrCuboid)
            {
                return TypeCuboid;
            }else if(type == TypeStrEdge)
            {
                return TypeEdge;
            }else if(type == TypeStrCorner)
            {
                return TypeCorner;
            }else if(type == TypeStrSlope)
            {
                return TypeSlope;
            }
            return -1;
        }
    }
}
