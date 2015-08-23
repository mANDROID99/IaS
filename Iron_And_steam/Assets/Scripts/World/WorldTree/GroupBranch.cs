using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using UnityEngine;
using IaS.Helpers;

namespace IaS.World.WorldTree
{
    public class GroupBranch : RotateableBranch
    {
        public readonly BaseTree ParticlesLeaf;
        public readonly BaseTree DoodadsLeaf;
        public readonly Data GroupData;
        public readonly LevelTree Level;

        public readonly List<SplitBoundsBranch> SplitBoundsBranches = new List<SplitBoundsBranch>(); 

        public IList<SplitTrack> Tracks { get { return GroupData.Group.Tracks; } }
        public IList<Split> Splits { get { return GroupData.Group.Splits; } }
        public IList<Junction> Junctions { get { return GroupData.Group.Junctions; } }
        public string GroupId { get { return GroupData.Group.Id; } }
        public IList<MeshBlock> SplittedMeshBlocks { get { return GroupData.Group.SplittedMeshBlocks; } }
        public Group Group { get { return GroupData.Group; } }

        public new struct Data
        {
            public readonly Group Group;

            public Data(Group group)
            {
                Group = group;
            }
        }

        public static GroupBranch CreateAndAttachTo(LevelTree level, Data groupData, RotationData rotationData)
        {
           return new GroupBranch(new Vector3(), groupData, rotationData, level);
        }

        private GroupBranch(Vector3 pos, Data groupData, RotationData rotationData, LevelTree level) : base(groupData.Group.Id, pos, rotationData, level)
        {
            Level = level;
            GroupData = groupData;
            ParticlesLeaf = new BaseTree("Particles", new Vector3(), this, NodeConfig.Dynamic);
            DoodadsLeaf = new BaseTree("Doodads", new Vector3(), this, NodeConfig.Dynamic);
            level.AddGroupBranch(this);

            foreach (SplitTrack track in Tracks)
            {
                track.OnAttachedToGroupBranch(this);
            }
        }

        public void AddSplitBoundsBranch(SplitBoundsBranch splitBoundsBranch)
        {
            SplitBoundsBranches.Add(splitBoundsBranch);
        }

        private SplitBoundsBranch CreateNewSplitBounds(BlockBounds bounds)
        {
            BlockBounds splittedRegion = GroupData.Group.SplittedRegions.First(sr => sr.Contains(bounds));
            return SplitBoundsBranch.CreateAndAttachTo(SplitBoundsBranches.Count, this, new RotationData(splittedRegion, true));
        }

        public SplitBoundsBranch SplitBoundsBranchContaining(BlockBounds bounds)
        {
            return SplitBoundsBranches.FirstOrDefault(s => s.BlockBounds.Contains(bounds)) ?? CreateNewSplitBounds(bounds);
        }
    }
}
