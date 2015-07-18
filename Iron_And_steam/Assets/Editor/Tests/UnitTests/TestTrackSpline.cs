using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using IaS.WorldBuilder.Meshes.Tracks;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder;

namespace IASTest
{
    [TestFixture]
    [Category("Track Spline Tests")]
    class TestTrackSpline
    {

        private TrackBuilderConfiguration config;
        private TrackSubTrackSplitter splitter;
        private TrackSplineGenerator splineGenerator;

        [SetUp]
        public void Init()
        {
            config = TrackBuilderConfiguration.DefaultConfigWithCurveOffset(0);
            splitter = new TrackSubTrackSplitter(config);
            splineGenerator = new TrackSplineGenerator(config);
        }

        [Test]
        public void twoYAlignedNodes_producesYLineBetween()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 3, 0))
            });
            BezierSpline spline = GetSplineNoSplits(track, 0, 0);

            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 3, 0.5f));
        }

        [Test]
        public void twoXAlignedNodes_producesXLineBetween()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(3, 0, 0))
            });

            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.5f, 0.5f), new Vector3(3, 0.5f, 0.5f));
        }

        [Test]
        public void twoZAlignedNodes_producesZLineBetween()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 0, 3))
            });

            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 3));
        }

        [Test]
        public void lineUpThenCurveRight_producesCorrectSpline()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 3, 0)),
                new TrackNode(new Vector3(3, 3, 0))
            });

            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 3, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.5f, 3, 0.5f), new Vector3(1, 3.5f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 3.5f, 0.5f), new Vector3(3, 3.5f, 0.5f));
        }

        [Test]
        public void lineRightThenCurveBack_producesCorrectSpline()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(3, 0, 0)),
                new TrackNode(new Vector3(3, 0, 3))
            });

            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.5f, 0.5f), new Vector3(3, 0.5f, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(3, 0.5f, 0.5f), new Vector3(3.5f, 0.5f, 1));
            AssertSplinePt(spline.pts[2], new Vector3(3.5f, 0.5f, 1), new Vector3(3.5f, 0.5f, 3));
        }

        [Test]
        public void twoConsecutiveCurves_producesCorrectSpline()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 2, 0)),
                new TrackNode(new Vector3(1, 2, 0)),
                new TrackNode(new Vector3(1, 3, 0))
            });

            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 2, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.5f, 2, 0.5f), new Vector3(1, 2.5f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 2.5f, 0.5f), new Vector3(1.5f, 3f, 0.5f));
        }

        [Test]
        public void withOffset_and_twoYAlignedNodesFacingForward_movesTrackForward()
        {
            Track track = new Track("id", Vector3.forward, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 3, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.75f), new Vector3(0.5f, 3, 0.75f));
        }

       [Test]
        public void withOffset_and_twoXAlignedNodesFacingUpward_movesTrackUpward()
        {
            Track track = new Track("id", Vector3.up, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(3, 0, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.75f, 0.5f), new Vector3(3, 0.75f, 0.5f));
        }

        [Test]
        public void withOffset_and_singleCurveRightFacingLeft_movesTrackCorrectly()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 2, 0)),
                new TrackNode(new Vector3(2, 2, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(track, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.25f, 0, 0.5f), new Vector3(0.25f, 2, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.25f, 2, 0.5f), new Vector3(1, 2.75f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 2.75f, 0.5f), new Vector3(2, 2.75f, 0.5f));
        }

        
        [Test]
        public void curveAdjacentToSplit_doesNotAddCurveEndPosTwice()
        {
            Track track = new Track("id", Vector3.left, new TrackNode[]{
                new TrackNode(new Vector3(0, 0, 0)),
                new TrackNode(new Vector3(0, 1, 0)),
                new TrackNode(new Vector3(3, 1, 0))
            });

            BezierSpline[][] splines = GetSplinesWithSplits(track, new Split[]{
                new Split("split_id", Vector3.right, new Vector3(), 1, new SubSplit[0]),
            });

            Assert.That(splines.Length, Is.EqualTo(2));
            Assert.That(splines[0][0].pts.Length, Is.EqualTo(2));
            AssertSplinePt(splines[0][0].pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 1, 0.5f));
            AssertSplinePt(splines[0][0].pts[1], new Vector3(0.5f, 1, 0.5f), new Vector3(1, 1.5f, 0.5f));

            Assert.That(splines[1][0].pts.Length, Is.EqualTo(1));
            AssertSplinePt(splines[1][0].pts[0], new Vector3(1, 1.5f, 0.5f), new Vector3(3, 1.5f, 0.5f));
        }

        private BezierSpline[][] GetSplinesWithSplits(Track track, Split[] splits)
        {
            SplitTrack splitTrack = splitter.SplitTrack(track, splits);
            return splitTrack.subTracks.Select(subTrack => splineGenerator.GenerateSplines(splitTrack, subTrack)).ToArray();
        }


        private BezierSpline GetSplineNoSplits(Track track, int subTrackIdx, int groupIdx)
        {
            SplitTrack splitTrack = splitter.SplitTrack(track, new Split[0]);
            return splineGenerator.GenerateSplines(splitTrack, splitTrack.subTracks[subTrackIdx])[groupIdx];
        }

        private void AssertSplinePt(BezierSpline.BezierPoint pt, Vector3 expectedStartPos, Vector3 expectedEndPos, Vector3? expectedAnchor1 = null, Vector3? expectedAnchor2 = null)
        {
            Assert.That(pt.startPos, Is.EqualTo(expectedStartPos));
            Assert.That(pt.endPos, Is.EqualTo(expectedEndPos));
            if (expectedAnchor1 != null)
            {
                Assert.That(pt.anchor1, Is.EqualTo(expectedAnchor1));
            }
            if (expectedAnchor2 != null)
            {
                Assert.That(pt.anchor2, Is.EqualTo(expectedAnchor2));
            }
        }

    }
}
