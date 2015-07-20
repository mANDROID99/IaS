using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.Helpers;
using IaS.WorldBuilder;
using IaS.GameObjects;
using IaS.WorldBuilder.Tracks;

namespace IaS.GameState
{
    public class TrackConnections : EventConsumer<BlockRotationEvent>
    {
        private SplitTrack track;
        private List<Connection> connections = new List<Connection>();
        private Dictionary<SubTrackGroup, Connection> connectionsMap = new Dictionary<SubTrackGroup, Connection>();
        private Dictionary<InstanceWrapper, SubTrack> instancesMap = new Dictionary<InstanceWrapper, SubTrack>();
        private WorldContext world;

        public TrackConnections(WorldContext world, SplitTrack track)
        {
            this.world = world;
            this.track = track;
            world.eventRegistry.RegisterConsumer(this);
        }

        public void RegisterSubTrack(InstanceWrapper instance, SubTrack subTrack)
        {
            instancesMap.Add(instance, subTrack);
            foreach (SubTrackGroup group in subTrack.trackGroups)
            {
                Connection conn = new Connection(group);
                connections.Add(conn);
                connectionsMap.Add(group, conn);
                RefreshConnections(conn);
            }
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            if (evt.type == BlockRotationEvent.EventType.BeforeRotation)
            {
                SubTrack subTrack;
                if(instancesMap.TryGetValue(evt.rotatedInstance, out subTrack))
                {
                    foreach (SubTrackGroup group in subTrack.trackGroups)
                    {
                        Connection from = connectionsMap[group];
                        from.RotateStartAndEndPoints(evt.transformation);
                        RefreshConnections(from);
                    }
                }
            }
        }

        private void RefreshConnections(Connection from)
        {
            if (from.nextConnection != null)
                from.nextConnection.prevConnection = null;

            from.nextConnection = null;

            foreach(Connection to in connections)
            {
                if(to.trackGroup.subTrack != from.trackGroup.subTrack)
                {
                    if((Vector3.Distance(to.startPos, from.endPos) < 0.1f) && (Vector3.Distance(to.startForward, from.endForward) < 0.1f))
                    {
                        from.nextConnection = to;
                        to.prevConnection = from;
                        break;
                    }

                    if((Vector3.Distance(to.endPos, from.startPos) < 0.1f) && (Vector3.Distance(to.endForward, from.startForward) < 0.1f))
                    {
                        to.nextConnection = from;
                        from.prevConnection = to;
                        break;
                    }
                }
            }
        }

        public SubTrackGroup GetNext(SubTrackGroup last, out Transformation transform)
        {
             Connection conn = connectionsMap[last];
            if(conn.nextConnection == null)
            {
                transform = null;
                return null;
            }

            SubTrackGroup nextGroup = conn.nextConnection.trackGroup;
            transform = conn.nextConnection.transform;
            return nextGroup;
        }

        private class Connection
        {
            internal SubTrackGroup trackGroup;
            internal Connection nextConnection = null;
            internal Connection prevConnection = null;
            internal Vector3 startPos;
            internal Vector3 endPos;
            internal Vector3 startForward;
            internal Vector3 endForward;
            internal Transformation transform;

            internal Connection( SubTrackGroup trackGroup)
            {
                this.trackGroup = trackGroup;
                this.startPos = trackGroup.spline.pts[0].startPos;
                this.endPos = trackGroup.spline.pts.Last().endPos;
                this.startForward = trackGroup[0].forward;
                this.endForward = trackGroup.Last().forward;
                this.transform = IdentityTransform.IDENTITY;
            }

            internal void RotateStartAndEndPoints(Transformation transform)
            {
                this.startPos = transform.Transform(trackGroup.spline.pts[0].startPos);
                this.endPos = transform.Transform(trackGroup.spline.pts.Last().endPos);
                this.startForward = transform.TransformVector(trackGroup[0].forward);
                this.endForward = transform.TransformVector(trackGroup.Last().forward);
                this.transform = transform;
            }
        }
    }
}
