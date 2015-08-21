using IaS.Xml;

namespace IaS.Domain
{
    public class SplitAttachment
    {
        public readonly Reference<Split> Split; 
        public readonly Group AttachedGroup;
        public readonly bool Lhs;

        public SplitAttachment(Group attachedGroup, Reference<Split> split, bool lhs)
        {
            Split = split;
            AttachedGroup = attachedGroup;
            Lhs = lhs;
        }
    }
}
