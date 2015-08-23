using IaS.Domain;
using UnityEngine;

namespace IaS.World.WorldTree
{
    public class SplitBoundsBranch : RotateableBranch
    {
        public BlockBounds BlockBounds { get { return Data.OriginalBounds; } }
        public readonly BaseTree TracksLeaf;
        public readonly BaseTree BlocksLeaf;
        public readonly BaseTree OthersLeaf;
        public readonly GroupBranch Group;
        public readonly string Name;

        public SplitBoundsBranch(string name, Vector3 pos, RotationData data, GroupBranch group) : base (name, pos, data, group)
        {
            BaseTree staticBranch = new BaseTree("Static", new Vector3(), this, NodeConfig.StaticAndPropogate);
            TracksLeaf = new BaseTree("Tracks", new Vector3(), staticBranch, NodeConfig.StaticAndPropogate);
            BlocksLeaf = new BaseTree("Blocks", new Vector3(), staticBranch, NodeConfig.StaticAndPropogate);

            OthersLeaf = new BaseTree("Others", new Vector3(), this, NodeConfig.Dynamic);

            Group = group;
            group.AddSplitBoundsBranch(this);
            Name = name;
        }

        public static SplitBoundsBranch CreateAndAttachTo(int count, GroupBranch groupBranch, RotationData rotationData)
        {
            string name = "split_" + count;
            return new SplitBoundsBranch(name, new Vector3(), rotationData, groupBranch);
        }
    }
}
