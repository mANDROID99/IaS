using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class Junction
    {
        public enum BranchType
        {
            BranchDefault, BranchAlternate
        }

        public readonly SplitTrack Root;
        public readonly SplitTrack BranchDefault;
        public readonly SplitTrack BranchAlternate;

        public BranchType NextBranch { get; private set; }

        public Junction(SplitTrack root, SplitTrack branchDefault, SplitTrack branchAlternate)
        {
            Root = root;
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
