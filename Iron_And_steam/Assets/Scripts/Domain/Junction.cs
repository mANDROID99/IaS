using System.Linq;
using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class Junction
    {
        public enum BranchType
        {
            BranchDefault,
            BranchAlternate
        }

        public enum JunctionDirection
        {
            OneToMany,
            ManyToOne
        }

        public readonly SubTrackGroup BranchDefault;
        public readonly SubTrackGroup BranchAlternate;
        public readonly JunctionDirection Direction;

        public static Junction FromXml(JunctionXML junction, SplitTrack[] splitTracks)
        {
            SplitTrack branchLeft = splitTracks.First(t => junction.BranchDefault == t.TrackXml);
            SplitTrack branchRight = splitTracks.First(t => junction.BranchAlternate == t.TrackXml);
            return new Junction(branchLeft.FirstSubTrack.FirstGroup, branchRight.FirstSubTrack.FirstGroup, junction.Direction);
        }

        public BranchType NextBranchType { get; private set; }

        public SubTrackGroup NextBranch { get { return NextBranchType == BranchType.BranchDefault ? BranchDefault : BranchAlternate; } }

        public Junction(SubTrackGroup branchDefault, SubTrackGroup branchAlternate, JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            NextBranchType = BranchType.BranchDefault;
            this.Direction = direction;
        }

        public void SwitchDirection()
        {
            NextBranchType = (NextBranchType == BranchType.BranchDefault) ? BranchType.BranchAlternate : BranchType.BranchDefault;
        }

        public bool ReferencesGroup(SubTrackGroup group)
        {
            return BranchDefault == group || BranchAlternate == group;
        }
    }
}
