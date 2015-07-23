using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;

namespace IaS.GameState
{
    class TrackContextCreator
    {

        public TrackContext CreateTrackContext(EventRegistry eventRegistry, Track track, Split[] splits)
        {
            var splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
             var splineGenerator = new TrackSplineGenerator(TrackBuilderConfiguration.DefaultConfig());

            SplitTrack splitTrack = splitter.SplitTrack(track, splits);
            foreach (SubTrack subTrack in splitTrack.SubTracks)
            {
                splineGenerator.GenerateSplines(splitTrack, subTrack);
            }

            var trackConnections = new TrackConnections(eventRegistry, splitTrack);
            return new TrackContext(trackConnections, splitTrack);
        }

    }
}
