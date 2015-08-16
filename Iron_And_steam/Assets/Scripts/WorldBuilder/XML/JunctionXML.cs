using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain;
using IaS.WorldBuilder.XML;

namespace IaS.WorldBuilder.Xml
{
    public class JunctionXML
    {
        public const string ElementJunction = "junction";
        private const string AttrJunctionId = "id";
        private const string AttrJunctionBranchDefault = "branch_default";
        private const string AttrJunctionBranchAlternate = "branch_alternate";
        private const string AttrJunctionDirection = "type";

        public readonly string Id;
        public readonly TrackXML BranchDefault;
        public readonly TrackXML BranchAlternate;
        public readonly Junction.JunctionDirection Direction;

        public static JunctionXML FromElement(XElement element, TrackXML[] tracks, Dictionary<string, int> counts)
        {
            string id = XmlValueResult<string>.FromAttribute(element, AttrJunctionId).AsIdValue("junction", counts);
            TrackXML branchDefault = XmlValueResult<string>.FromAttribute(element, AttrJunctionBranchDefault).AsReference(tracks).MandatoryValue();
            TrackXML branchAlternate = XmlValueResult<string>.FromAttribute(element, AttrJunctionBranchAlternate).AsReference(tracks).MandatoryValue();
            Junction.JunctionDirection direction = XmlValueResult<string>.FromAttribute(element, AttrJunctionDirection).AsEnum<Junction.JunctionDirection>().OptionalValue(Junction.JunctionDirection.OneToMany);
            return new JunctionXML(id, branchDefault, branchAlternate, direction);
        }

        public JunctionXML(string id, TrackXML branchDefault, TrackXML branchAlternate, Junction.JunctionDirection direction)
        {
            Id = id;
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            Direction = direction;
        }
        
    }
}
