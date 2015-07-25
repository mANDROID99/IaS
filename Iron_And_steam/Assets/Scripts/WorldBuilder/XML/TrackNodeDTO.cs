using UnityEngine;

namespace IaS.WorldBuilder.Xml
{
    public class TrackNodeDTO
    {
        public Vector3 Position { get; private set; }
        public string Id { get; private set; }
        public TrackNodeDTO Previous { get; private set; }
        public TrackNodeDTO Next { get; private set; }

        public TrackNodeDTO(string id, Vector3 position)
        {
            Id = id;
            Position = position;
        }
    }
}
