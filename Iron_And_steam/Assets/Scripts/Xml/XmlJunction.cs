using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain;

namespace IaS.Xml
{
    public class XmlJunction
    {
        public const string ElementJunction = "junction";
        private const string AttrJunctionId = "id";
        private const string AttrJunctionBranchDefault = "branch_default";
        private const string AttrJunctionBranchAlternate = "branch_alternate";
        private const string AttrJunctionDirection = "type";

        public readonly string Id;
        public readonly string BranchDefaultId;
        public readonly string BranchAlternateId;
        public readonly Junction.JunctionDirection Direction;

        public static XmlJunction FromElement(XElement element, Track[] tracks, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrJunctionId).AsIdValue("junction", counts);
            string branchDefault = XmlValueMapper.FromAttribute(element, AttrJunctionBranchDefault).AsText().MandatoryValue();
            string branchAlternate = XmlValueMapper.FromAttribute(element, AttrJunctionBranchAlternate).AsText().MandatoryValue();
            Junction.JunctionDirection direction = XmlValueMapper.FromAttribute(element, AttrJunctionDirection).AsEnum<Junction.JunctionDirection>().OptionalValue(Junction.JunctionDirection.OneToMany);
            return new XmlJunction(id, branchDefault, branchAlternate, direction);
        }

        public XmlJunction(string id, string branchDefaultId, string branchAlternateId, Junction.JunctionDirection direction)
        {
            Id = id;
            BranchDefaultId = branchDefaultId;
            BranchAlternateId = branchAlternateId;
            Direction = direction;
        }
        
    }
}
