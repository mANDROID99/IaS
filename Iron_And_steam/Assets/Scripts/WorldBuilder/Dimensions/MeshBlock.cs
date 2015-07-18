using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Xml;

namespace IaS.WorldBuilder
{

    [Serializable]
    public class MeshBlock
    {

        public const int TYPE_CUBOID = 0;
        public const int TYPE_EDGE = 1;
        public const int TYPE_CORNER = 2;
        public const int TYPE_SLOPE = 3;
        public const int TYPE_SPLIT = 4;
        public const int TYPE_TRACK = 5;

        public const string TYPE_STR_CUBOID = "block";
        public const string TYPE_STR_EDGE = "edge";
        public const string TYPE_STR_CORNER= "corner";
        public const string TYPE_STR_SLOPE = "slope";

        public Quaternion rotationQuat { get; set; }
		public LevelGroup group {get; private set; }
        public String id { get; private set; }
        public int type { get; private set; }
		public MeshSource meshSource { get; private set; }

        public int occludeOrder { get; private set; }
        public BlockRotation rotation { get; private set; }
        public BlockBounds bounds { get; private set; }
        public BlockBounds rotatedBlockBounds { get; private set; }

        public MeshBlock(String id, MeshSource gameObjectBuilder, int type, BlockBounds blockBounds, BlockRotation blockRotation, int occludeOrder)
        {
            this.id = id;
			this.group = group;
			this.meshSource = gameObjectBuilder;
            this.type = type;
            this.rotation = blockRotation;
            this.occludeOrder = occludeOrder;
            this.bounds = blockBounds;
            this.rotatedBlockBounds = blockBounds.Copy();
            this.rotationQuat = Quaternion.Euler(0, 0, 0);
        }

        public bool Intersects(MeshBlock test)
        {
            return Intersects1D(bounds.minX, bounds.minX, bounds.maxX, bounds.maxX) &&
                Intersects1D(bounds.minY, bounds.minY, bounds.maxY, bounds.maxY) &&
                Intersects1D(bounds.minZ, bounds.minZ, bounds.maxZ, bounds.maxZ);
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
            return new MeshBlock(this.id, this.meshSource, this.type, blockBounds, this.rotation, this.occludeOrder);
        }

        public static int TypeStringToType(String type)
        {
            if (type == TYPE_STR_CUBOID)
            {
                return TYPE_CUBOID;
            }else if(type == TYPE_STR_EDGE)
            {
                return TYPE_EDGE;
            }else if(type == TYPE_STR_CORNER)
            {
                return TYPE_CORNER;
            }else if(type == TYPE_STR_SLOPE)
            {
                return TYPE_SLOPE;
            }
            return -1;
        }
    }
}
