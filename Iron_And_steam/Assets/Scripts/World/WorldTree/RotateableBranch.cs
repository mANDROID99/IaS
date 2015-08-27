using IaS.Domain;
using UnityEngine;

namespace IaS.World.WorldTree
{
    public abstract class RotateableBranch : BaseTree
    {
        public readonly RotationData Data;
        public BlockBounds RotatedBounds { get { return Data.RotatedBounds; } }
        public BlockBounds OriginalBounds { get { return Data.OriginalBounds; } }

        protected RotateableBranch(string name, Vector3 pos, RotationData data, BaseTree parent) : base(name, pos, parent, NodeConfig.Dynamic)
        {
            Data = data;
        }

        public struct RotationData
        {
            public readonly BlockBounds RotatedBounds;
            public readonly BlockBounds OriginalBounds;
            public readonly bool BlocksRotation;
            
            public RotationData(BlockBounds originalBounds, bool blocksRotation) : this()
            {
                OriginalBounds = originalBounds;
                RotatedBounds = originalBounds.Copy();
                BlocksRotation = blocksRotation;
            }
        }
    }
}
