using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class LevelGroup
    {
        public MeshBlock[] Meshes { get; private set; }
		public Split[] Splits { get; private set; }
        public Track[] Tracks { get; private set; }

        public String Id { get; private set; }
		public Vector3 Position { get; private set; }

        public LevelGroup(String id, Vector3 position, MeshBlock[] meshes, Split[] splits, Track[] tracks)
        {
            this.Id = id;
			this.Position = position;
            this.Meshes = meshes;
            this.Splits = splits;
            this.Tracks = tracks;
        }
    }
}
