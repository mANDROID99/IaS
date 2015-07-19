using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.GameState;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.Splines;

namespace IaS.WorldBuilder.Tracks
{
    public class SubTrack
    {
        public BlockBounds subBounds { get; private set; }
        public SubTrackGroup[] trackGroups { get; private set; }
        public InstanceWrapper instanceWrapper { get; set; }

        public SubTrack(BlockBounds subBounds, SubTrackGroup[] trackGroups)
        {
            this.instanceWrapper = null;
            this.subBounds = subBounds;
            this.trackGroups = trackGroups;
            UpdateReferences();
        }

        private void UpdateReferences()
        {
            foreach (SubTrackGroup trackGroup in trackGroups)
            {
                trackGroup.subTrack = this;
                trackGroup.UpdateReferences();
            }
        }
    }

    public class SubTrackGroup
    {
        public SubTrackNode[] nodes { get; private set; }
        public SubTrack subTrack { get; internal set; }
        public BezierSpline spline { get; internal set; }
        public int NumTrackNodes
        {
            get
            {
                return nodes.Length;
            }
        }

        public SubTrackNode this[int i]{
            get { 
                return nodes[i];
            }
        }

        public SubTrackNode Last()
        {
            return nodes.Last();
        }

        public SubTrackGroup(BezierSpline spline, SubTrackNode[] nodes)
        {
            this.spline = spline;
            this.nodes = nodes;
        }

        internal void UpdateReferences()
        {
            foreach (SubTrackNode node in nodes)
            {
                node.group = this;
            }
        }
    }

    public class SubTrackNode
    {
        public Vector3 position { get; private set; }
        public SubTrackGroup group { get; internal set; }

        public Vector3 forward { get; set; }
        public Vector3 down { get; set; }

        public SubTrackNode previous { get; set; }
        public SubTrackNode next { get; set; }

        public SubTrackNode(Vector3 position, Vector3 forward, Vector3 down)
        {
            this.position = position;
            this.forward = forward;
            this.down = down;
        }

        public override string ToString()
        {
            return String.Format("{{pos:{0}, fwd:{1}}}", position, forward);
        }

        
    }
}
