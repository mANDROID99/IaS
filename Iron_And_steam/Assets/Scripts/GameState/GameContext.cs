using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.WorldBuilder.Tracks;

namespace IaS.GameState
{
    public class GameContext
    {
        public List<TrackContext> tracks { get; private set; }

        public GameContext()
        {
            this.tracks = new List<TrackContext>();
        }



        public TrackContext AddTrackContext(SplitTrack splitTrack)
        {
            TrackContext trackContext = new TrackContext(splitTrack);
            tracks.Add(trackContext);
            return trackContext;
        }
    }
}
