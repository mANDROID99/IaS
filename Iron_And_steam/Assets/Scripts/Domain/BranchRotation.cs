using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.Scripts.Domain
{
    public class BranchRotation
    {
        public BlockBounds RotatedBounds { get; set; }
        public Quaternion StartRotation { get; set; }
        public Quaternion EndRotation { get; set; }

        public BranchRotation(BlockBounds bounds)
        {
            RotatedBounds = bounds.Copy();
            EndRotation = Quaternion.identity;
        }
    }
}
