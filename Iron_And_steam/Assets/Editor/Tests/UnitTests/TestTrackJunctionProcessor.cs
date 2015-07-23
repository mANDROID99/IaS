using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using NUnit.Framework;
using UnityEngine;

namespace IASTest
{
    [TestFixture]
    [Category("Track Junctions")]
    class TestTrackJunctionProcessor
    {

        private TrackBuilderConfiguration _config;
        private TrackJunctionProcessor _junctionProcessor;

        [SetUp]
        public void Init()
        {
            _config = TrackBuilderConfiguration.DefaultConfigWithCurveOffset(0);
            _junctionProcessor = new TrackJunctionProcessor();
        }
       
        [Test]
        public void track_with_junctions()
        {
            Track track = new Track("track_1", Vector3.forward, new[]
            {
                new TrackNode(null, new Vector3(0, 0, 0)),
                new TrackNode("crossroads", new Vector3(0, 3, 0)),
                new TrackNode(null, new Vector3(3, 3, 0))
            });
            Track track2 = new Track("track_2", Vector3.forward, new[]
            {
                new TrackNode(null, new Vector3(-3, 3, 0)),
            }, "crossroads");

            /*_junctionProcessor.Process(new []
            {
                new SplitTrack(track),
                new SplitTrack(track), 
            });*/
        }

    }
}
