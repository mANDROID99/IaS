using IaS.Scripts.Domain;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.Domain.WorldTree
{
    public class RotateableBranch : BaseTree
    {
        
        public readonly RotationData Data;
        public readonly string SplitId;
        public readonly BaseTree TracksLeaf;
        public readonly BaseTree BlocksLeaf;
        public readonly BaseTree OthersLeaf;
        public GroupBranch Group { get; protected set; }

        public BlockBounds OriginalBounds { get { return Data.OriginalBounds; } }
        public RotationState RotationState { get { return Data.RotationState; } }

        public RotateableBranch(string splitId, RotationData data, GroupBranch group, BaseTree parent)
            : base(splitId, new Vector3(), parent)
        {
            Group = group;
            Data = data;
            SplitId = splitId;
            TracksLeaf = new BaseTree("Tracks", new Vector3(), this);
            BlocksLeaf = new BaseTree("Blocks", new Vector3(), this);
            OthersLeaf = new BaseTree("Others", new Vector3(), this);
            if (group != null)
            {
                group.AddRotateableBranch(data.OriginalBounds, this);
            }
        }

        public RotateableBranch(string splitId, RotationData data, GroupBranch group) : this(splitId, data, group, group)
        {
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
