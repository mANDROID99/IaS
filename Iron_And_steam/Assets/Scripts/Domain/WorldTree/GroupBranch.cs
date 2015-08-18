using System.Collections.Generic;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.Domain.WorldTree
{
    public class GroupBranch : RotateableBranch
    {
        public readonly string GroupId;
        public readonly BaseTree ParticlesLeaf;
        public readonly BaseTree DoodadsLeaf;
        public readonly Data GroupData;
        public readonly LevelTree Level;
        public readonly List<BlockBounds> RotateableBounds = new List<BlockBounds>(); 
        public readonly Dictionary<BlockBounds, RotateableBranch> RotateableBranches = new Dictionary<BlockBounds, RotateableBranch>();

        public SplitTrack[] Tracks { get { return GroupData.Tracks; } }
        public Split[] Splits { get { return GroupData.Splits; } }
        public Junction[] Junctions { get { return GroupData.Junctions; } }

        public new struct Data
        {
            public readonly SplitTrack[] Tracks;
            public readonly Split[] Splits;
            public readonly Junction[] Junctions;

            public Data(SplitTrack[] tracks, Split[] splits, Junction[] junctions)
            {
                Tracks = tracks;
                Splits = splits;
                Junctions = junctions;
            }
        }

        public GroupBranch(string groupId, Data groupData, RotationData rotationData, LevelTree level) : base(groupId, rotationData, null, level)
        {
            Level = level;
            Group = this;
            GroupId = groupId;
            GroupData = groupData;
            ParticlesLeaf = new BaseTree("Particles", new Vector3(), this);
            DoodadsLeaf = new BaseTree("Doodads", new Vector3(), this);
            level.AddGroupBranch(groupId, this);

            foreach (SplitTrack track in GroupData.Tracks)
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
    }
}
