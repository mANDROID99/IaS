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

        public BranchType NextBranch { get; private set; }

        public Junction(SubTrackGroup branchDefault, SubTrackGroup branchAlternate, JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            NextBranch = BranchType.BranchDefault;
            this.Direction = direction;
        }

        public void SwitchDirection()
        {
            NextBranch = (NextBranch == BranchType.BranchDefault) ? BranchType.BranchAlternate : BranchType.BranchDefault;
        }

        public bool ReferencesGroup(SubTrackGroup group)
        {
            return BranchDefault == group || BranchAlternate == group;
        }
    }
}
