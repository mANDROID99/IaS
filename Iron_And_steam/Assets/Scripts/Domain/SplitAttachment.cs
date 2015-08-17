using IaS.WorldBuilder;

namespace IaS.Domain
{
    public class SplitAttachment
    {

        public readonly Split Split;
        public readonly bool Lhs;

        public SplitAttachment(Split split, bool lhs)
        {
            Split = split;
            Lhs = lhs;
        }
    }
}
