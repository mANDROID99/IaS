using System.Collections.Generic;
using IaS.Helpers;
using IaS.Xml;
using UnityEngine;

namespace IaS.Domain.XmlToDomainMapper
{
    public class SubTrackNodesBuilder
    {

        private Vector3 GetStartForward(Track track)
        {
            return track.StartDir ?? GetDirection(track.Nodes[0].Position, track.Nodes[1].Position);
        }

        private Vector3 GetDirection(Vector3 a, Vector3 b)
        {
            return (b - a).normalized;
        }

        private Vector3 GetNextDownDirection(Vector3 forward, Vector3 lastForward, Vector3 down)
        {
            down = Quaternion.FromToRotation(lastForward, forward) * down;
            return MathHelper.RoundVector3ToDp(down, 3);
        }

        private void UpdateLinks(SubTrackNode previous, SubTrackNode next)
        {
            if (previous != null)
                previous.Next = next;
            if (next != null)
                next.Previous = previous;
        }

        public List<SubTrackNode> Build(Track track)
        {
            List<SubTrackNode> stNodes = new List<SubTrackNode>();
            XmlTrackNode[] nodes = track.Nodes;

            if (nodes.Length < 2) return new List<SubTrackNode>();

            SubTrackNode previousStNode = null;
            Vector3 down = track.Down;
            Vector3 lastForward = GetStartForward(track);

            for (int i = 0; i < nodes.Length; i++)
            {
                Vector3 forward = i > 0
                    ? GetDirection(nodes[i - 1].Position, nodes[i].Position)
                    : lastForward;

                Vector3 nextForward = i < nodes.Length - 1
                    ? GetDirection(nodes[i].Position, nodes[i + 1].Position)
                    : forward;

                down = GetNextDownDirection(forward, lastForward, down);
                lastForward = forward;

                SubTrackNode stNode = new SubTrackNode(nodes[i].Position, forward, down);
                stNodes.Add(stNode);
                UpdateLinks(previousStNode, stNode);
                previousStNode = stNode;

                // create a duplicate corner node if required
                if (Vector3.Angle(forward, nextForward) > 0.1f)
                {
                    down = GetNextDownDirection(nextForward, lastForward, down);
                    lastForward = nextForward;

                    SubTrackNode cornerNode = new SubTrackNode(nodes[i].Position, nextForward, down);
                    stNodes.Add(cornerNode);
                    UpdateLinks(stNode, cornerNode);
                    previousStNode = cornerNode;
                }
            }

            return stNodes;
        }
    }
}
