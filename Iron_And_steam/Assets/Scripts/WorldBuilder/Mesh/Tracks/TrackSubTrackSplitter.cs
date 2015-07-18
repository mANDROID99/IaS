using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Xml;
using IaS.Helpers;

namespace IaS.WorldBuilder.Meshes.Tracks
{
    public class TrackSubTrackSplitter
    {
        private TrackBuilderConfiguration config;
        private SubTrackNode firstTrackNode;
        

        public TrackSubTrackSplitter(TrackBuilderConfiguration config)
        {
            this.config = config;
        }

        public SplitTrack SplitTrack(Track track, Split[] splits)
        {
            List<SubTrackNode> subTrackNodes = CreateCornerDuplicatedTrackNodes(track);
            SplitTrack splitTrack = SplitSubTrackNodes(subTrackNodes, splits, track);
            return splitTrack;
        }


        private Vector3 GetOffset(Vector3 forward)
        {
            return (new Vector3(0.5f, 0.5f, 0.5f) - forward / 2f);
        }
        private SplitTrack SplitSubTrackNodes(List<SubTrackNode> trackNodes, Split[] splits, Track trackRef)
        {
            BlockBounds[] splitBounds = SplitContainingBounds(trackNodes, splits);
            Dictionary<BlockBounds, List<List<SubTrackNode>>> splitTrack = splitBounds.ToDictionary(bounds => bounds, bounds => new List<List<SubTrackNode>>());

            BlockBounds currentSubBounds = null;
            List<SubTrackNode> currentSubTrackNodes = null;

            for (int i = 0; i < trackNodes.Count; i++ )
            {
                SubTrackNode nextNode = trackNodes[i];

                if ((currentSubBounds == null) || (!currentSubBounds.Contains(nextNode.position)))
                {
                    List<SubTrackNode> nextSubTrackNodes = new List<SubTrackNode>();
                    BlockBounds nextSubBounds = splitBounds.FirstOrDefault(b => b.Contains(nextNode.position));

                    if(currentSubBounds != null)
                    {
                        if(trackNodes[i - 1].position != nextNode.position)
                        {
                            SubTrackNode previousNode = trackNodes[i - 1];
                            Vector3 actualPrevPos = previousNode.position + GetOffset(nextNode.forward);
                            Vector3 actualIntersectPos = nextSubBounds.ToAxisAlignedBounds().ClosestPoint(actualPrevPos);
                            Vector3 intersectPos = actualIntersectPos - new Vector3(0.5f, 0.5f, 0.5f) + nextNode.forward / 2f;

                            if ((intersectPos != previousNode.position) || (intersectPos != nextNode.position))
                            {
                                bool intersectsPrev = intersectPos == previousNode.position;
                                bool intersectsNext = intersectPos == nextNode.position;

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
                                    SubTrackNode intersectNode = new SubTrackNode(intersectPos, previousNode.forward, previousNode.down);
                                    UpdateLinks(intersectNode, nextNode);
                                    UpdateLinks(previousNode, intersectNode);
                                    currentSubTrackNodes.Add(intersectNode);
                                    nextSubTrackNodes.Add(intersectNode);
                                }
                            }
                        }
                        splitTrack[currentSubBounds].Add(currentSubTrackNodes);
                    }

                    currentSubTrackNodes = nextSubTrackNodes;
                    currentSubBounds = nextSubBounds;
                }

                currentSubTrackNodes.Add(nextNode);
            }

            splitTrack[currentSubBounds].Add(currentSubTrackNodes);
            return ConvertDictionaryToSplitTrack(splitTrack, firstTrackNode, trackRef);
        }

        private void UpdateLinks(SubTrackNode previous, SubTrackNode next)
        {
            if(previous != null)
                previous.next = next;
            if(next != null)
                next.previous = previous;
        }

        private List<SubTrackNode> CreateCornerDuplicatedTrackNodes(Track track)
        {
            SubTrackNode previousTrackNode = null;
            Vector3 down = track.down;
            Vector3? lastForward = null;

            TrackNode[] nodes = track.nodes;
            List<SubTrackNode> subTrackNodes = new List<SubTrackNode>();

            for (int i = 0; i < nodes.Length; i++)
            {
                Vector3 position = nodes[i].position;
                Vector3? previousPos = i > 0 ? nodes[i - 1].position : (Vector3?)null;
                Vector3? nextPos = i < nodes.Length - 1 ? nodes[i + 1].position : (Vector3?)null;

                Vector3 forward = GetForward(position, previousPos, nextPos);
                down = GetNextDownDirection(forward, lastForward, down);
                lastForward = forward;

                SubTrackNode currentNode = new SubTrackNode(position, forward, down);
                UpdateLinks(previousTrackNode, currentNode);

                previousTrackNode = currentNode;
                subTrackNodes.Add(currentNode);

                if ((previousPos.HasValue) && (nextPos.HasValue) && (IsCorner(position, previousPos.Value, nextPos.Value)))
                {
                    // corner
                    Vector3 nextForward = (nextPos.Value - position).normalized;
                    down = GetNextDownDirection(nextForward, lastForward, down);
                    lastForward = nextForward;

                    SubTrackNode cornerNode = new SubTrackNode(position, nextForward, down);
                    UpdateLinks(currentNode, cornerNode);

                    previousTrackNode = cornerNode;
                    subTrackNodes.Add(cornerNode);
                }
            }

            return subTrackNodes;
        }

        private Vector3 GetForward(Vector3 position, Vector3? previousPos, Vector3? nextPos)
        {
            if (previousPos.HasValue)
                return (position - previousPos.Value).normalized;
            if (nextPos.HasValue)
                return (nextPos.Value - position).normalized;
            else return new Vector3();
        }

        private Vector3 GetNextDownDirection(Vector3 forward, Vector3? lastForward, Vector3 down)
        {
            if (lastForward.HasValue)
            {
                down = Quaternion.FromToRotation(lastForward.Value, forward) * down;
                down = MathHelper.RoundVector3ToDp(down, 3);
            }
            lastForward = forward;
            return down;
        }

        private bool IsCorner(Vector3 pos, Vector3 previousPos, Vector3 nextPos)
        {
            return Vector3.Angle((pos - previousPos).normalized, (nextPos - pos).normalized) > 0.1f;
        }


        private SplitTrack ConvertDictionaryToSplitTrack(Dictionary<BlockBounds, List<List<SubTrackNode>>> dict, SubTrackNode firstTrackNode, Track trackRef)
        {
           SubTrack[] subTracks = dict.Keys
               .Where(subBounds => dict[subBounds].Count > 0)
               .Select(subBounds => {
                    SubTrackNode[][] trackNodes = dict[subBounds].Select(list => list.ToArray()).ToArray();
                    return new SubTrack(subBounds, trackNodes);
                }).ToArray();

           return new SplitTrack(trackRef, subTracks, firstTrackNode);
        }


        private BlockBounds[] SplitContainingBounds(List<SubTrackNode> nodes, Split[] splits)
        {
            BlockBounds containingBounds = GetContainingBounds(nodes);
            SplitTree splitTree = new SplitTree(containingBounds);
            splitTree.Split(splits);
            return splitTree.GatherSplitBounds();
        }

        private BlockBounds GetContainingBounds(List<SubTrackNode> nodes)
        {
            float minX = nodes.Min(node => node.position.x);
            float minY = nodes.Min(node => node.position.y);
            float minZ = nodes.Min(node => node.position.z);
            float maxX = nodes.Max(node => node.position.x);
            float maxY = nodes.Max(node => node.position.y);
            float maxZ = nodes.Max(node => node.position.z);
            return new BlockBounds(minX - 1, minY - 1, minZ - 1, maxX + 1, maxY + 1, maxZ + 1);
        }

    }
}
