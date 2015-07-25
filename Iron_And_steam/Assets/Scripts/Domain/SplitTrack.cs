using System.Linq;
using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class SplitTrack
    {
        public SubTrack[] SubTracks { get; private set; }
        public TrackDTO TrackDto { get; private set; }
        public SubTrackNode FirstTrackNode { get; private set; }
        //public bool AreSubTracksWarmedUp { get; set; }

        public SplitTrack(TrackDTO trackDto, SubTrack[] subTracks, SubTrackNode firstTrackNode)
        {
            SubTracks = subTracks;
            TrackDto = trackDto;
            FirstTrackNode = firstTrackNode;
        }

        public override string ToString()
        {
            return string.Join(",\n------\n", SubTracks.Select(subTrack => subTrack.ToString()).ToArray());
        }
    }
}
