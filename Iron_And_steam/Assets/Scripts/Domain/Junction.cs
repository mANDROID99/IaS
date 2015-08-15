using System;
using System.Linq;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.Domain
{
    public class Junction
    {
        public enum BranchType
        {
            BranchDefault,
            BranchAlternate
        }

        public enum JunctionDirection
        {
            OneToMany,
            ManyToOne
        }

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

        public Junction(SubTrackGroup branchDefault, SubTrackGroup branchAlternate, JunctionDirection direction)
        {
            BranchDefault = branchDefault;
            BranchAlternate = branchAlternate;
            NextBranchType = BranchType.BranchDefault;
            Direction = direction;
        }

        public static Junction FromXml(JunctionXML junction, SplitTrack[] splitTracks)
        {
            SplitTrack branchLeft = splitTracks.First(t => junction.BranchDefault == t.TrackXml);
            SplitTrack branchRight = splitTracks.First(t => junction.BranchAlternate == t.TrackXml);
            return new Junction(branchLeft.FirstSubTrack.FirstGroup, branchRight.FirstSubTrack.FirstGroup, junction.Direction);
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
            return groupBranch.GetSplitBoundsBranch(BranchDefault.SubTrack.SplitBounds);
        }

        private Vector3 DirectionFromTrackGroup(SubTrackGroup trackGroup)
        {
            return trackGroup.NumTrackNodes > 1 ? trackGroup.Nodes[1].Forward : trackGroup.Nodes[0].Forward;
        }
    }
}
