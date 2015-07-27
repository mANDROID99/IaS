using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.GameObjects;
using IaS.Helpers;
using UnityEngine;

namespace IaS.GameState
{
    public class TrackConnectionMapper : EventConsumer<BlockRotationEvent>
    {
        private readonly SplitTrack _track;
        private readonly List<Connection> _connections = new List<Connection>();
        private readonly Dictionary<SubTrackGroup, Connection> _connectionsMap = new Dictionary<SubTrackGroup, Connection>();
        private readonly Dictionary<InstanceWrapper, SubTrack> _instancesMap = new Dictionary<InstanceWrapper, SubTrack>();
        private readonly BlockRotaterController _blockRotaterController;

        public TrackConnectionMapper(BlockRotaterController blockRotaterController, EventRegistry eventRegistry, TrackContext[] tracks, Junction[] junctions)
        {
            _blockRotaterController = blockRotaterController;
            _track = tracks[0].SplitTrack;
            eventRegistry.RegisterConsumer(this);
        }

        public void AddSubTrackTrackInstances(SubTrack[] subTracks)
        {
            _blockRotaterController.AddInstancesToRotate(subTracks.Select(subTrack => subTrack.instanceWrapper).ToArray());
            foreach (SubTrack subTrack in subTracks)
            {
                _instancesMap.Add(subTrack.instanceWrapper, subTrack);
                
                foreach (var group in subTrack.trackGroups)
                {
                    var conn = new Connection(group);
                    _connections.Add(conn);
                    _connectionsMap.Add(group, conn);
                    RefreshConnections(conn);
                }
            }
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            if (evt.type == BlockRotationEvent.EventType.BeforeRotation)
            {
                SubTrack subTrack;
                if(_instancesMap.TryGetValue(evt.rotatedInstance, out subTrack))
                {
                    foreach (var connection in subTrack.trackGroups.Select(group => _connectionsMap[group]))
                    {
                        connection.RotateStartAndEndPoints(evt.transformation);
                        RefreshConnections(connection);
                    }
                }
            }
        }

        private void RefreshConnections(Connection current)
        {
            if (current.NextConnection != null)
                current.NextConnection.PrevConnection = null;

            current.NextConnection = null;

            foreach(var next in _connections)
            {
                if (next.trackGroup.subTrack == current.trackGroup.subTrack) continue;
                if ((Vector3.Distance(next.StartPos, current.EndPos) < 0.1f) && (Vector3.Angle(next.StartForward, current.EndForward) < 0.1f))
                {
                    current.NextConnection = next;
                    next.PrevConnection = current;
                }

                if((Vector3.Distance(next.EndPos, current.StartPos) < 0.1f) && (Vector3.Angle(next.EndForward, current.StartForward) < 0.1f))
                {
                    next.NextConnection = current;
                    current.PrevConnection = next;
                }
            }
        }

        public SubTrackGroup GetNext(SubTrackGroup last, out Transformation transform)
        {
            var conn = _connectionsMap[last];
            if(conn.NextConnection == null)
            {
                transform = null;
                return null;
            }

            var nextGroup = conn.NextConnection.trackGroup;
            transform = conn.NextConnection.Transform;
            return nextGroup;
        }

        private class Connection
        {
            internal readonly SubTrackGroup trackGroup;
            internal Connection NextConnection = null;
            internal Connection PrevConnection = null;
            internal Vector3 StartPos;
            internal Vector3 EndPos;
            internal Vector3 StartForward;
            internal Vector3 EndForward;
            internal Transformation Transform;

            internal Connection( SubTrackGroup trackGroup)
            {
                this.trackGroup = trackGroup;
                this.StartPos = trackGroup.spline.pts[0].startPos;
                this.EndPos = trackGroup.spline.pts.Last().endPos;
                this.StartForward = trackGroup[0].forward;
                this.EndForward = trackGroup.Last().forward;
                this.Transform = IdentityTransform.IDENTITY;
            }

            internal void RotateStartAndEndPoints(Transformation transform)
            {
                this.StartPos = transform.Transform(trackGroup.spline.pts[0].startPos);
                this.EndPos = transform.Transform(trackGroup.spline.pts.Last().endPos);
                this.StartForward = transform.TransformVector(trackGroup[0].forward);
                this.EndForward = transform.TransformVector(trackGroup.Last().forward);
                this.Transform = transform;
            }
        }
    }
}
