using IaS.Scripts.Domain;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.GameState.WorldTree
{
    public class SplitBoundsBranch : BaseTree
    {
        
        public readonly SplitData Data;
        public readonly string SplitId;
        public readonly BaseTree TracksLeaf;
        public readonly BaseTree BlocksLeaf;
        public readonly GroupBranch Group;

        public struct SplitData
        {
            public readonly BlockBounds OriginalBounds;
            public readonly BranchRotation BranchRotation;

            public SplitData(BlockBounds originalBounds) : this()
            {
                OriginalBounds = originalBounds;
                BranchRotation = new BranchRotation(originalBounds);
            }
        }

        public SplitBoundsBranch(string splitId, SplitData data, GroupBranch group) : base(splitId, new Vector3(), group)
        {
            Group = group;
            Data = data;
            SplitId = splitId;
            TracksLeaf = new BaseTree("Tracks", new Vector3(), this);
            BlocksLeaf = new BaseTree("Blocks", new Vector3(), this);
            group.AddSplitBoundsBranch(data.OriginalBounds, this);
        }
    }
}
