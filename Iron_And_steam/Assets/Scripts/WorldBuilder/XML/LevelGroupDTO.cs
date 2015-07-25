using System;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class LevelGroupDTO
    {
        public MeshBlock[] Meshes { get; private set; }
		public Split[] Splits { get; private set; }
        public TrackDTO[] TracksDto { get; private set; }
        public JunctionDTO[] JunctionsDto { get; private set; }

        public string Id { get; private set; }
		public Vector3 Position { get; private set; }

        public LevelGroupDTO(string id, Vector3 position, MeshBlock[] meshes, Split[] splits, TrackDTO[] tracksDto, JunctionDTO[] junctionsDto)
        {
            Id = id;
			Position = position;
            Meshes = meshes;
            Splits = splits;
            TracksDto = tracksDto;
            JunctionsDto = junctionsDto;
        }
    }
}
