using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class WorldParser
    {

		private const string ElementLevel = "level";
		private const string ElementGroups = "groups";
		private const string ElementGroup = "group";
		private const string ElementMesh = "mesh";
		private const string ElementMeshblock = "sub";
        private const string ElementRotation = "rot";
		private const string ElementSplit = "split";
        private const string ElementSubsplitsL = "subsplits_left";
        private const string ElementSubsplitsR = "subsplits_right";
        private const string ElementTrack = "track";
        private const string ElementTrackNode = "node";

        private const string AttrId = "id";
		private const string AttrMeshblockPosition = "p";
		private const string AttrMeshblockSize = "s";
        private const string AttrMeshblockType = "m";
		private const string AttrGroupPosition = "p";
        private const string AttrRotDirection = "dir";
        private const string AttrRotAmount = "r";
		private const string AttrSplitAxis = "axis";
		private const string AttrSplitValue = "val";
        private const string AttrSplitPivot = "pivot";
        private const string AttrTrackStartDirection = "startdir";
        private const string AttrTrackNodePosition = "p";
        private const string AttrTrackStartRef = "startref";
        private const string AttrTrackEndRef = "endref";

        private int _idCountSplit = 0;
        private int _idCountBlock = 0;
        private int _idCountGroup = 0;
        private int _idCountTrack = 0;

        private int _occludeOrderCount = 0;

        public Level Parse(TextAsset source, TextAsset schema)
        {

            XmlReader sourceReader = XmlReader.Create(new StringReader(source.text));
            XDocument xDoc = new XDocument(XDocument.Load(sourceReader));
            XElement xLevel = xDoc.Element(ElementLevel);
            return ParseElementLevel(xLevel);
        }

        private Level ParseElementLevel(XElement xLevel)
        {
            Level level = new Level();
            foreach (XElement xGroup in xLevel.Element(ElementGroups).Elements(ElementGroup))
            {
                level.Groups.Add(ParseBlockGroup(xGroup));
            }
            return level;
        }

        private LevelGroup ParseBlockGroup(XElement xGroup)
        {
            string id = GetId(xGroup, "group", ref _idCountGroup);
            Vector3 groupPos = XmlAttributeHelper.ParsePosition(xGroup.Attribute(AttrGroupPosition), false);

            MeshBlock[] meshBlocks = xGroup.Element(ElementMesh).Elements(ElementMeshblock).Select(element => ParseMeshBlock(element)).ToArray();
            Split[] splits = xGroup.Elements(ElementSplit).Select(element => ParseSplit(element)).ToArray();
            Track[] tracks = xGroup.Elements(ElementTrack).Select(element => ParseTrack(element)).ToArray();

            LevelGroup blockGroup = new LevelGroup(id, groupPos, meshBlocks, splits, tracks);
            return blockGroup;
        }



        private MeshBlock ParseMeshBlock(XElement xBlock)
        {
            Vector3 position = XmlAttributeHelper.ParsePosition(xBlock.Attribute(AttrMeshblockPosition), false);
            Vector3 size = XmlAttributeHelper.ParsePosition(xBlock.Attribute(AttrMeshblockSize), false);

            string typeStr = XmlAttributeHelper.Parse(xBlock.Attribute(AttrMeshblockType), false);
            int type = MeshBlock.TypeStringToType(typeStr);
            string id = GetId(xBlock, string.Format("sub_{0}", typeStr), ref _idCountBlock);

            BlockRotation rotation = ParseRotation(xBlock.Element(ElementRotation));

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
                throw new Exception(string.Format("Invalid block type encountered: {0}", type));
            }
            BlockBounds blockBounds = new BlockBounds(position, size);
            return new MeshBlock(id, goBuilder, type, blockBounds, rotation, _occludeOrderCount++);
        }

        private BlockRotation ParseRotation(XElement xRot)
        {
            if(xRot == null){
                return new BlockRotation();
            }

            string direction = xRot.Attribute(AttrRotDirection).Value;
            float amount = float.Parse(xRot.Attribute(AttrRotAmount).Value);
            return new BlockRotation(direction, amount);
        }

		private Split ParseSplit(XElement xSplit)
		{
		    float value = XmlAttributeHelper.ParseFloat(xSplit.Attribute(AttrSplitValue), false);
            Vector3 pivot = XmlAttributeHelper.ParsePosition(xSplit.Attribute(AttrSplitPivot), false);
            Vector3 axis = XmlAttributeHelper.ParseAxis(xSplit.Attribute(AttrSplitAxis), false);
            string id = GetId(xSplit, "split", ref _idCountSplit);

            XElement xSubSplitsL = xSplit.Element(ElementSubsplitsL);
            XElement xSubSplitsR = xSplit.Element(ElementSubsplitsR);

            SubSplit[] subSplits = (ParseSubSplit(xSubSplitsL, true).Concat(ParseSubSplit(xSubSplitsR, false))).ToArray();
            Split split = new Split(id, axis, pivot, value, subSplits);
            return split;
		}

        private IEnumerable<SubSplit> ParseSubSplit(XElement xSubSplit, bool clipParentLeft)
        {
            if (xSubSplit == null)
            {
                return new SubSplit[0];
            }

            return xSubSplit.Elements(ElementSplit).Select(xSplit => new SubSplit(ParseSplit(xSplit), clipParentLeft));
        }

        private Track ParseTrack(XElement xTrack)
        {
            string id = GetId(xTrack, "track", ref _idCountTrack);
            Vector3 initialDirection = XmlAttributeHelper.ParseDirection(xTrack.Attribute(AttrTrackStartDirection), true, Vector3.forward);
            string startRef = XmlAttributeHelper.Parse(xTrack.Attribute(AttrTrackStartRef), true, null);
            string endRef = XmlAttributeHelper.Parse(xTrack.Attribute(AttrTrackEndRef), true, null);

            TrackNode[] nodes = xTrack.Elements(ElementTrackNode).Select(xNode => ParseTrackNode(xNode)).ToArray();
            return new Track(id, initialDirection, nodes, startRef, endRef);
        }

        private TrackNode ParseTrackNode(XElement xNode)
        {
            Vector3 position = XmlAttributeHelper.ParsePosition(xNode.Attribute(AttrTrackNodePosition), false);
            string id = XmlAttributeHelper.Parse(xNode.Attribute(AttrId), true, null);
            return new TrackNode(id, position);
        }


        private string GetId(XElement element, string elementType, ref int count)
        {
            string value;
            if (XmlAttributeHelper.TryFindAttribute(element.Attribute(AttrId), out value))
            {
                return value;
            }
           
            count += 1;
            return string.Format("{0}_{1}", elementType, count);
        }
    }
}
