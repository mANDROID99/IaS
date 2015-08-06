using IaS.WorldBuilder;

namespace IaS.Domain
{
    public class SubTrack
    {
        public readonly BlockBounds SplitBounds;
        public readonly SubTrackGroup[] TrackGroups;
        public readonly SubTrackGroup FirstGroup;
        public readonly SubTrackGroup LastGroup;

        public int NumGroups
        {
            get { return TrackGroups.Length; }
        }

        public SubTrack(BlockBounds split, SubTrackGroup[] trackGroups, SubTrackGroup firstGroup, SubTrackGroup lastGroup)
        {
            FirstGroup = firstGroup;
            LastGroup = lastGroup;
            SplitBounds = split;
            TrackGroups = trackGroups;

            foreach (SubTrackGroup trackGroup in trackGroups) trackGroup.SubTrack = this;
        }
    }

    
}
