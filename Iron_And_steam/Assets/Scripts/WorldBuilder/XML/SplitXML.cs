using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain;
using UnityEngine;

namespace IaS.WorldBuilder.XML
{
    public class SplitXML : IXmlReferenceable
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
        public readonly Split.RestrictionType Restriction;
        private readonly List<SplitAttachment> _attachedGroupIds = new List<SplitAttachment>(); 

        public SplitXML(string id, Vector3 axis, float value, Vector3 pivot, Split.RestrictionType restriction)
        {
            Id = id;
            Axis = axis;
            Value = value;
            Pivot = pivot;
            Restriction = restriction;
        }

        public void AddAttachedGroupId(string groupId, bool lhs)
        {
            _attachedGroupIds.Add(new SplitAttachment(groupId, lhs));
        }

        public static SplitXML FromElement(XElement element, Dictionary<string, int> counts)
        {
            string id = XmlValueMapper.FromAttribute(element, AttrSplitId).AsIdValue("split", counts);
            Vector3 axis = XmlValueMapper.FromAttribute(element, AttrSplitAxis).AsAxis().MandatoryValue();
            float value = XmlValueMapper.FromAttribute(element, AttrSplitValue).AsFloat().MandatoryValue();
            Vector3 pivot = XmlValueMapper.FromAttribute(element, AttrSplitPivot).AsVector3().MandatoryValue();
            Split.RestrictionType restriction = XmlValueMapper.FromAttribute(element, AttrSplitRestrict).AsEnum<Split.RestrictionType>().OptionalValue(Split.RestrictionType.Both);
            return new SplitXML(id, axis, value, pivot, restriction);
        }

        public string GetId()
        {
            return Id;
        }

        public Split ToSplit(string groupId)
        {
            return new Split(Id, groupId, _attachedGroupIds.ToArray(), Axis, Pivot, Value, Restriction);
        }
    }
}
