using System;
using System.Linq;
using IaS.GameState;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.Domain
{
    public class SubTrack
    {
        public BlockBounds subBounds { get; private set; }
        public SubTrackGroup[] trackGroups { get; private set; }
        public InstanceWrapper instanceWrapper { get; set; }
        public readonly SubTrackGroup FirstGroup;
        public readonly SubTrackGroup LastGroup;

        public SubTrack(BlockBounds subBounds, SubTrackGroup[] trackGroups, SubTrackGroup firstGroup, SubTrackGroup lastGroup)
        {
            FirstGroup = firstGroup;
            LastGroup = lastGroup;
            this.instanceWrapper = null;
            this.subBounds = subBounds;
            this.trackGroups = trackGroups;
            UpdateReferences();
        }

        private void UpdateReferences()
        {
            foreach (SubTrackGroup trackGroup in trackGroups)
            {
                trackGroup.subTrack = this;
                trackGroup.UpdateReferences();
            }
        }
    }

    public class SubTrackNode
    {
        public Vector3 position { get; private set; }
        public SubTrackGroup group { get; internal set; }

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
            return string.Format("{{pos:{0}, fwd:{1}}}", position, forward);
        }

        
    }
}
