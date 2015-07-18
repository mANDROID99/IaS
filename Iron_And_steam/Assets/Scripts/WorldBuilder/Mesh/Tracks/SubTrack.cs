using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.Splines;

namespace IaS.WorldBuilder.Meshes.Tracks
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

    public class SubTrack
    {
        public BlockBounds subBounds { get; private set; }
        public SubTrackNode[][] trackNodes { get; private set; }

        public SubTrack(BlockBounds subBounds, SubTrackNode[][] trackNodes)
        {
            this.subBounds = subBounds;
            this.trackNodes = trackNodes;
        }

        public override string ToString()
        {

            return String.Format("subTrack: {0},\ngroups: [{1}\n]", subBounds, 
                String.Join("", trackNodes.Select(
                        group => String.Format("\n   group: [{0}]", String.Join(", ", group.Select(node => node.ToString()).ToArray()))
                    ).ToArray()
                )
           );
        }
    }

    public class SubTrackNode
    {
        public Vector3 position { get; private set; }

        public Vector3 forward { get; set; }
        public Vector3 down { get; set; }

        public SubTrackNode previous { get; set; }
        public SubTrackNode next { get; set; }

        public SubTrackNode(Vector3 position, Vector3 forward, Vector3 down)
        {
            this.position = position;
            this.forward = forward;
            this.down = down;
        }

        public override string ToString()
        {
            return String.Format("{{pos:{0}, fwd:{1}}}", position, forward);
        }
    }
}
