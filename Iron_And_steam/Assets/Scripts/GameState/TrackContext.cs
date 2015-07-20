using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.WorldBuilder.Tracks;

namespace IaS.GameState
{
    public class TrackContext
    {
        public SplitTrack splitTrack { get; private set; }
        public TrackConnections connections { get; private set; }

        internal TrackContext(WorldContext worldContext, SplitTrack splitTrack)
        {
            this.splitTrack = splitTrack;
            this.connections = new TrackConnections(worldContext, splitTrack);
        }
    }
}
