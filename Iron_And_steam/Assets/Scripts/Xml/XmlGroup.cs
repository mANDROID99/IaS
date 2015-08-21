using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.Domain;
using IaS.Domain.XML;

namespace IaS.Xml
{
    public class XmlGroup
    {
        public const string ElementGroup = "group";
        private const string ElementTracks = "tracks";
        private const string ElementSplits = "splits";
        private const string ElementJunctions = "junctions";
        private const string ElementSubMeshes = "submeshes";
        private const string AttrGroupId = "id";
        private const string AttrAttach = "attach";

        public readonly XmlSplitSide SplitAttachment;
        public readonly XmlMeshBlock[] Blocks;
        public readonly XmlSplit[] Splits;
        public readonly Track[] Tracks;
        public readonly XmlJunction[] Junctions;

        public string Id { get; private set; }

        public static XmlGroup FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrGroupId).AsIdValue("group", counts);

            XElement xTracks = element.Element(ElementTracks);
            Track[] tracks = xTracks != null 
                ? xTracks.Elements(Track.ElementTrack).Select(xTrack => Track.FromElement(xTrack, counts)).ToArray()
                : new Track[0];

            XElement xSplits = element.Element(ElementSplits);
            XmlSplit[] splits = xSplits != null
                ? xSplits.Elements(XmlSplit.ElementSplit).Select(xSplit => XmlSplit.FromElement(xSplit, counts)).ToArray()
                : new XmlSplit[0];

            XElement xJunctions = element.Element(ElementJunctions);
            XmlJunction[] junctions = xJunctions != null
                ? xJunctions.Elements(XmlJunction.ElementJunction).Select(xJunction => XmlJunction.FromElement(xJunction, tracks, counts)).ToArray()
                : new XmlJunction[0];

            XElement xMeshes = element.Element(ElementSubMeshes);
            XmlMeshBlock[] meshes = xMeshes != null
                ? xMeshes.Elements(XmlMeshBlock.ElementSubMesh).Select(xMesh => XmlMeshBlock.FromElement(xMesh, counts)).ToArray()
                : new XmlMeshBlock[0];

            string attachment = XmlValueMapper.FromAttribute(element, AttrAttach).AsText().OptionalValue(null);
            XmlSplitSide splitSide = attachment == null ? null : XmlSplitSide.FromString(attachment);

            return new XmlGroup(id, meshes, splits, tracks, junctions, splitSide);
        }

        public XmlGroup(string id, XmlMeshBlock[] blocks, XmlSplit[] splits, Track[] tracks, XmlJunction[] junctions, XmlSplitSide splitAttachment)
        {
            Id = id;
            Blocks = blocks;
            Splits = splits;
            Tracks = tracks;
            Junctions = junctions;
            SplitAttachment = splitAttachment;
        }
    }
}
