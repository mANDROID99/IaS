using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IaS.WorldBuilder.XML;
using UnityEngine;

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


        public MeshBlock[] Meshes { get; private set; }
		public Split[] Splits { get; private set; }
        public TrackXML[] Tracks { get; private set; }
        public JunctionXML[] Junctions { get; private set; }

        public string Id { get; private set; }

        public static LevelGroupXML FromElement(XElement element, Dictionary<string, int> counts)
        {
            int occludeOrderCount = -1;
            string id = XmlValueResult<string>.FromAttribute(element, AttrGroupId).AsIdValue("group", counts);

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

            return new LevelGroupXML(id, meshes, splits, tracks, junctions);
        }

        public LevelGroupXML(string id, MeshBlock[] meshes, Split[] splits, TrackXML[] tracks, JunctionXML[] junctions)
        {
            Id = id;
            Meshes = meshes;
            Splits = splits;
            Tracks = tracks;
            Junctions = junctions;
        }
    }
}
