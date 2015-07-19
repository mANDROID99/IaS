using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder;

namespace IASTest
{
    [TestFixture]
    [Category("Track Splitting")]
    class TrackSubTrackSplitterTest
    {
        private TrackSubTrackSplitter splitter;

        [SetUp]
        public void Init()
        {
            splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
        }

        [Test]
        public void twoXAlignedNodes_splitYBetween_twoCorrectSubTracks()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 4, 0))
            });

            Split[] splits = {new Split("split_1", Vector3.up, new Vector3(), 2, new SubSplit[0])};
            IList<SubTrack> subTracks = splitter.SplitTrack(track, splits).subTracks;

            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 2, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[]{new Vector3(0, 2, 0), new Vector3(0, 4, 0)});
        }

        [Test]
        public void twoYAlignedNodes_splitXBetween_twoCorrectSubTracks()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(4, 0, 0))
            });

            Split[] splits = { new Split("split_1", Vector3.right, new Vector3(), 2, new SubSplit[0]) };
            IList<SubTrack> subTracks = splitter.SplitTrack(track, splits).subTracks;

            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(2, 0, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[] { new Vector3(2, 0, 0), new Vector3(4, 0, 0) });
        }

        [Test]
        public void twoXAlignedNodes_splitYAboveBoth_oneSubtrackContainingBothNodes()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 4, 0))
            });
            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[] { new Split("split_id", Vector3.up, new Vector3(), 5, new SubSplit[0]) }).subTracks;

            Assert.That(subTracks.Count, Is.EqualTo(1));
            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 4, 0) });
        }

        [Test]
        public void oneTrackCurve_splitTwice_threeCorrectSubtracks()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 3, 0)),
                new TrackNode(new Vector3(3, 3, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[] { 
                new Split("split_id", Vector3.up, new Vector3(), 2, new SubSplit[0]), 
                new Split("split_id", Vector3.right, new Vector3(), 2, new SubSplit[0]) }).subTracks;

            Assert.That(subTracks.Count, Is.EqualTo(3));
            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 2, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[] { new Vector3(0, 2, 0), new Vector3(0, 3, 0), new Vector3(0, 3, 0), new Vector3(2, 3, 0) });
            AssertSubtrackContainsNodes(subTracks[2], 0, new Vector3[] { new Vector3(2, 3, 0), new Vector3(3, 3, 0) });
        }

        [Test]
        public void trackDoublesBack_createsTwoSubTracks()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(2, 0, 0)),
                new TrackNode(new Vector3(2, 1, 0)),
                new TrackNode(new Vector3(0, 1, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[]{
                new Split("split_id", Vector3.right, new Vector3(), 1, new SubSplit[0])
            }).subTracks;

            Assert.That(subTracks.Count, Is.EqualTo(2));
            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[] { new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(2, 0, 0), new Vector3(2, 1, 0), new Vector3(2, 1, 0), new Vector3(1, 1, 0) });
            AssertSubtrackContainsNodes(subTracks[0], 1, new Vector3[] { new Vector3(1, 1, 0), new Vector3(0, 1, 0) });
        }

        [Test]
        public void trackDownwardsSplitBelowPrevious_doesNotAddTrackNodeTwice()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 3, 0)),
                new TrackNode(new Vector3(0, 0, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[]{
                new Split("split_id", Vector3.up, new Vector3(), 3, new SubSplit[0])
            }).subTracks;

            Assert.That(subTracks.Count, Is.EqualTo(2));
            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 3, 0), new Vector3(0, 0, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[] { new Vector3(0, 3, 0) });
        }

        [Test]
        public void firstNodeAtSplit_doesNotAddIntersectNode()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 2, 0)),
                new TrackNode(new Vector3(0, 0, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[]{
                new Split("", Vector3.up, new Vector3(), 2, new SubSplit[0])
            }).subTracks;

            Assert.That(SecondPart(subTracks).NumTrackNodes, Is.EqualTo(1));
            Assert.That(FirstPart(subTracks).NumTrackNodes, Is.EqualTo(2));
            Assert.That(SecondPart(subTracks)[0].next, Is.EqualTo(FirstPart(subTracks)[1]));
            Assert.That(FirstPart(subTracks)[1].previous, Is.EqualTo(SecondPart(subTracks)[0]));
            Assert.That(FirstPart(subTracks)[0], Is.EqualTo(SecondPart(subTracks)[0]));
        }

        [Test]
        public void secondNodeAtSplit_doesNotAddIntersectNode()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 2, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[]{
                new Split("", Vector3.up, new Vector3(), 2, new SubSplit[0])
            }).subTracks;

            Assert.That(FirstPart(subTracks).NumTrackNodes, Is.EqualTo(2));
            Assert.That(SecondPart(subTracks).NumTrackNodes, Is.EqualTo(1));
            Assert.That(FirstPart(subTracks)[0].next, Is.EqualTo(SecondPart(subTracks)[0]));
            Assert.That(FirstPart(subTracks)[1], Is.EqualTo(SecondPart(subTracks)[0]));
            Assert.That(SecondPart(subTracks)[0].previous, Is.EqualTo(FirstPart(subTracks)[0]));
        }

        [Test]
        public void splitBetweenBothNodes_addsOneIntersectNodeBetweenBoth()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 4, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[]{
                new Split("", Vector3.up, new Vector3(), 2, new SubSplit[0])
            }).subTracks;

            Assert.That(FirstPart(subTracks).NumTrackNodes, Is.EqualTo(2));
            Assert.That(SecondPart(subTracks).NumTrackNodes, Is.EqualTo(2));

            Assert.That(FirstPart(subTracks)[1], Is.EqualTo(SecondPart(subTracks)[0])); // assert that both intersecting nodes are the same
            Assert.That(FirstPart(subTracks)[0].next, Is.EqualTo(FirstPart(subTracks)[1])); // assert that first node leads to intersecting node

            Assert.That(FirstPart(subTracks)[1].next, Is.EqualTo(SecondPart(subTracks)[1])); // assert that intersecting node leads to last node
            Assert.That(FirstPart(subTracks)[1].previous, Is.EqualTo(FirstPart(subTracks)[0])); // assert that intersecting node leads to previous node

            Assert.That(SecondPart(subTracks)[1].previous, Is.EqualTo(SecondPart(subTracks)[0])); // assert that last node leads to intersecting node
        }

        [Test]
        public void trackCurvesRight_nextAndPreviousSetCorrectlyForCurveNodes()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 4, 0)),
                new TrackNode(new Vector3(4, 4, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[0]).subTracks;
            Assert.That(FirstPart(subTracks)[0].next, Is.EqualTo(FirstPart(subTracks)[1]));
            Assert.That(FirstPart(subTracks)[1].next, Is.EqualTo(FirstPart(subTracks)[2]));
            Assert.That(FirstPart(subTracks)[2].next, Is.EqualTo(FirstPart(subTracks)[3]));

            Assert.That(FirstPart(subTracks)[1].previous, Is.EqualTo(FirstPart(subTracks)[0]));
            Assert.That(FirstPart(subTracks)[2].previous, Is.EqualTo(FirstPart(subTracks)[1]));
            Assert.That(FirstPart(subTracks)[3].previous, Is.EqualTo(FirstPart(subTracks)[2]));
        }

        [Test]
        public void TrackCurvesRight_forwardAndBackValuesAreSetCorrectly()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 4, 0)),
                new TrackNode(new Vector3(4, 4, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(track, new Split[]{ new Split("", Vector3.up, new Vector3(), 3, new SubSplit[0])}).subTracks;

            Assert.That(FirstPart(subTracks)[0].forward, Is.EqualTo(Vector3.up));
            Assert.That(FirstPart(subTracks)[1].forward, Is.EqualTo(Vector3.up));
            Assert.That(SecondPart(subTracks)[0].forward, Is.EqualTo(Vector3.up));
            Assert.That(SecondPart(subTracks)[1].forward, Is.EqualTo(Vector3.up));
            Assert.That(SecondPart(subTracks)[2].forward, Is.EqualTo(Vector3.right));
            Assert.That(SecondPart(subTracks)[3].forward, Is.EqualTo(Vector3.right));
        }


        private void AssertSubtrackContainsNodes(SubTrack subtrack, int groupIdx, Vector3[] nodePositions, bool allowSubset = false){
            if(!allowSubset){
                Assert.That(subtrack.trackGroups[groupIdx].NumTrackNodes, Is.EqualTo(nodePositions.Length));
            }
            Assert.That(nodePositions, Is.SubsetOf(subtrack.trackGroups[groupIdx].nodes.Select(t => t.position)));
        }

        private void AssertSubTrackNode(int group, int i, SubTrackNode node, Vector3 position, Vector3 forward, Vector3 down)
        {
            Assert.That(node.position, Is.EqualTo(position), String.Format("node at group {0} index {1}, expected pos: {2}, got: {3}", group, i, position, node.position));
            Assert.That(node.forward, Is.EqualTo(forward), String.Format("node at group {0} index {1}, expected forward: {2}, got: {3}", group, i, forward, node.forward));
            Assert.That(Vector3.Angle(node.down, down), Is.LessThan(0.1f), String.Format("node at group {0} index {1}, expected down: {2}, got: {3}", group, i, down, node.down));
        }

        private SubTrackGroup FirstPart(IList<SubTrack> subTrack, int group=0)
        {
            return subTrack[0].trackGroups[group];
        }

        private SubTrackGroup SecondPart(IList<SubTrack> subTrack, int group = 0)
        {
            return subTrack[1].trackGroups[group];
        }

    }
}
