using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class Junction
    {
        public enum BranchType
        {
            BranchDefault, BranchAlternate
        }

        public readonly SubTrackGroup BranchDefault;
        public readonly SubTrackGroup BranchAlternate;

        public BranchType NextBranch { get; private set; }

        public Junction(SubTrackGroup branchDefault, SubTrackGroup branchAlternate)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            NextBranch = BranchType.BranchDefault;
        }

        public void SwitchDirection()
        {
            NextBranch = (NextBranch == BranchType.BranchDefault) ? BranchType.BranchAlternate : BranchType.BranchDefault;
        }
    }
}
