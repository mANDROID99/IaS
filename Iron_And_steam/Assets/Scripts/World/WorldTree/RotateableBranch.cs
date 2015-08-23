using IaS.Domain;
using IaS.Scripts.Domain;
using UnityEngine;

namespace IaS.World.WorldTree
{
    public abstract class RotateableBranch : BaseTree
    {
        public readonly RotationData Data;

        public BlockBounds Bounds { get { return Data.OriginalBounds; } }
        public RotationState RotationState { get { return Data.RotationState; } }

        protected RotateableBranch(string name, Vector3 pos, RotationData data, BaseTree parent) : base(name, pos, parent, NodeConfig.Dynamic)
        {
            Data = data;
        }

        public struct RotationData
        {
            public readonly BlockBounds OriginalBounds;
            public readonly RotationState RotationState;

            public RotationData(BlockBounds originalBounds, bool blocksRotation) : this()
            {
                OriginalBounds = originalBounds;
                RotationState = new RotationState(originalBounds, blocksRotation);
            }
        }
    }
}
