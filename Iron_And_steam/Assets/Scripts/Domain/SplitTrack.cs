using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class SplitTrack
    {
        public SubTrack[] SubTracks { get; private set; }
        public TrackXML TrackXml { get; private set; }

        public readonly SubTrack FirstSubTrack;
        public readonly SubTrack LastSubTrack;

        public string Id
        {
            get { return TrackXml.Id; }
        }

        public SplitTrack(TrackXML trackXml, SubTrack[] subTracks, SubTrack firstSubTrack, SubTrack lastSubTrack)
        {
            SubTracks = subTracks;
            TrackXml = trackXml;
            FirstSubTrack = firstSubTrack;
            LastSubTrack = lastSubTrack;
        }

        public IEnumerable<SubTrackGroup> AllSubTrackGroups()
        {
            return SubTracks.SelectMany(s => s.TrackGroups);
        }

        public override string ToString()
        {
            return string.Join(",\n------\n", SubTracks.Select(subTrack => subTrack.ToString()).ToArray());
        }
    }
}
