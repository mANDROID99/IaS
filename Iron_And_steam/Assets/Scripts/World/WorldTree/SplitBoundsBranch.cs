using IaS.Domain;
using UnityEngine;

namespace IaS.World.WorldTree
{
    public class SplitBoundsBranch : RotateableBranch
    {
        public readonly BaseTree TracksLeaf;
        public readonly BaseTree BlocksLeaf;
        public readonly BaseTree OthersLeaf;
        public readonly GroupBranch Group;
        public readonly SplittedRegion SplittedRegion;
        public readonly string Name;

        public SplitBoundsBranch(string name, SplittedRegion splittedRegion, Vector3 pos, RotationData data, GroupBranch group) : base (name, pos, data, group)
        {
            BaseTree staticBranch = new BaseTree("Static", new Vector3(), this, NodeConfig.StaticAndPropogate);
            TracksLeaf = new BaseTree("Tracks", new Vector3(), staticBranch, NodeConfig.StaticAndPropogate);
            BlocksLeaf = new BaseTree("Blocks", new Vector3(), staticBranch, NodeConfig.StaticAndPropogate);
            OthersLeaf = new BaseTree("Others", new Vector3(), this, NodeConfig.Dynamic);

            SplittedRegion = splittedRegion;
            Group = group;
            group.AddSplitBoundsBranch(this);
            Name = name;
        }

        public static SplitBoundsBranch CreateAndAttachTo(int count, SplittedRegion splittedRegion, GroupBranch groupBranch, RotationData rotationData)
        {
            string name = "split_" + count;
            return new SplitBoundsBranch(name, splittedRegion, new Vector3(), rotationData, groupBranch);
        }
    }
}
