using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.Domain;
using UnityEngine;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder;
using IaS.Helpers;

namespace IaS.WorldBuilder.Tracks
{
    public class TrackSubTrackSplitter
    {
        private TrackBuilderConfiguration config;

        public TrackSubTrackSplitter(TrackBuilderConfiguration config)
        {
            this.config = config;
        }

        public SplitTrack SplitTrack(TrackDTO trackDto, Split[] splits)
        {
            List<SubTrackNode> subTrackNodes = CreateCornerDuplicatedTrackNodes(trackDto);
            SplitTrack splitTrack = SplitSubTrackNodes(subTrackNodes, splits, trackDto);
            return splitTrack;
        }


        private Vector3 GetOffset(Vector3 forward)
        {
            return (new Vector3(0.5f, 0.5f, 0.5f) - forward / 2f);
        }

        private SplitTrack SplitSubTrackNodes(List<SubTrackNode> trackNodes, Split[] splits, TrackDTO trackDtoRef)
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

            if (currentSubTrackNodes.Count > 1)
            {
                splitTrack[currentSubBounds].Add(currentSubTrackNodes);
            }
            return ConvertDictionaryToSplitTrack(splitTrack, trackNodes[0], trackDtoRef);
        }

        private void UpdateLinks(SubTrackNode previous, SubTrackNode next)
        {
            if(previous != null)
                previous.next = next;
            if(next != null)
                next.previous = previous;
        }

        private List<SubTrackNode> CreateCornerDuplicatedTrackNodes(TrackDTO trackDto)
        {
            SubTrackNode previousTrackNode = null;
            Vector3 down = trackDto.Down;
            Vector3? lastForward = trackDto.StartDir;

            TrackNodeDTO[] nodesDto = trackDto.NodesDto;
            List<SubTrackNode> subTrackNodes = new List<SubTrackNode>();

            if(nodesDto.Length < 2) return subTrackNodes;

            for (int i = 0; i < nodesDto.Length; i++)
            {
                Vector3 position = nodesDto[i].Position;
                Vector3? previousPos = i > 0 ? nodesDto[i - 1].Position : (Vector3?)null;
                Vector3? nextPos = i < nodesDto.Length - 1 ? nodesDto[i + 1].Position : (Vector3?)null;

                Vector3? nextForward = nextPos.HasValue ? (nextPos.Value - position).normalized : (Vector3?)null;
                Vector3 forward;
                if (i == 0)
                {
                    forward = trackDto.StartDir ?? (nextPos.Value - position).normalized;
                }
                else
                {
                    forward = (position - previousPos.Value).normalized;
                }

                down = GetNextDownDirection(forward, lastForward, down);
                lastForward = forward;

                SubTrackNode currentNode = new SubTrackNode(position, forward, down);
                UpdateLinks(previousTrackNode, currentNode);

                previousTrackNode = currentNode;
                subTrackNodes.Add(currentNode);



                if ((nextForward.HasValue) && (Vector3.Angle(forward, nextForward.Value) > 0.1f))
                {
                    // corner
                    down = GetNextDownDirection(nextForward.Value, lastForward, down);
                    lastForward = nextForward;

                    SubTrackNode cornerNode = new SubTrackNode(position, nextForward.Value, down);
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

        


        private SplitTrack ConvertDictionaryToSplitTrack(Dictionary<BlockBounds, List<List<SubTrackNode>>> dict, SubTrackNode firstTrackNode, TrackDTO trackDtoRef)
        {
           SubTrack[] subTracks = dict.Keys
               .Where(subBounds => dict[subBounds].Count > 0)
               .Select(subBounds => {
                    SubTrackGroup[] trackGroups = dict[subBounds].Select(group => new SubTrackGroup(null, group.ToArray())).ToArray();
                    return new SubTrack(subBounds, trackGroups);
                }).ToArray();

           return new SplitTrack(trackDtoRef, subTracks, firstTrackNode);
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
