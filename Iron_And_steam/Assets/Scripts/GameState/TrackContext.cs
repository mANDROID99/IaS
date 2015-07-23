using IaS.WorldBuilder.Tracks;

namespace IaS.GameState
{
    public class TrackContext
    {
        public SplitTrack splitTrack { get; private set; }
        public TrackConnections connections { get; private set; }

        internal TrackContext(TrackConnections trackConnections, SplitTrack splitTrack)
        {
            this.splitTrack = splitTrack;
            this.connections = trackConnections;
        }
    }
}
