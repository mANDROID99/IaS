using System.Linq;
using IaS.Domain;

namespace IaS.Domain
{
    public class SubTrack
    {
        public readonly string Id;
        public SplitTrack SplitTrack { get; private set; }
        public readonly BlockBounds SplitBounds;
        public readonly SubTrackGroup[] TrackGroups;
        public SubTrackGroup FirstGroup { get { return TrackGroups[0]; } }
        public SubTrackGroup LastGroup { get { return TrackGroups.Last(); } }

        public int NumGroups
        {
            get { return TrackGroups.Length; }
        }

        public SubTrack(string id, BlockBounds split, SubTrackGroup[] trackGroups)
        {
            SplitBounds = split;
            TrackGroups = trackGroups;
            Id = id;

            foreach (SubTrackGroup trackGroup in trackGroups)
            {
                trackGroup.OnAttachToSubTrack(this);
            }
        }

        public void OnAttachToSplitTrack(SplitTrack splitTrack)
        {

            SplitTrack = splitTrack;
        }

        public static string CreateId(int i)
        {
            return "sub_" + i;
        }
    }

    
}
