using System.Linq;
using IaS.GameState;
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

        public Vector3 StartBezierPos
        {
            get { return spline.pts[0].startPos; }
        }

        public Vector3 EndBezierPos
        {
            get { return spline.pts.Last().endPos; }
        }

        public Vector3 StartForward
        {
            get { return nodes[0].forward; }
        }

        public Vector3 EndForward
        {
            get { return nodes[nodes.Length - 1].forward; }
        }

        public bool HasInstance
        {
            get { return subTrack.HasInstance; }
        }

        public InstanceWrapper InstanceWrapper
        {
            get { return subTrack.InstanceWrapper; }
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
