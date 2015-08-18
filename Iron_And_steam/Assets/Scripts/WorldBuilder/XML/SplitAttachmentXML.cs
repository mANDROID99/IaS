using IaS.WorldBuilder.XML;

namespace IaS.WorldBuilder.XML
{
    public class SplitAttachmentXML
    {

        public readonly SplitXML Split;
        public readonly bool Lhs;

        public SplitAttachmentXML(SplitXML split, bool lhs)
        {
            Split = split;
            Lhs = lhs;
        }
    }
}
