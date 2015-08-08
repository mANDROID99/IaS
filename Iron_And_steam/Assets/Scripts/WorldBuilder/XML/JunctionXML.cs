using IaS.Domain;

namespace IaS.WorldBuilder.Xml
{
    public class JunctionXML
    {
        

        public readonly TrackXML BranchDefault;
        public readonly TrackXML BranchAlternate;
        public readonly Junction.JunctionDirection Direction;

        public JunctionXML(TrackXML branchDefault, TrackXML branchAlternate, Junction.JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            Direction = direction;
        }
    }
}
