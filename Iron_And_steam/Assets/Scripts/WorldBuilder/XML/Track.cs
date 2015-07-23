using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class Track
    {
        public string Id { get; private set; }
        public TrackNode[] Nodes { get; private set; }
        public Vector3 Down { get; private set; }

        public string StartRef { get; private set; }
        public string EndRef { get; private set; }


        public Track(string id, Vector3 down, TrackNode[] nodes, string startRef=null, string endRef=null)
        {
            Id = id;
            Down = down;
            Nodes = nodes;
            StartRef = startRef;
            EndRef = endRef;
        }
    }

    public class TrackNode
    {
        
        public Vector3 Position { get; private set; }
        public string Id { get; private set; }
        internal TrackNode Previous { get; private set; }
        internal TrackNode Next { get; private set; }

        public TrackNode(string id, Vector3 position, TrackNode previous = null)
        {
            Id = id;
            Position = position;
            Previous = previous;
           
            if(previous != null)
            {
                previous.Next = this;
            }
        }
    }
}
