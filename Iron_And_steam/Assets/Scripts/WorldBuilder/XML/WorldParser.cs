using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using IaS.Domain;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class WorldParser
    {

		private const string ElementLevel = "level";
		private const string ElementGroups = "groups";
		private const string ElementGroup = "group";
        private const string ElementSubMeshes = "submeshes";
		private const string ElementMeshblock = "sub";
        private const string ElementRotation = "rot";
        private const string ElementSplits = "splits";
		private const string ElementSplit = "split";
        private const string ElementSubsplitsL = "subsplits_left";
        private const string ElementSubsplitsR = "subsplits_right";
        private const string ElementTracks = "tracks";
        private const string ElementTrack = "track";
        private const string ElementTrackNode = "node";
        private const string ElementJunctions = "junctions";
        private const string ElementJunction = "junction";

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
        private const string AttrTrackDown = "down";
        private const string AttrTrackStartDirection = "startdir";
        private const string AttrTrackNodePosition = "p";
        private const string AttrJunctionBranchDefault = "branch_default";
        private const string AttrJunctionBranchAlternate = "branch_alternate";
        private const string AttrJunctionDirection = "type";

        private int _idCountSplit = 0;
        private int _idCountBlock = 0;
        private int _idCountGroup = 0;
        private int _idCountTrack = 0;

        private int _occludeOrderCount = 0;

        public LevelXML Parse(TextAsset source, TextAsset schema)
        {
            XmlReader sourceReader = XmlReader.Create(new StringReader(source.text));
            XDocument xDoc = new XDocument(XDocument.Load(sourceReader));
            XElement xLevel = xDoc.Element(ElementLevel);
            return ParseElementLevel(xLevel);
        }

        private LevelXML ParseElementLevel(XElement xLevel)
        {
            LevelXML levelXml = new LevelXML("Level");
            foreach (XElement xGroup in xLevel.Element(ElementGroups).Elements(ElementGroup))
            {
                levelXml.Groups.Add(ParseBlockGroup(xGroup));
            }
            return levelXml;
        }

        private LevelGroupXML ParseBlockGroup(XElement xGroup)
        {
            string id = GetId(xGroup, "GroupXml", ref _idCountGroup);
            Vector3 groupPos = XmlAttributeHelper.ParsePositionAttrib(xGroup, AttrGroupPosition).Value;

            MeshBlock[] meshBlocks = ParseSubMeshes(xGroup.Element(ElementSubMeshes));
            Split[] splits = ParseSplits(xGroup.Element(ElementSplits));
            TrackXML[] tracksXml = ParseTracks(xGroup.Element(ElementTracks));
            JunctionXML[] junctionsXml = ParseJunctions(xGroup.Element(ElementJunctions), tracksXml);

            var blockGroup = new LevelGroupXML(id, groupPos, meshBlocks, splits, tracksXml, junctionsXml);
            return blockGroup;
        }

        private MeshBlock[] ParseSubMeshes(XElement xMeshBlocks)
        {
            return xMeshBlocks.Elements(ElementMeshblock).Select(xBlock => ParseMeshBlock(xBlock)).ToArray();
        }

        private Split[] ParseSplits(XElement xSplits)
        {
            return xSplits.Elements(ElementSplit).Select(xSplit => ParseSplit(xSplit)).ToArray();
        }

        private TrackXML[] ParseTracks(XElement xTracks)
        {
            return xTracks.Elements(ElementTrack).Select(xTrack => ParseTrack(xTrack)).ToArray();
        }

        private JunctionXML[] ParseJunctions(XElement xJunctions, TrackXML[] trackXmlRefs)
        {
            return xJunctions.Elements(ElementJunction).Select(xJunction => ParseJunction(xJunction, trackXmlRefs)).ToArray();
        }



        private MeshBlock ParseMeshBlock(XElement xBlock)
        {
            Vector3 position = XmlAttributeHelper.ParsePositionAttrib(xBlock, AttrMeshblockPosition).Value;
            Vector3 size = XmlAttributeHelper.ParsePositionAttrib(xBlock, AttrMeshblockSize).Value;

            string typeStr = XmlAttributeHelper.ParseTextAttrib(xBlock, AttrMeshblockType).Value;
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

            string direction = XmlAttributeHelper.ParseTextAttrib(xRot, AttrRotDirection).Value;
            float amount = XmlAttributeHelper.ParseFloatAttrib(xRot, AttrRotAmount).Value;

            return new BlockRotation(direction, amount);
        }

		private Split ParseSplit(XElement xSplit)
		{
		    float value = XmlAttributeHelper.ParseFloatAttrib(xSplit, AttrSplitValue).Value;
		    Vector3 pivot = XmlAttributeHelper.ParsePositionAttrib(xSplit, AttrSplitPivot).Value;
            Vector3 axis = XmlAttributeHelper.ParseAxisAttrib(xSplit, AttrSplitAxis).Value;
            string id = GetId(xSplit, "split", ref _idCountSplit);

            XElement xSubSplitsL = xSplit.Element(ElementSubsplitsL);
            XElement xSubSplitsR = xSplit.Element(ElementSubsplitsR);

            Split split = new Split(id, axis, pivot, value);
            return split;
		}

        private TrackXML ParseTrack(XElement xTrack)
        {
            string id = GetId(xTrack, "TrackXML", ref _idCountTrack);

            Vector3? startDir = XmlAttributeHelper.ParseDirectionAttrib(xTrack, AttrTrackStartDirection, true).ToOptional<Vector3>();
            Vector3 downDir = XmlAttributeHelper.ParseDirectionAttrib(xTrack, AttrTrackDown).Value;

            TrackNodeXML[] nodesXml = xTrack.Elements(ElementTrackNode).Select(xNode => ParseTrackNode(xNode)).ToArray();
            return new TrackXML(id, downDir, startDir, nodesXml);
        }

        private TrackNodeXML ParseTrackNode(XElement xNode)
        {
            Vector3 position = XmlAttributeHelper.ParsePositionAttrib(xNode, AttrTrackNodePosition).Value;
            string id = XmlAttributeHelper.ParseTextAttrib(xNode, AttrId, true).Value;
            return new TrackNodeXML(id, position);
        }

        private JunctionXML ParseJunction(XElement xJunction, TrackXML[] tracksXml)
        {
            TrackXML branchLeft = XmlAttributeHelper.ParseReference(xJunction, AttrJunctionBranchAlternate, tracksXml).Value;
            TrackXML branchRight = XmlAttributeHelper.ParseReference(xJunction, AttrJunctionBranchDefault, tracksXml).Value;
            Junction.JunctionDirection junctionDirection = XmlAttributeHelper.ParseEnumAttrib<Junction.JunctionDirection>(xJunction, AttrJunctionDirection).Value;
            return new JunctionXML(branchLeft, branchRight, junctionDirection);
        }

        private string GetId(XElement element, string elementType, ref int count)
        {
            string value;
            if (XmlAttributeHelper.TryGetAttributeValue(element, AttrId, out value))
            {
                return value;
            }
           
            count += 1;
            return string.Format("{0}_{1}", elementType, count);
        }
    }
}
