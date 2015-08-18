using System;
using System.Collections.Generic;
using IaS.Domain;

namespace IaS.WorldBuilder.XML.mappers
{
    public class SplitAttachMapper
    {

        public static void Create(string groupId, string from, IEnumerable<SplitXML> splits)
        {
            string[] parts = from.Split(':');
            string reference = parts[0].Trim();
            string side = parts[1].Trim();

            var refMapper = new ReferenceMapper<SplitXML>(splits);
            SplitXML found = refMapper.Map(reference);

            if ("1".Equals(side))
            {
                found.AddAttachedGroupId(groupId, false);
                return;
            }
            if ("0".Equals(side))
            {
                found.AddAttachedGroupId(groupId, true);
                return;
            }

            throw new Exception(string.Format("Value {0} is not a valid side.", side));
        }
    }
}
