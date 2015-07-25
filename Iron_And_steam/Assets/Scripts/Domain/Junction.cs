using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class Junction
    {
        public readonly SplitTrack Root;
        public readonly SplitTrack BranchDefault;
        public readonly SplitTrack BranchAlternate;

        public SplitTrack NextBranch { get; private set; }

        public Junction(SplitTrack root, SplitTrack branchDefault, SplitTrack branchAlternate)
        {
            Root = root;
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            NextBranch = BranchDefault;
        }

        public void SwitchDirection()
        {
            NextBranch =(NextBranch == BranchDefault) ? BranchAlternate : BranchDefault;
        }
    }
}
