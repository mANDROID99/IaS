using System;
using System.Collections.Generic;
using IaS.Domain;
using IaS.Helpers;

namespace IaS.WorldBuilder.XML.mappers
{
    public class SplitAttachMapper : IXmlValueMapper<SplitAttachment>
    {
        private readonly IEnumerable<Split> Splits;

        public SplitAttachMapper(IEnumerable<Split> splits)
        {
            Splits = splits;
        }

        public SplitAttachment Map(string from)
        {
            string[] parts = from.Split(':');
            string reference = parts[0].Trim();
            string side = parts[1].Trim();

            var refMapper = new ReferenceMapper<Split>(Splits);
            Split found = refMapper.Map(reference);

            if ("1".Equals(side))
            {
                return new SplitAttachment(found, true);
            }
            if ("0".Equals(side))
            {
                return new SplitAttachment(found, false);
            }

            throw new Exception(string.Format("Value {0} is not a valid side.", side));
        }
    }
}
