using IaS.Domain;

namespace IaS.Scripts.Domain
{
    public class RotationState
    {
        public readonly bool BlocksRotation;
        public readonly BlockBounds RotatedBounds;

        public RotationState(BlockBounds bounds, bool blocksRotation)
        {
            RotatedBounds = bounds.Copy();
            BlocksRotation = blocksRotation;
        }
    }
}
