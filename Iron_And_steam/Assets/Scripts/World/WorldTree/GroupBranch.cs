using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using UnityEngine;

namespace IaS.Domain.WorldTree
{
    public class GroupBranch : RotateableBranch
    {
        public readonly BaseTree ParticlesLeaf;
        public readonly BaseTree DoodadsLeaf;
        public readonly Data GroupData;
        public readonly LevelTree Level;
        public readonly List<BlockBounds> RotateableBounds = new List<BlockBounds>(); 
        public readonly Dictionary<BlockBounds, RotateableBranch> RotateableBranches = new Dictionary<BlockBounds, RotateableBranch>();

        public IList<SplitTrack> Tracks { get { return GroupData.Group.Tracks; } }
        public IList<Split> Splits { get { return GroupData.Group.Splits; } }
        public IList<Junction> Junctions { get { return GroupData.Group.Junctions; } }
        public string GroupId { get { return GroupData.Group.Id; } }
        public IList<MeshBlock> SplittedMeshBlocks { get { return GroupData.Group.SplittedMeshBlocks; } }

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
           return new GroupBranch(groupData, rotationData, level);
        }

        private GroupBranch(Data groupData, RotationData rotationData, LevelTree level) : base(groupData.Group.Id, rotationData, null, level)
        {
            Level = level;
            Group = this;
            GroupData = groupData;
            ParticlesLeaf = new BaseTree("Particles", new Vector3(), this);
            DoodadsLeaf = new BaseTree("Doodads", new Vector3(), this);
            level.AddGroupBranch(this);

            foreach (SplitTrack track in Tracks)
            {
                track.OnAttachedToGroupBranch(this);
            }
        }

        public void AddRotateableBranch(BlockBounds rotateableBounds, RotateableBranch rotateableBranch)
        {
            RotateableBounds.Add(rotateableBounds);
            RotateableBranches.Add(rotateableBounds, rotateableBranch);
        }

        public RotateableBranch GetRotateableBranch(BlockBounds bounds)
        {
            return RotateableBranches[bounds];
        }

        public RotateableBranch RotateableRegionContaining(BlockBounds bounds)
        {
            return RotateableBranches.First(kv => kv.Key.Contains(bounds)).Value;
        }
    }
}
