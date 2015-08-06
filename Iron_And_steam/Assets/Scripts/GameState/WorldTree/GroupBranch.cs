using System.Collections.Generic;
using IaS.Domain;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.GameState.WorldTree
{
    public class GroupBranch : BaseTree
    {
        public readonly string GroupId;
        public readonly BaseTree ParticlesLeaf;
        public readonly BaseTree DoodadsLeaf;
        public readonly GroupData Data;
        public readonly LevelTree Level;
        public readonly List<BlockBounds> SplitBounds = new List<BlockBounds>(); 
        private readonly Dictionary<BlockBounds, SplitBoundsBranch> _splitBoundsBranches = new Dictionary<BlockBounds, SplitBoundsBranch>();

        public struct GroupData
        {
            public readonly SplitTrack[] Tracks;
            public readonly Split[] Splits;
            public readonly Junction[] Junctions;

            public GroupData(SplitTrack[] tracks, Split[] splits, Junction[] junctions)
            {
                Tracks = tracks;
                Splits = splits;
                Junctions = junctions;
            }
        }

        public GroupBranch(string groupId, Vector3 position, GroupData data, LevelTree level) : base(groupId, position, level)
        {
            Level = level;
            GroupId = groupId;
            Data = data;
            ParticlesLeaf = new BaseTree("Particles", new Vector3(), this);
            DoodadsLeaf = new BaseTree("Doodads", new Vector3(), this);
            level.AddGroupBranch(groupId, this);
        }

        public void AddSplitBoundsBranch(BlockBounds splitBounds, SplitBoundsBranch splitBoundsBranch)
        {
            SplitBounds.Add(splitBounds);
            _splitBoundsBranches.Add(splitBounds, splitBoundsBranch);
        }

        public SplitBoundsBranch GetSplitBoundsBranch(BlockBounds bounds)
        {
            return _splitBoundsBranches[bounds];
        }

        
    }
}
