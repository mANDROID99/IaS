using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.Domain;
using IaS.WorldBuilder.XML;

namespace IaS.WorldBuilder.Xml
{
    public class LevelGroupXML
    {
        public const string ElementGroup = "group";
        private const string ElementTracks = "tracks";
        private const string ElementSplits = "splits";
        private const string ElementJunctions = "junctions";
        private const string ElementSubMeshes = "submeshes";
        private const string AttrGroupId = "id";
        private const string AttrAttach = "attach";

        public readonly SplitAttachment SplitAttachment;
        public readonly MeshBlock[] Meshes;
        public readonly Split[] Splits;
        public readonly TrackXML[] Tracks;
        public readonly JunctionXML[] Junctions;

        public string Id { get; private set; }

        public static LevelGroupXML FromElement(XElement element, Dictionary<string, int> counts)
        {
            int occludeOrderCount = -1;
            string id = XmlValueMapper.FromAttribute(element, AttrGroupId).AsIdValue("group", counts);


            TrackXML[] tracks = element.Element(ElementTracks).Elements(TrackXML.ElementTrack).Select(xTrack => TrackXML.FromElement(xTrack, counts)).ToArray();

            XElement xSplits = element.Element(ElementSplits);
            Split[] splits = xSplits != null
                ? xSplits.Elements(Split.ElementSplit).Select(xSplit => Split.FromElement(xSplit, counts)).ToArray()
                : new Split[0];

            XElement xJunctions = element.Element(ElementJunctions);
            JunctionXML[] junctions = xJunctions != null
                ? xJunctions.Elements(JunctionXML.ElementJunction).Select(xJunction => JunctionXML.FromElement(xJunction, tracks, counts)).ToArray()
                : new JunctionXML[0];

            XElement xMeshes = element.Element(ElementSubMeshes);
            MeshBlock[] meshes = xMeshes != null
                ? xMeshes.Elements(MeshBlock.ElementSubMesh).Select(xMesh => MeshBlock.FromElement(xMesh, counts, ref occludeOrderCount)).ToArray()
                : new MeshBlock[0];


            SplitAttachment attachment = XmlValueMapper.FromAttribute(element, AttrAttach).AsSplitAttachment(splits).OptionalValue(null);

            return new LevelGroupXML(id, meshes, splits, tracks, junctions, attachment);
        }

        public LevelGroupXML(string id, MeshBlock[] meshes, Split[] splits, TrackXML[] tracks, JunctionXML[] junctions, SplitAttachment splitAttachment)
        {
            Id = id;
            Meshes = meshes;
            Splits = splits;
            Tracks = tracks;
            Junctions = junctions;
            SplitAttachment = splitAttachment;
        }
    }
}
