using IaS.WorldBuilder.Tracks;
using NUnit.Framework;

namespace IASTest
{
    [TestFixture]
    [Category("TrackXML JunctionsDto")]
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
