using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Xml;

namespace IaS.WorldBuilder.Tracks
{
    public class SplitTrackBuilder
    {
        public readonly TrackXML TrackXml;
        public SubTrackBuilder FirstSubTrack { get; private set; }
        public SubTrackBuilder LastSubTrack { get; private set; }
        public readonly List<SubTrackBuilder> SubTracks = new List<SubTrackBuilder>();

        public SplitTrackBuilder(TrackXML trackXml)
        {
            TrackXml = trackXml;
        }

        public SplitTrackBuilder WithSubTrack(SubTrackBuilder subTrack)
        {
            SubTracks.Add(subTrack);
            return this;
        }

        public SplitTrackBuilder WithFirstSubTrack(SubTrackBuilder subTrack)
        {
            FirstSubTrack = subTrack;
            return this;
        }

        public SplitTrackBuilder WithLastSubTrack(SubTrackBuilder subTrack)
        {
            LastSubTrack = subTrack;
            return this;
        }

        public SplitTrack Build()
        {
            SubTrack[] subTracks = SubTracks.Select(st => st.Build()).ToArray();
            SubTrack firstSubTrack = subTracks[SubTracks.IndexOf(FirstSubTrack)];
            SubTrack lastSubTrack = subTracks[SubTracks.IndexOf(LastSubTrack)];
            return new SplitTrack(TrackXml, subTracks, firstSubTrack, lastSubTrack);
        }
    }

    public class SubTrackBuilder
    {
        public readonly BlockBounds SplitBounds;
        public SubTrackGroupBuilder FirstGroup { get; private set; }
        public SubTrackGroupBuilder LastGroup { get; private set; }
        public readonly List<SubTrackGroupBuilder> TrackGroups = new List<SubTrackGroupBuilder>();

        public SubTrackBuilder(BlockBounds splitBounds)
        {
            SplitBounds = splitBounds;
        }

        public SubTrackBuilder WithTrackGroup(SubTrackGroupBuilder group)
        {
            TrackGroups.Add(group);
            return this;
        }

        public SubTrackBuilder WithFirstGroup(SubTrackGroupBuilder first)
        {
            FirstGroup = first;
            return this;
        }

        public SubTrackBuilder WithLastGroup(SubTrackGroupBuilder last)
        {
            LastGroup = last;
            return this;
        }

        public void WithTrackGroupAutoFirstLast()
        {
            FirstGroup = TrackGroups[0];
            LastGroup = TrackGroups.Last();
        }

        public SubTrack Build()
        {
            WithTrackGroupAutoFirstLast();
            SubTrackGroup[] subTrackGroups = TrackGroups.Select(tg => tg.Build()).ToArray();
            SubTrackGroup firstGroup = subTrackGroups[TrackGroups.IndexOf(FirstGroup)];
            SubTrackGroup lastGroup = subTrackGroups[TrackGroups.IndexOf(LastGroup)];
            return new SubTrack(SplitBounds, subTrackGroups, firstGroup, lastGroup);
        }
        
    }

    public class SubTrackGroupBuilder
    {
        public List<SubTrackNode> SubTrackNodes = new List<SubTrackNode>();

        public SubTrackGroupBuilder WithNodes(ICollection<SubTrackNode> nodes)
        {
            SubTrackNodes.AddRange(nodes);
            return this;
        }

        public SubTrackGroup Build()
        {
            TrackSplineGenerator splineGenerator = new TrackSplineGenerator(TrackBuilderConfiguration.DefaultConfig());
            BezierSpline spline = splineGenerator.GenerateSpline(SubTrackNodes);
            return new SubTrackGroup(spline, SubTrackNodes);
        }
    }
}
