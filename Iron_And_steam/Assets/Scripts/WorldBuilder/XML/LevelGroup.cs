using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class LevelGroup
    {
        public MeshBlock[] meshes { get; private set; }
		public Split[] splits { get; private set; }
        public Track[] tracks { get; private set; }

        public String id { get; private set; }
		public Vector3 position { get; private set; }

        public LevelGroup(String id, Vector3 position, MeshBlock[] meshes, Split[] splits, Track[] tracks)
        {
            this.id = id;
			this.position = position;
            this.meshes = meshes;
            this.splits = splits;
            this.tracks = tracks;
        }
    }
}
