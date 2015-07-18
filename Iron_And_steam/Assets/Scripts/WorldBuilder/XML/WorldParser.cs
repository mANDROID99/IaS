using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;

namespace IaS.WorldBuilder.Xml
{
    public class WorldParser
    {

		private const string ELEMENT_LEVEL = "level";
		private const string ELEMENT_GROUPS = "groups";
		private const string ELEMENT_GROUP = "group";
		private const string ELEMENT_MESH = "mesh";
		private const string ELEMENT_MESHBLOCK = "sub";
        private const string ELEMENT_ROTATION = "rot";
		private const string ELEMENT_SPLIT = "split";
        private const string ELEMENT_SUBSPLITS_L = "subsplits_left";
        private const string ELEMENT_SUBSPLITS_R = "subsplits_right";
        private const string ELEMENT_TRACK = "track";
        private const string ELEMENT_TRACK_NODE = "node";

        private const string ATTR_ID = "id";
		private const string ATTR_MESHBLOCK_POSITION = "p";
		private const string ATTR_MESHBLOCK_SIZE = "s";
        private const string ATTR_MESHBLOCK_TYPE = "m";
		private const string ATTR_WEDGE_ROTATION = "r";
		private const string ATTR_GROUP_POSITION = "p";
        private const string ATTR_ROT_DIRECTION = "dir";
        private const string ATTR_ROT_AMOUNT = "r";
		private const string ATTR_SPLIT_AXIS = "axis";
		private const string ATTR_SPLIT_VALUE = "val";
        private const string ATTR_SPLIT_PIVOT = "pivot";
        private const string ATTR_TRACK_START_DIRECTION = "startdir";
        private const string ATTR_TRACK_NODE_POSITION = "p";

        private int idCount_split = 0;
        private int idCount_block = 0;
        private int idCount_group = 0;
        private int idCount_track = 0;

        private int occludeOrderCount = 0;

        public Level Parse(TextAsset source, TextAsset schema)
        {

            XmlReader sourceReader = XmlReader.Create(new StringReader(source.text));
            XDocument xDoc = new XDocument(XDocument.Load(sourceReader));
            XElement xLevel = xDoc.Element(ELEMENT_LEVEL);
            return ParseElementLevel(xLevel);
        }

        private Level ParseElementLevel(XElement xLevel)
        {
            Level level = new Level();
            foreach (XElement xGroup in xLevel.Element(ELEMENT_GROUPS).Elements(ELEMENT_GROUP))
            {
                level.groups.Add(ParseBlockGroup(xGroup));
            }
            return level;
        }

        private LevelGroup ParseBlockGroup(XElement xGroup)
        {
            String id = GetId(xGroup, "group", ref idCount_group);
            Vector3 groupPos = PositionParser.ParsePosition(xGroup.Attribute(ATTR_GROUP_POSITION).Value);

            MeshBlock[] meshBlocks = xGroup.Element(ELEMENT_MESH).Elements(ELEMENT_MESHBLOCK).Select(xBlock => ParseMeshBlock(xBlock)).ToArray();
            Split[] splits = xGroup.Elements(ELEMENT_SPLIT).Select(xSplit => ParseSplit(xSplit)).ToArray();
            Track[] tracks = xGroup.Elements(ELEMENT_TRACK).Select(xTrack => ParseTrack(xTrack)).ToArray();

            LevelGroup blockGroup = new LevelGroup(id, groupPos, meshBlocks, splits, tracks);
            return blockGroup;
        }



        private MeshBlock ParseMeshBlock(XElement xBlock)
        {
            Vector3 position = PositionParser.ParsePosition(xBlock.Attribute(ATTR_MESHBLOCK_POSITION).Value);
            Vector3 size = PositionParser.ParsePosition(xBlock.Attribute(ATTR_MESHBLOCK_SIZE).Value);
     
            String typeStr = xBlock.Attribute(ATTR_MESHBLOCK_TYPE).Value;
            int type = MeshBlock.TypeStringToType(typeStr);
            String id = GetId(xBlock, String.Format("sub_{0}", typeStr), ref idCount_block);

            BlockRotation rotation = ParseRotation(xBlock.Element(ELEMENT_ROTATION));

            MeshSource goBuilder;
            if (MeshBlock.TYPE_CUBOID.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(MeshBlock.TYPE_CUBOID);
            }else if (MeshBlock.TYPE_EDGE.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(MeshBlock.TYPE_EDGE);
            }else if (MeshBlock.TYPE_CORNER.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(MeshBlock.TYPE_CORNER);
            }else if (MeshBlock.TYPE_SLOPE.Equals(type))
            {
                goBuilder = ProceduralMeshSource.GetInstance(MeshBlock.TYPE_SLOPE);
            }
            else
            {
                throw new Exception(String.Format("Invalid block type encountered: {0}", type));
            }
            BlockBounds blockBounds = new BlockBounds(position, size);
            return new MeshBlock(id, goBuilder, type, blockBounds, rotation, occludeOrderCount++);
        }

        private BlockRotation ParseRotation(XElement xRot)
        {
            if(xRot == null){
                return new BlockRotation();
            }

            String direction = xRot.Attribute(ATTR_ROT_DIRECTION).Value;
            float amount = Single.Parse(xRot.Attribute(ATTR_ROT_AMOUNT).Value);
            return new BlockRotation(direction, amount);
        }

		private Split ParseSplit(XElement xSplit)
		{
			float value = Int32.Parse(xSplit.Attribute (ATTR_SPLIT_VALUE).Value);
            Vector3 pivot = PositionParser.ParsePosition(xSplit.Attribute(ATTR_SPLIT_PIVOT).Value);
            Vector3 axis = AxisParser.ParseAxis(xSplit.Attribute(ATTR_SPLIT_AXIS).Value);
            String id = GetId(xSplit, "split", ref idCount_split);

            XElement xSubSplits_L = xSplit.Element(ELEMENT_SUBSPLITS_L);
            XElement xSubSplits_R = xSplit.Element(ELEMENT_SUBSPLITS_R);

            SubSplit[] subSplits = (ParseSubSplit(xSubSplits_L, true).Concat(ParseSubSplit(xSubSplits_R, false))).ToArray();
            Split split = new Split(id, axis, pivot, value, subSplits);
            return split;
		}

        private IEnumerable<SubSplit> ParseSubSplit(XElement xSubSplit, bool clipParentLeft)
        {
            if (xSubSplit == null)
            {
                return new SubSplit[0];
            }

            return xSubSplit.Elements(ELEMENT_SPLIT).Select(xSplit => new SubSplit(ParseSplit(xSplit), clipParentLeft));
        }

        private Track ParseTrack(XElement xTrack)
        {
            String id = GetId(xTrack, "track", ref idCount_track);
            Vector3 initialDirection = DirectionParser.ParseDirection(xTrack.Attribute(ATTR_TRACK_START_DIRECTION).Value);
            TrackNode[] nodes = xTrack.Elements(ELEMENT_TRACK_NODE).Select(xNode => ParseTrackNode(xNode)).ToArray();
            return new Track(id, initialDirection, nodes);
        }

        private TrackNode ParseTrackNode(XElement xNode)
        {
            Vector3 position = PositionParser.ParsePosition(xNode.Attribute(ATTR_TRACK_NODE_POSITION).Value);
            return new TrackNode(position);
        }


        private String GetId(XElement element, String elementType, ref int count)
        {
            XAttribute attr = element.Attribute(ATTR_ID);
            if(attr == null)
            {
                count += 1;
                return String.Format("{0}_{1}", elementType, count);
            }
            else
            {
                return attr.Value;
            }
        }
    }
}
