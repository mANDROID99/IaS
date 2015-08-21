using NUnit.Framework;

namespace IASTest
{
    [TestFixture]
    [Category("TrackXML Spline Tests")]
    class TestTrackSpline
    {
        /*
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
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 3, 0))
            });
            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);

            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 3, 0.5f));
        }

        [Test]
        public void twoXAlignedNodes_producesXLineBetween()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(3, 0, 0))
            });

            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.5f, 0.5f), new Vector3(3, 0.5f, 0.5f));
        }

        [Test]
        public void twoZAlignedNodes_producesZLineBetween()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 0, 3))
            });

            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 3));
        }

        [Test]
        public void lineUpThenCurveRight_producesCorrectSpline()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 3, 0)),
                new TrackNodeXML(null, new Vector3(3, 3, 0))
            });

            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 3, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.5f, 3, 0.5f), new Vector3(1, 3.5f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 3.5f, 0.5f), new Vector3(3, 3.5f, 0.5f));
        }

        [Test]
        public void lineRightThenCurveBack_producesCorrectSpline()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(3, 0, 0)),
                new TrackNodeXML(null, new Vector3(3, 0, 3))
            });

            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.5f, 0.5f), new Vector3(3, 0.5f, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(3, 0.5f, 0.5f), new Vector3(3.5f, 0.5f, 1));
            AssertSplinePt(spline.pts[2], new Vector3(3.5f, 0.5f, 1), new Vector3(3.5f, 0.5f, 3));
        }

        [Test]
        public void twoConsecutiveCurves_producesCorrectSpline()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 2, 0)),
                new TrackNodeXML(null, new Vector3(1, 2, 0)),
                new TrackNodeXML(null, new Vector3(1, 3, 0))
            });

            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 2, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.5f, 2, 0.5f), new Vector3(1, 2.5f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 2.5f, 0.5f), new Vector3(1.5f, 3f, 0.5f));
        }

        [Test]
        public void withOffset_and_twoYAlignedNodesFacingForward_movesTrackForward()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.forward, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 3, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            AssertSplinePt(spline.pts[0], new Vector3(0.5f, 0, 0.75f), new Vector3(0.5f, 3, 0.75f));
        }

       [Test]
        public void withOffset_and_twoXAlignedNodesFacingUpward_movesTrackUpward()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.up, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(3, 0, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(1));
            AssertSplinePt(spline.pts[0], new Vector3(0, 0.75f, 0.5f), new Vector3(3, 0.75f, 0.5f));
        }

        [Test]
        public void withOffset_and_singleCurveRightFacingLeft_movesTrackCorrectly()
        {
            TrackXML trackXml = new TrackXML("id", Vector3.left, null, new []{
                new TrackNodeXML(null, new Vector3(0, 0, 0)),
                new TrackNodeXML(null, new Vector3(0, 2, 0)),
                new TrackNodeXML(null, new Vector3(2, 2, 0))
            });

            config.curveOffset = 0.25f;
            BezierSpline spline = GetSplineNoSplits(trackXml, 0, 0);
            Assert.That(spline.pts.Length, Is.EqualTo(3));
            AssertSplinePt(spline.pts[0], new Vector3(0.25f, 0, 0.5f), new Vector3(0.25f, 2, 0.5f));
            AssertSplinePt(spline.pts[1], new Vector3(0.25f, 2, 0.5f), new Vector3(1, 2.75f, 0.5f));
            AssertSplinePt(spline.pts[2], new Vector3(1, 2.75f, 0.5f), new Vector3(2, 2.75f, 0.5f));
        }

        private BezierSpline GetSplineNoSplits(TrackXML trackXml, int subTrackIdx, int groupIdx)
        {
            SplitTrack splitTrack = splitter.SplitTrack(trackXml, new BlockBounds[0]);
            return splitTrack.SubTracks[subTrackIdx].TrackGroups[groupIdx].Spline;
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
        */
    }
}
