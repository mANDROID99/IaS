﻿using System.Collections.Generic;
using System.Linq;
using IaS.Domain.Splines;
using UnityEngine;

namespace IaS.Domain
{
    public class SubTrackGroup
    {
        public readonly string Id;
        public readonly List<SubTrackNode> Nodes;
        public readonly BezierSpline Spline;
        public SubTrack SubTrack { get; private set; }

        public SubTrackGroup(string id, BezierSpline spline, List<SubTrackNode> nodes)
        {
            Spline = spline;
            Nodes = nodes;
            Id = id;
            foreach (SubTrackNode node in nodes) node.Group = this;
        }

        public int NumTrackNodes
        {
            get { return Nodes.Count;}
        }

        public Vector3 StartPos
        {
            get { return Nodes[0].Position; }
        }

        public Vector3 EndPos
        {
            get { return Nodes[Nodes.Count - 1].Position; }
        }

        public Vector3 StartBezierPos
        {
            get { return Spline.pts[0].startPos; }
        }

        public Vector3 EndBezierPos
        {
            get { return Spline.pts.Last().endPos; }
        }

        public Vector3 StartForward
        {
            get { return Nodes[0].Forward; }
        }

        public Vector3 EndForward
        {
            get { return Nodes[Nodes.Count - 1].Forward; }
        }

        public SubTrackNode Last()
        {
            return Nodes.Last();
        }

        public SubTrackNode FirstNode(bool reverse)
        {
            return !reverse ? Nodes[0] : Nodes.Last();
        }

        public void OnAttachToSubTrack(SubTrack subTrack)
        {
            SubTrack = subTrack;
        }

        public static string CreateId(int i)
        {
            return "group_" + i;
        }
    }

    public class SubTrackNode
    {
        public readonly Vector3 Position;
        public readonly Vector3 Forward;
        public readonly Vector3 Down;

        public SubTrackNode Previous { get; set; }
        public SubTrackNode Next { get; set; }
        public SubTrackGroup Group { get; internal set; }

        public SubTrackNode(Vector3 position, Vector3 forward, Vector3 down)
        {
            Position = position;
            Forward = forward;
            Down = down;
        }

        public override string ToString()
        {
            return string.Format("{{pos:{0}, fwd:{1}}}", Position, Forward);
        }
    }
}
