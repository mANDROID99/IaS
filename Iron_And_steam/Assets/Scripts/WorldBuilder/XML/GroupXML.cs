using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.Domain;
using IaS.WorldBuilder.XML;

namespace IaS.WorldBuilder.Xml
{
    public class GroupXML
    {
        public const string ElementGroup = "group";
        private const string ElementTracks = "tracks";
        private const string ElementSplits = "splits";
        private const string ElementJunctions = "junctions";
        private const string ElementSubMeshes = "submeshes";
        private const string AttrGroupId = "id";
        private const string AttrAttach = "attach";

        public readonly string SplitAttachmentStr;
        public readonly MeshBlock[] Meshes;
        public readonly SplitXML[] Splits;
        public readonly TrackXML[] Tracks;
        public readonly JunctionXML[] Junctions;

        public string Id { get; private set; }

        public static GroupXML FromElement(XElement element, Dictionary<string, int> counts)
        {
            int occludeOrderCount = -1;
            string id = XmlValueMapper.FromAttribute(element, AttrGroupId).AsIdValue("group", counts);


            TrackXML[] tracks = element.Element(ElementTracks).Elements(TrackXML.ElementTrack).Select(xTrack => TrackXML.FromElement(xTrack, counts)).ToArray();

            XElement xSplits = element.Element(ElementSplits);
            SplitXML[] splits = xSplits != null
                ? xSplits.Elements(SplitXML.ElementSplit).Select(xSplit => SplitXML.FromElement(xSplit, counts)).ToArray()
                : new SplitXML[0];

            XElement xJunctions = element.Element(ElementJunctions);
            JunctionXML[] junctions = xJunctions != null
                ? xJunctions.Elements(JunctionXML.ElementJunction).Select(xJunction => JunctionXML.FromElement(xJunction, tracks, counts)).ToArray()
                : new JunctionXML[0];

            XElement xMeshes = element.Element(ElementSubMeshes);
            MeshBlock[] meshes = xMeshes != null
                ? xMeshes.Elements(MeshBlock.ElementSubMesh).Select(xMesh => MeshBlock.FromElement(xMesh, counts, ref occludeOrderCount)).ToArray()
                : new MeshBlock[0];


            string attachment = XmlValueMapper.FromAttribute(element, AttrAttach).AsText().OptionalValue(null);

            return new GroupXML(id, meshes, splits, tracks, junctions, attachment);
        }

        public GroupXML(string id, MeshBlock[] meshes, SplitXML[] splits, TrackXML[] tracks, JunctionXML[] junctions, string splitAttachmentStr)
        {
            Id = id;
            Meshes = meshes;
            Splits = splits;
            Tracks = tracks;
            Junctions = junctions;
            SplitAttachmentStr = splitAttachmentStr;
        }
    }
}
