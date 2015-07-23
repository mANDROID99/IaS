using System;
using IaS.WorldBuilder;

namespace IaS.GameState
{
    public class GroupContext
    {
        public readonly string GroupId;
        public TrackContext[] Tracks { get; private set; }
        public EventRegistry EventRegistry { get; private set; }
        public MeshBlock[] MeshBlocks { get; private set; }
        public Split[] Splits { get; private set; }

        public GroupContext(String groupId, TrackContext[] tracks, EventRegistry eventRegistry, MeshBlock[] meshBlocks, Split[] splits)
        {
            GroupId = groupId;
            Tracks = tracks;
            EventRegistry = eventRegistry;
            MeshBlocks = meshBlocks;
            Splits = splits;
        }
    }
}
