using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain;
using UnityEngine;

namespace IaS.Xml
{
    public class XmlSplit : IReferenceable
    {
        public const string ElementSplit = "split";
        private const string AttrSplitId = "id";
        private const string AttrSplitAxis = "axis";
        private const string AttrSplitValue = "val";
        private const string AttrSplitPivot = "pivot";
        private const string AttrSplitRestrict = "restrict";

        public readonly string Id;
        public readonly Vector3 Axis;
        public readonly float Value;
        public readonly Vector3 Pivot;
        public readonly Domain.Split.RestrictionType Restriction;

        public XmlSplit(string id, Vector3 axis, float value, Vector3 pivot, Domain.Split.RestrictionType restriction)
        {
            Id = id;
            Axis = axis;
            Value = value;
            Pivot = pivot;
            Restriction = restriction;
        }

        public static XmlSplit FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrSplitId).AsIdValue("split", counts);
            Vector3 axis = XmlValueMapper.FromAttribute(element, AttrSplitAxis).AsAxis().MandatoryValue();
            float value = XmlValueMapper.FromAttribute(element, AttrSplitValue).AsFloat().MandatoryValue();
            Vector3 pivot = XmlValueMapper.FromAttribute(element, AttrSplitPivot).AsVector3().MandatoryValue();
            Domain.Split.RestrictionType restriction = XmlValueMapper.FromAttribute(element, AttrSplitRestrict).AsEnum<Domain.Split.RestrictionType>().OptionalValue(Domain.Split.RestrictionType.Both);
            return new XmlSplit(id, axis, value, pivot, restriction);
        }

        public string GetId()
        {
            return Id;
        }
    }
}
