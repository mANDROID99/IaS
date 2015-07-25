using IaS.Domain;
using IaS.WorldBuilder.Tracks;

namespace IaS.GameState
{
    public class TrackContext
    {
        public readonly SplitTrack SplitTrack;
        public GroupContext Group { get; internal set; }

        internal TrackContext(SplitTrack splitTrack)
        {
            SplitTrack = splitTrack;
        }

        
    }
}
