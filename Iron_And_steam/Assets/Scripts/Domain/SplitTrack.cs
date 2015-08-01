using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.Domain
{
    public class SplitTrack
    {
        public SubTrack[] SubTracks { get; private set; }
        public TrackDTO TrackDto { get; private set; }

        public readonly SubTrack FirstSubTrack;
        public readonly SubTrack LastSubTrack;

        public SplitTrack(TrackDTO trackDto, SubTrack[] subTracks, SubTrack firstSubTrack, SubTrack lastSubTrack)
        {
            SubTracks = subTracks;
            TrackDto = trackDto;
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
