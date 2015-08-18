namespace IaS.Domain
{
    public class SplitAttachment
    {

        public readonly string AttachedGroupId;
        public readonly bool Lhs;

        public SplitAttachment(string attachedGroupId, bool lhs)
        {
            AttachedGroupId = attachedGroupId;
            Lhs = lhs;
        }
    }
}
