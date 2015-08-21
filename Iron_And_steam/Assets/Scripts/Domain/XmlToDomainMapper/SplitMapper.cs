using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class SplitMapper
    {

        public Split MapXmlToDomain(XmlSplit xmlSplit, Group group)
        {
            Split split = new Split(xmlSplit.Id, group, xmlSplit.Axis, xmlSplit.Pivot, xmlSplit.Value, xmlSplit.Restriction);
            return split;
        }
    }
}
