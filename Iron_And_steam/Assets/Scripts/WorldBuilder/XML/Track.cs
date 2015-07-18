using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Splines;

namespace IaS.WorldBuilder.Xml
{
    public class Track
    {
        public String id { get; private set; }
        public TrackNode[] nodes { get; private set; }
        public Vector3 down { get; private set; }

        public Track(String id, Vector3 down, TrackNode[] nodes)
        {
            this.id = id;
            this.down = down;
            this.nodes = nodes;
        }
    }

    public class TrackNode
    {
        public Vector3 position { get; private set; }
        internal TrackNode previous { get; private set; }
        internal TrackNode next { get; private set; }

        public TrackNode(Vector3 position, TrackNode previous = null)
        {
            this.position = position;
            this.previous = previous;
            if(previous != null)
            {
                previous.next = this;
            }
        }
    }
}
