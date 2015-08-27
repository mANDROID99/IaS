using IaS.World.WorldTree;
using UnityEngine;

namespace IaS.Domain
{
    public class Junction
    {
        public enum BranchType { BranchDefault, BranchAlternate }
        public enum JunctionDirection { OneToMany, ManyToOne }

        public readonly string Id;
        public readonly SubTrackGroup BranchDefault;
        public readonly SubTrackGroup BranchAlternate;
        public readonly JunctionDirection Direction;

        public BranchType NextBranchType { get; private set; }

        public SubTrackGroup NextBranch
        {
            get { return NextBranchType == BranchType.BranchDefault ? BranchDefault : BranchAlternate; }
        }

        public Vector3 NextDirection
        {
            get { return DirectionFromTrackGroup(NextBranch); }
        }

        public Vector3 Position
        {
            get { return Direction == JunctionDirection.OneToMany ? BranchDefault.StartPos : BranchDefault.EndPos;}
        }

        public Junction(string id, SplitTrack branchDefault, SplitTrack branchAlternate, JunctionDirection direction)
        {
            Id = id;
            NextBranchType = BranchType.BranchDefault;
            Direction = direction;

            BranchDefault = GetFirstGroup(branchDefault);
            BranchAlternate = GetFirstGroup(branchAlternate);
        }

        private SubTrackGroup GetFirstGroup(SplitTrack subTrack)
        {
            return Direction == JunctionDirection.OneToMany ? subTrack.FirstSubTrack.FirstGroup : subTrack.LastSubTrack.LastGroup;
        }
        
        public void SwitchDirection()
        {
            NextBranchType = (NextBranchType == BranchType.BranchDefault) ? BranchType.BranchAlternate : BranchType.BranchDefault;
        }

        public bool ReferencesGroup(SubTrackGroup group)
        {
            return BranchDefault == group || BranchAlternate == group;
        }

        public BaseTree GetSplitBoundsBranch(GroupBranch groupBranch)
        {
            return groupBranch.SplitBoundsBranchContaining(BranchDefault.SubTrack.SplittedRegion);
        }

        private Vector3 DirectionFromTrackGroup(SubTrackGroup trackGroup)
        {
            return trackGroup.NumTrackNodes > 1 ? trackGroup.Nodes[1].Forward : trackGroup.Nodes[0].Forward;
        }
    }
}
