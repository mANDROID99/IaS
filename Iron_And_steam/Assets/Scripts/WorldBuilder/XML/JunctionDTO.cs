namespace IaS.WorldBuilder.Xml
{
    public class JunctionDTO
    {
        public enum JunctionDirection
        {
            OneToMany,
            ManyToOne
        }

        public readonly TrackDTO BranchDefault;
        public readonly TrackDTO BranchAlternate;
        public readonly JunctionDirection Direction;

        public JunctionDTO(TrackDTO branchDefault, TrackDTO branchAlternate, JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            Direction = direction;
        }
    }
}
