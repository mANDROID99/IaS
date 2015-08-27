using System.Collections.Generic;
using System.Linq;
using IaS.Domain.Splines;
using IaS.World.Tracks;
using UnityEngine;

namespace IaS.Domain.XmlToDomainMapper.Tracks
{
    public class SubTrackGroupsBuilder
    {
        private List<SubTrackBuilder> _trackBuilders;
        private TrackSplineGenerator _splineGenerator;

        public List<SubTrackBuilder> Build(TrackSplineGenerator splineGenerator, IList<SubTrackNode> trackNodes, IList<Split> splits, IList<SplittedRegion> splitRegions)
        {
            _splineGenerator = splineGenerator;
            _trackBuilders = new List<SubTrackBuilder>();
            CreateTrackGroups(trackNodes, splitRegions);

            return _trackBuilders;
        }

        private void CreateTrackGroups(IList<SubTrackNode> trackNodes, IList<SplittedRegion> splitRegions)
        {
            SubTrackBuilder lastStBuilder = null;
           
            for (int i = 0; i < trackNodes.Count; i += 1)
            {
                SubTrackNode node = trackNodes[i];
                SubTrackBuilder stBuilder = NextTrackBuilder(node.Position, splitRegions);
                SplittedRegion bounds = stBuilder.SplitRegion;

                if ((lastStBuilder != stBuilder) && (lastStBuilder != null))
                {
                    stBuilder.NextGroup();
                    SubTrackNode previousNode = trackNodes[i - 1];
                    Vector3 actualPrevPos = previousNode.Position + GetOffset(node.Forward);
                    Vector3 actualIntersectPos = bounds.ToAxisAlignedBounds().ClosestPoint(actualPrevPos);
                    Vector3 intersectPos = actualIntersectPos - new Vector3(0.5f, 0.5f, 0.5f) + node.Forward / 2f;

                    if ((intersectPos != previousNode.Position) || (intersectPos != node.Position))
                    {
                        bool intersectsPrev = intersectPos == previousNode.Position;
                        bool intersectsNext = intersectPos == node.Position;

                        if (intersectsPrev && !intersectsNext)
                        {
                            stBuilder.Add(previousNode);
                            UpdateLinks(previousNode, node);
                        }
                        else if (intersectsNext && !intersectsPrev)
                        {
                            lastStBuilder.Add(node);
                            UpdateLinks(previousNode, node);
                        }
                        else
                        {
                            SubTrackNode intersectNode = new SubTrackNode(intersectPos, previousNode.Forward, previousNode.Down);
                            UpdateLinks(intersectNode, node);
                            UpdateLinks(previousNode, intersectNode);
                            lastStBuilder.Add(intersectNode);
                            stBuilder.Add(intersectNode);
                        }
                    }
                }

                stBuilder.Add(node);
                lastStBuilder = stBuilder;
            }
        }

        private Vector3 GetOffset(Vector3 forward)
        {
            return (new Vector3(0.5f, 0.5f, 0.5f) - forward / 2f);
        }

        private void UpdateLinks(SubTrackNode previous, SubTrackNode next)
        {
            if (previous != null)
                previous.Next = next;
            if (next != null)
                next.Previous = previous;
        }

        private SubTrackBuilder NextTrackBuilder(Vector3 position, IList<SplittedRegion> splitRegions)
        {
            SubTrackBuilder stBuilder = _trackBuilders.FirstOrDefault(builder => builder.SplitRegion.Bounds.Contains(position));
            if (stBuilder == null)
            {
                SplittedRegion containingRegion = splitRegions.First(bounds => bounds.Contains(position));
                stBuilder = new SubTrackBuilder(_splineGenerator, containingRegion);
                _trackBuilders.Add(stBuilder);
            }

            return stBuilder;
        }


        public class SubTrackBuilder
        {
            public readonly SplittedRegion SplitRegion;
            public readonly List<List<SubTrackNode>> Groups = new List<List<SubTrackNode>>();
            private readonly TrackSplineGenerator _splineGenerator;
            private List<SubTrackNode> _currentGroup = null;
            private int subTrackGroupIdCount = 0;

            public SubTrackBuilder (TrackSplineGenerator splineGenerator, SplittedRegion splitRegion)
            {
                _splineGenerator = splineGenerator;
                SplitRegion = splitRegion;
                NextGroup();
            }

            public void NextGroup()
            {
                _currentGroup = new List<SubTrackNode>();
                Groups.Add(_currentGroup);
            }

            public void Add(SubTrackNode node)
            {
                _currentGroup.Add(node);   
            }

            public SubTrackGroup[] BuildSubTrackGroups()
            {
                return Groups
                    .Where(nodes => nodes.Count >= 2)
                    .Select(nodes => BuildSubTrackGroup(nodes))
                    .ToArray();
            }

            private SubTrackGroup BuildSubTrackGroup(List<SubTrackNode> trackNodes)
            {
                BezierSpline spline = _splineGenerator.GenerateSpline(trackNodes);
                return new SubTrackGroup(SubTrackGroup.CreateId(subTrackGroupIdCount += 1), spline, trackNodes);
            }
        }
    }
}
