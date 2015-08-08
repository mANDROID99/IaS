﻿using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class LevelGroupXML
    {
        public MeshBlock[] Meshes { get; private set; }
		public Split[] Splits { get; private set; }
        public TrackXML[] TracksXml { get; private set; }
        public JunctionXML[] JunctionsXml { get; private set; }

        public string Id { get; private set; }
		public Vector3 Position { get; private set; }

        public LevelGroupXML(string id, Vector3 position, MeshBlock[] meshes, Split[] splits, TrackXML[] tracksXml, JunctionXML[] junctionsXml)
        {
            Id = id;
			Position = position;
            Meshes = meshes;
            Splits = splits;
            TracksXml = tracksXml;
            JunctionsXml = junctionsXml;
        }
    }
}
