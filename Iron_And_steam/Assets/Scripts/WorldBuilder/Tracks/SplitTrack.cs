using System;
using System.Linq;
using IaS.WorldBuilder.Xml;

namespace IaS.WorldBuilder.Tracks
{
    public class SplitTrack
    {
        public SubTrack[] SubTracks { get; private set; }
        public Track TrackRef { get; private set; }
        public SubTrackNode FirstTrackNode { get; private set; }
        //public bool AreSubTracksWarmedUp { get; set; }

        public SplitTrack(Track track, SubTrack[] subTracks, SubTrackNode firstTrackNode)
        {
            SubTracks = subTracks;
            TrackRef = track;
            FirstTrackNode = firstTrackNode;
        }

        public override string ToString()
        {
            return String.Join(",\n------\n", SubTracks.Select(subTrack => subTrack.ToString()).ToArray());
        }
    }
}
