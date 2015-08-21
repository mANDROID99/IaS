using System;
using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using NUnit.Framework;
using UnityEngine;
using IaS.Domain.Tracks;

namespace IASTest
{
    [TestFixture]
    [Category("Track Splitting")]
    class TrackSubTrackSplitterTest
    {
        /*
        private TrackSubTrackSplitter splitter;

        private static Split CreateSplit(Vector3 axis, float value)
        {
            return new Split("id", "group_id", null, axis, new Vector3(), value, Split.RestrictionType.Both);
        }

        [SetUp]
        public void Init()
        {
            splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
        }

        private BlockBounds[] splitToBlockBounds(Split[] splits)
        {
            SplitTree tree = new SplitTree(BlockBounds.Unbounded);
            tree.Split(splits);
            return tree.GatherSplitBounds();
        }

        [Test]
        public void twoXAlignedNodes_splitYBetween_twoCorrectSubTracks()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 4, 0))
            });

            Split[] splits = { CreateSplit(Vector3.up, 2)};
            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(splits)).SubTracks;

            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 2, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[]{new Vector3(0, 2, 0), new Vector3(0, 4, 0)});
        }

        [Test]
        public void twoYAlignedNodes_splitXBetween_twoCorrectSubTracks()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(4, 0, 0))
            });

            Split[] splits = { CreateSplit(Vector3.right, 2) };
            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(splits)).SubTracks;

            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(2, 0, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[] { new Vector3(2, 0, 0), new Vector3(4, 0, 0) });
        }

        [Test]
        public void twoXAlignedNodes_splitYAboveBoth_oneSubtrackContainingBothNodes()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 4, 0))
            });
            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(new[] { CreateSplit(Vector3.up, 5) })).SubTracks;

            Assert.That(subTracks.Count, Is.EqualTo(1));
            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 4, 0) });
        }

        [Test]
        public void oneTrackCurve_splitTwice_threeCorrectSubtracks()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 3, 0)),
                new TrackNodeXML(null, new Vector3(3, 3, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(new Split[] {
                CreateSplit(Vector3.up, 2),
                CreateSplit(Vector3.right, 2) })).SubTracks;

            Assert.That(subTracks.Count, Is.EqualTo(3));
            AssertSubtrackContainsNodes(subTracks[0], 0, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 2, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new Vector3[] { new Vector3(0, 2, 0), new Vector3(0, 3, 0), new Vector3(0, 3, 0), new Vector3(2, 3, 0) });
            AssertSubtrackContainsNodes(subTracks[2], 0, new Vector3[] { new Vector3(2, 3, 0), new Vector3(3, 3, 0) });
        }

        [Test]
        public void trackDoublesBack_createsTwoSubTracks()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(4, 0, 0)),
                new TrackNodeXML(null, new Vector3(4, 1, 0)),
                new TrackNodeXML(null, new Vector3(0, 1, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(new Split[]{
                CreateSplit(Vector3.right, 2)
            })).SubTracks;

            Assert.That(subTracks.Count, Is.EqualTo(2));
            AssertSubtrackContainsNodes(subTracks[0], 0, new [] { new Vector3(0, 0, 0), new Vector3(2, 0, 0) });
            AssertSubtrackContainsNodes(subTracks[1], 0, new [] { new Vector3(2, 0, 0), new Vector3(4, 0, 0), new Vector3(4, 0, 0), new Vector3(4, 1, 0), new Vector3(4, 1, 0), new Vector3(1, 1, 0) });
            AssertSubtrackContainsNodes(subTracks[0], 1, new [] { new Vector3(1, 1, 0), new Vector3(0, 1, 0) });
        }

        [Test]
        public void splitBetweenBothNodes_addsOneIntersectNodeBetweenBoth()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.left, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 4, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(new Split[]{
                CreateSplit(Vector3.up, 2)
            })).SubTracks;

            Assert.That(FirstPart(subTracks).NumTrackNodes, Is.EqualTo(2));
            Assert.That(SecondPart(subTracks).NumTrackNodes, Is.EqualTo(2));

            Assert.That(FirstPart(subTracks).Nodes[1], Is.EqualTo(SecondPart(subTracks).Nodes[0])); // assert that both intersecting NodesXml are the same
            Assert.That(FirstPart(subTracks).Nodes[0].Next, Is.EqualTo(FirstPart(subTracks).Nodes[1])); // assert that first node leads to intersecting node

            Assert.That(FirstPart(subTracks).Nodes[1].Next, Is.EqualTo(SecondPart(subTracks).Nodes[1])); // assert that intersecting node leads to last node
            Assert.That(FirstPart(subTracks).Nodes[1].Previous, Is.EqualTo(FirstPart(subTracks).Nodes[0])); // assert that intersecting node leads to previous node

            Assert.That(SecondPart(subTracks).Nodes[1].Previous, Is.EqualTo(SecondPart(subTracks).Nodes[0])); // assert that last node leads to intersecting node
        }

        [Test]
        public void trackCurvesRight_nextAndPreviousSetCorrectlyForCurveNodes()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.left, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 4, 0)),
                new TrackNodeXML(null, new Vector3(4, 4, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, new BlockBounds[0]).SubTracks;
            Assert.That(FirstPart(subTracks).Nodes[0].Next, Is.EqualTo(FirstPart(subTracks).Nodes[1]));
            Assert.That(FirstPart(subTracks).Nodes[1].Next, Is.EqualTo(FirstPart(subTracks).Nodes[2]));
            Assert.That(FirstPart(subTracks).Nodes[2].Next, Is.EqualTo(FirstPart(subTracks).Nodes[3]));

            Assert.That(FirstPart(subTracks).Nodes[1].Previous, Is.EqualTo(FirstPart(subTracks).Nodes[0]));
            Assert.That(FirstPart(subTracks).Nodes[2].Previous, Is.EqualTo(FirstPart(subTracks).Nodes[1]));
            Assert.That(FirstPart(subTracks).Nodes[3].Previous, Is.EqualTo(FirstPart(subTracks).Nodes[2]));
        }

        [Test]
        public void TrackCurvesRight_forwardAndBackValuesAreSetCorrectly()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.left, null, new TrackNodeXML[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 4, 0)),
                new TrackNodeXML(null, new Vector3(4, 4, 0))
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(new Split[]{ CreateSplit(Vector3.up, 3) })).SubTracks;

            Assert.That(FirstPart(subTracks).Nodes[0].Forward, Is.EqualTo(Vector3.up));
            Assert.That(FirstPart(subTracks).Nodes[1].Forward, Is.EqualTo(Vector3.up));
            Assert.That(SecondPart(subTracks).Nodes[0].Forward, Is.EqualTo(Vector3.up));
            Assert.That(SecondPart(subTracks).Nodes[1].Forward, Is.EqualTo(Vector3.up));
            Assert.That(SecondPart(subTracks).Nodes[2].Forward, Is.EqualTo(Vector3.right));
            Assert.That(SecondPart(subTracks).Nodes[3].Forward, Is.EqualTo(Vector3.right));
        }

        [Test]
        public void ImmediateCurveToRight()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, Vector3.up, new[]{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(3, 0, 0)), 
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, new BlockBounds[0]).SubTracks;

            Assert.That(FirstPart(subTracks).Nodes[0].Position, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(FirstPart(subTracks).Nodes[0].Forward, Is.EqualTo(Vector3.up));

            Assert.That(FirstPart(subTracks).Nodes[1].Position, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(FirstPart(subTracks).Nodes[1].Forward, Is.EqualTo(Vector3.right));

            Assert.That(FirstPart(subTracks).Nodes[2].Position, Is.EqualTo(new Vector3(3, 0, 0)));
            Assert.That(FirstPart(subTracks).Nodes[2].Forward, Is.EqualTo(Vector3.right));
        }

        [Test]
        public void SplitsAtSamePointAsLastNode()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, Vector3.up, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 3, 0)),
            });

            IList<SubTrack> subTracks = splitter.SplitTrack(trackXml, splitToBlockBounds(new []{
               CreateSplit(Vector3.up, 3)
            })).SubTracks;

            Assert.That(subTracks.Count, Is.EqualTo(1));
        }


        private void AssertSubtrackContainsNodes(SubTrack subtrack, int groupIdx, Vector3[] nodePositions, bool allowSubset = false){
            if(!allowSubset){
                Assert.That(subtrack.TrackGroups[groupIdx].NumTrackNodes, Is.EqualTo(nodePositions.Length));
            }
            Assert.That(nodePositions, Is.SubsetOf(subtrack.TrackGroups[groupIdx].Nodes.Select(t => t.Position)));
        }

        private void AssertSubTrackNode(int group, int i, SubTrackNode node, Vector3 position, Vector3 forward, Vector3 down)
        {
            Assert.That(node.Position, Is.EqualTo(position), String.Format("node at GroupXml {0} index {1}, expected pos: {2}, got: {3}", group, i, position, node.Position));
            Assert.That(node.Forward, Is.EqualTo(forward), String.Format("node at GroupXml {0} index {1}, expected forward: {2}, got: {3}", group, i, forward, node.Forward));
            Assert.That(Vector3.Angle(node.Down, down), Is.LessThan(0.1f), String.Format("node at GroupXml {0} index {1}, expected down: {2}, got: {3}", group, i, down, node.Down));
        }

        private SubTrackGroup FirstPart(IList<SubTrack> subTrack, int group=0)
        {
            return subTrack[0].TrackGroups[group];
        }

        private SubTrackGroup SecondPart(IList<SubTrack> subTrack, int group = 0)
        {
            return subTrack[1].TrackGroups[group];
        }
        */
    }
}
