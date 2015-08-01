using IaS.Domain;

namespace IaS.WorldBuilder.Xml
{
    public class JunctionDTO
    {
        

        public readonly TrackDTO BranchDefault;
        public readonly TrackDTO BranchAlternate;
        public readonly Junction.JunctionDirection Direction;

        public JunctionDTO(TrackDTO branchDefault, TrackDTO branchAlternate, Junction.JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            Direction = direction;
        }
    }
}
