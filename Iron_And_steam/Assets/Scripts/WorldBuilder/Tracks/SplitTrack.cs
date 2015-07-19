using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Xml;
using IaS.GameState;

namespace IaS.WorldBuilder.Tracks
{
    public class SplitTrack
    {
        public SubTrack[] subTracks { get; private set; }
        public Track trackRef { get; private set; }
        public SubTrackNode firstTrackNode { get; private set; }
        public bool areSubTracksWarmedUp { get; set; }

        public SplitTrack(Track track, SubTrack[] subTracks, SubTrackNode firstTrackNode)
        {
            this.subTracks = subTracks;
            this.trackRef = track;
            this.firstTrackNode = firstTrackNode;
        }

        public override string ToString()
        {
            return String.Join(",\n------\n", subTracks.Select(subTrack => subTrack.ToString()).ToArray());
        }
    }
}
