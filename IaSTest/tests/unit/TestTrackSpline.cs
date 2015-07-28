using System.Linq;
using IaS.Domain;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using NUnit.Framework;
using UnityEngine;

namespace IASTest
{
    [TestFixture]
    [Category("TrackDTO Spline Tests")]
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
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 3, 0))
            });
            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);

            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 3, 0.5f));
        }

        [Test]
        public void twoXAlignedNodes_producesXLineBetween()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(3, 0, 0))
            });

            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.5f, 0.5f), new Vector3(3, 0.5f, 0.5f));
        }

        [Test]
        public void twoZAlignedNodes_producesZLineBetween()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 0, 3))
            });

            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 3));
        }

        [Test]
        public void lineUpThenCurveRight_producesCorrectSpline()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 3, 0)),
                new TrackNodeDTO(null, new Vector3(3, 3, 0))
            });

            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 3, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.5f, 3, 0.5f), new Vector3(1, 3.5f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 3.5f, 0.5f), new Vector3(3, 3.5f, 0.5f));
        }

        [Test]
        public void lineRightThenCurveBack_producesCorrectSpline()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(3, 0, 0)),
                new TrackNodeDTO(null, new Vector3(3, 0, 3))
            });

            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.5f, 0.5f), new Vector3(3, 0.5f, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(3, 0.5f, 0.5f), new Vector3(3.5f, 0.5f, 1));
            AssertSplinePt(spline.pts[2], new Vector3(3.5f, 0.5f, 1), new Vector3(3.5f, 0.5f, 3));
        }

        [Test]
        public void twoConsecutiveCurves_producesCorrectSpline()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 2, 0)),
                new TrackNodeDTO(null, new Vector3(1, 2, 0)),
                new TrackNodeDTO(null, new Vector3(1, 3, 0))
            });

            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 2, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.5f, 2, 0.5f), new Vector3(1, 2.5f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 2.5f, 0.5f), new Vector3(1.5f, 3f, 0.5f));
        }

        [Test]
        public void withOffset_and_twoYAlignedNodesFacingForward_movesTrackForward()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.forward, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 3, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.75f), new Vector3(0.5f, 3, 0.75f));
        }

       [Test]
        public void withOffset_and_twoXAlignedNodesFacingUpward_movesTrackUpward()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.up, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(3, 0, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.75f, 0.5f), new Vector3(3, 0.75f, 0.5f));
        }

        [Test]
        public void withOffset_and_singleCurveRightFacingLeft_movesTrackCorrectly()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.left, null, new TrackNodeDTO[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 2, 0)),
                new TrackNodeDTO(null, new Vector3(2, 2, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(trackDto, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.25f, 0, 0.5f), new Vector3(0.25f, 2, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.25f, 2, 0.5f), new Vector3(1, 2.75f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 2.75f, 0.5f), new Vector3(2, 2.75f, 0.5f));
        }

        
        [Test]
        public void curveAdjacentToSplit_doesNotAddCurveEndPosTwice()
        {
            TrackDTO trackDto = new TrackDTO("id", Vector3.left, null, new[]{
                new TrackNodeDTO(null, new Vector3(0, 0, 0)),
                new TrackNodeDTO(null, new Vector3(0, 1, 0)),
                new TrackNodeDTO(null, new Vector3(3, 1, 0))
            });

            BezierSpline[][] splines = GetSplinesWithSplits(trackDto, new[]{
                new Split("split_id", Vector3.right, new Vector3(), 1, new SubSplit[0]),
            });

            Assert.That(splines.Length, Is.EqualTo(2));
            Assert.That(splines[0][0].pts.Length, Is.EqualTo(2));
            AssertSplinePt(splines[0][0].pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 1, 0.5f));
            AssertSplinePt(splines[0][0].pts[1], new Vector3(0.5f, 1, 0.5f), new Vector3(1, 1.5f, 0.5f));

            Assert.That(splines[1][0].pts.Length, Is.EqualTo(1));
            AssertSplinePt(splines[1][0].pts[0], new Vector3(1, 1.5f, 0.5f), new Vector3(3, 1.5f, 0.5f));
        }


        private BezierSpline[][] GetSplinesWithSplits(TrackDTO trackDto, Split[] splits)
        {
            SplitTrack splitTrack = splitter.SplitTrack(trackDto, splits);
            return splitTrack.SubTracks.Select(subTrack => splineGenerator.GenerateSplines(splitTrack, subTrack)).ToArray();
        }


        private BezierSpline GetSplineNoSplits(TrackDTO trackDto, int subTrackIdx, int groupIdx)
        {
            SplitTrack splitTrack = splitter.SplitTrack(trackDto, new Split[0]);
            return splineGenerator.GenerateSplines(splitTrack, splitTrack.SubTracks[subTrackIdx])[groupIdx];
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
