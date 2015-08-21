using IaS.Domain.XML;

namespace IaS.Xml
{
    public class XmlSplitSide
    {

        public readonly string SplitId;
        public readonly bool Lhs;

        public XmlSplitSide(string splitId, bool lhs)
        {
            SplitId = splitId;
            Lhs = lhs;
        }

        public static XmlSplitSide FromString(string attachment)
        {
            string[] split = attachment.Split(':');
            return new XmlSplitSide(split[0], split[1].Equals("0"));
        }
    }
}
