using IaS.Domain;

namespace IaS.WorldBuilder.Xml
{
    public class JunctionDTO
    {
        

        public readonly TrackXML BranchDefault;
        public readonly TrackXML BranchAlternate;
        public readonly Junction.JunctionDirection Direction;

        public JunctionDTO(TrackXML branchDefault, TrackXML branchAlternate, Junction.JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            Direction = direction;
        }
    }
}
