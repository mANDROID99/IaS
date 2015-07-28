using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using NUnit.Framework;
using UnityEngine;

namespace IASTest
{
    [TestFixture]
    [Category("TrackDTO JunctionsDto")]
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

    }
}
