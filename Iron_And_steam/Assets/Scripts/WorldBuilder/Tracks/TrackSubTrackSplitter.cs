using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.Helpers;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.WorldBuilder.Tracks
{
    public class TrackSubTrackSplitter
    {
        private TrackBuilderConfiguration config;

        public TrackSubTrackSplitter(TrackBuilderConfiguration config)
        {
            this.config = config;
        }

        public SplitTrack SplitTrack(TrackXML trackXml, BlockBounds[] splitBounds)
        {
            List<SubTrackNode> subTrackNodes = CreateTrackNodes(trackXml);
            BlockBounds firstSplitBounds;
            BlockBounds lastSplitBounds;
            Dictionary<BlockBounds, List<List<SubTrackNode>>> splitTrackDict = SplitSubTrackNodes(subTrackNodes, splitBounds, out firstSplitBounds, out lastSplitBounds);
            
            SplitTrackBuilder splitTrackBuilder = new SplitTrackBuilder(trackXml);
            foreach (BlockBounds bounds in splitTrackDict.Keys.Where(k => splitTrackDict[k].Count > 0))
            {
                var nodesLists = splitTrackDict[bounds];

                SubTrackBuilder subTrackBuilder = splitTrackBuilder.SubTracks.FirstOrDefault(st => st.SplitBounds.Equals(bounds));

                if (subTrackBuilder == null)
                    splitTrackBuilder.WithSubTrack((subTrackBuilder = new SubTrackBuilder(bounds)));

                if (bounds == firstSplitBounds) splitTrackBuilder.WithFirstSubTrack(subTrackBuilder);
                if (bounds == lastSplitBounds) splitTrackBuilder.WithLastSubTrack(subTrackBuilder);

                foreach (List<SubTrackNode> nodes in nodesLists)
                {
                    subTrackBuilder.WithTrackGroup(new SubTrackGroupBuilder().WithNodes(nodes));
                }
                subTrackBuilder.WithTrackGroupAutoFirstLast();
            }

            return splitTrackBuilder.Build();
        }

        private Vector3 GetOffset(Vector3 forward)
        {
            return (new Vector3(0.5f, 0.5f, 0.5f) - forward / 2f);
        }

        private Dictionary<BlockBounds, List<List<SubTrackNode>>> SplitSubTrackNodes(List<SubTrackNode> trackNodes, BlockBounds[] splitBounds, out BlockBounds firstSplitBounds, out BlockBounds lastSplitBounds)
        {
            List<BlockBounds> queue = new List<BlockBounds>();
            Dictionary<BlockBounds, List<List<SubTrackNode>>> splitTrackDict = splitBounds.ToDictionary(b => b, b => new List<List<SubTrackNode>>());

            BlockBounds currentSplitBounds = null;
            List<SubTrackNode> currentSubTrackNodes = null;

            for (int i = 0; i < trackNodes.Count; i++ )
            {
                SubTrackNode nextNode = trackNodes[i];

                if ((currentSplitBounds == null) || (!currentSplitBounds.Contains(nextNode.Position)))
                {
                    List<SubTrackNode> nextSubTrackNodes = new List<SubTrackNode>();
                    BlockBounds nextSubBounds = splitBounds.First(b => b.Contains(nextNode.Position));

                    if(currentSplitBounds != null)
                    {
                        if(trackNodes[i - 1].Position != nextNode.Position)
                        {
                            SubTrackNode previousNode = trackNodes[i - 1];
                            Vector3 actualPrevPos = previousNode.Position + GetOffset(nextNode.Forward);
                            Vector3 actualIntersectPos = nextSubBounds.ToAxisAlignedBounds().ClosestPoint(actualPrevPos);
                            Vector3 intersectPos = actualIntersectPos - new Vector3(0.5f, 0.5f, 0.5f) + nextNode.Forward / 2f;

                            if ((intersectPos != previousNode.Position) || (intersectPos != nextNode.Position))
                            {
                                bool intersectsPrev = intersectPos == previousNode.Position;
                                bool intersectsNext = intersectPos == nextNode.Position;

                                if (intersectsPrev && !intersectsNext)
                                {
                                    nextSubTrackNodes.Add(previousNode);
                                    UpdateLinks(previousNode, nextNode);
                                }else if (intersectsNext && !intersectsPrev)
                                {
                                    currentSubTrackNodes.Add(nextNode);
                                    UpdateLinks(previousNode, nextNode);
                                }else if (!intersectsNext && !intersectsPrev)
                                {
                                    SubTrackNode intersectNode = new SubTrackNode(intersectPos, previousNode.Forward, previousNode.Down);
                                    UpdateLinks(intersectNode, nextNode);
                                    UpdateLinks(previousNode, intersectNode);
                                    currentSubTrackNodes.Add(intersectNode);
                                    nextSubTrackNodes.Add(intersectNode);
                                }
                            }
                        }

                        if (currentSubTrackNodes.Count > 1)
                        {
                            queue.Add(currentSplitBounds);
                            splitTrackDict[currentSplitBounds].Add(currentSubTrackNodes);
                        }
                    }

                    currentSubTrackNodes = nextSubTrackNodes;
                    currentSplitBounds = nextSubBounds;
                }

                currentSubTrackNodes.Add(nextNode);
            }

            if (currentSubTrackNodes.Count > 1)
            {
                queue.Add(currentSplitBounds);
                splitTrackDict[currentSplitBounds].Add(currentSubTrackNodes);
            }

            firstSplitBounds = queue.FirstOrDefault();
            lastSplitBounds = queue.LastOrDefault();
            return splitTrackDict;
        }

        private void UpdateLinks(SubTrackNode previous, SubTrackNode next)
        {
            if(previous != null)
                previous.Next = next;
            if(next != null)
                next.Previous = previous;
        }

        private List<SubTrackNode> CreateTrackNodes(TrackXML trackXml)
        {
            SubTrackNode previousTrackNode = null;
            Vector3 down = trackXml.Down;
            Vector3? lastForward = trackXml.StartDir;

            TrackNodeXML[] nodesXml = trackXml.NodesXml;
            var subTrackNodes = new List<SubTrackNode>();

            if(nodesXml.Length < 2) return subTrackNodes;

            for (int i = 0; i < nodesXml.Length; i++)
            {
                Vector3 position = nodesXml[i].Position;
                Vector3? previousPos = i > 0 ? nodesXml[i - 1].Position : (Vector3?)null;
                Vector3? nextPos = i < nodesXml.Length - 1 ? nodesXml[i + 1].Position : (Vector3?)null;

                Vector3? nextForward = nextPos.HasValue ? (nextPos.Value - position).normalized : (Vector3?)null;
                Vector3 forward;
                if (i == 0)
                {
                    forward = trackXml.StartDir ?? (nextPos.Value - position).normalized;
                }
                else
                {
                    forward = (position - previousPos.Value).normalized;
                }

                down = GetNextDownDirection(forward, lastForward, down);
                lastForward = forward;

                var currentNode = new SubTrackNode(position, forward, down);
                UpdateLinks(previousTrackNode, currentNode);

                previousTrackNode = currentNode;
                subTrackNodes.Add(currentNode);

                if ((nextForward.HasValue) && (Vector3.Angle(forward, nextForward.Value) > 0.1f))
                {
                    // corner
                    down = GetNextDownDirection(nextForward.Value, lastForward, down);
                    lastForward = nextForward;

                    var cornerNode = new SubTrackNode(position, nextForward.Value, down);
                    UpdateLinks(currentNode, cornerNode);

                    previousTrackNode = cornerNode;
                    subTrackNodes.Add(cornerNode);
                }
            }

            return subTrackNodes;
        }

        private Vector3 GetNextDownDirection(Vector3 forward, Vector3? lastForward, Vector3 down)
        {
            if (lastForward.HasValue)
            {
                down = Quaternion.FromToRotation(lastForward.Value, forward) * down;
                down = MathHelper.RoundVector3ToDp(down, 3);
            }
            return down;
        }
    }
}
