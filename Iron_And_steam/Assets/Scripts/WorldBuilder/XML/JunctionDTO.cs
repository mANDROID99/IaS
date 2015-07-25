namespace IaS.WorldBuilder.Xml
{
    public class JunctionDTO
    {
        public TrackDTO Root;
        public TrackDTO BranchDefault;
        public TrackDTO BranchAlternate;

        public JunctionDTO(TrackDTO root, TrackDTO branchDefault, TrackDTO branchAlternate)
        {
            Root = root;
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
        }
    }
}
