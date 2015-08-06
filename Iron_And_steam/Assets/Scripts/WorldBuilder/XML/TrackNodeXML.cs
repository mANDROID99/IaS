using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class TrackNodeXML
    {
        public Vector3 Position { get; private set; }
        public string Id { get; private set; }
        public TrackNodeXML Previous { get; private set; }
        public TrackNodeXML Next { get; private set; }

        public TrackNodeXML(string id, Vector3 position)
        {
            Id = id;
            Position = position;
        }
    }
}
