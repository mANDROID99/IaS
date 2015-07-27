using System.Linq;
using IaS.WorldBuilder.Splines;
using UnityEngine;

namespace IaS.Domain
{
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

        public Vector3 StartPos
        {
            get { return nodes[0].position; }
        }

        public Vector3 EndPos
        {
            get { return nodes[nodes.Length - 1].position; }
        }

        public SubTrackNode this[int i]
        {
            get
            {
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
}
