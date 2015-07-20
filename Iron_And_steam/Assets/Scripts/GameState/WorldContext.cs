using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.WorldBuilder.Tracks;

namespace IaS.GameState
{
    public class WorldContext
    {
        public List<TrackContext> tracks { get; private set; }
        public EventRegistry eventRegistry { get; private set; }

        public WorldContext()
        {
            this.tracks = new List<TrackContext>();
            this.eventRegistry = new EventRegistry();
        }

        public TrackContext AddTrackContext(SplitTrack splitTrack)
        {
            TrackContext trackContext = new TrackContext(this, splitTrack);
            tracks.Add(trackContext);
            return trackContext;
        }
    }
}
