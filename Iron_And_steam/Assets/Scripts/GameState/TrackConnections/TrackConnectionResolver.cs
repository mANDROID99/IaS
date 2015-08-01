using System;
using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.GameObjects;
using IaS.GameState.TrackConnections;
using IaS.Helpers;

namespace IaS.GameState
{
    public class TrackConnectionResolver : EventConsumer<BlockRotationEvent>
    {

        private readonly Dictionary<SubTrackGroup, TrackConnection> _connectionsMap = new Dictionary<SubTrackGroup, TrackConnection>();
        private readonly Dictionary<InstanceWrapper, SubTrackGroup> _instancesMap = new Dictionary<InstanceWrapper, SubTrackGroup>(); 
        private readonly TrackConnection[] _connections;

        public TrackConnectionResolver(EventRegistry eventRegistry, TrackContext[] tracks, Junction[] junctions)
        {
            eventRegistry.RegisterConsumer(this);
            _connections = GenerateConnections(tracks, junctions);
        }

        private TrackConnection[] GenerateConnections(TrackContext[] tracks, Junction[] junctions)
        {
            List<TrackConnection> connections = new List<TrackConnection>();
            foreach (SubTrackGroup group in tracks.SelectMany(t => t.SplitTrack.AllSubTrackGroups()))
            {
                
                if (!group.HasInstance)
                    throw new Exception("Cannot initialize track connections for a Subtrack that has not been instantiated.");

                TrackConnection conn;
                if (!AttachJunction(out conn, group, junctions))
                {
                    conn = AttachOneToOneConnection(group);
                }

                _connectionsMap.Add(group, conn);
                _instancesMap.Add(group.InstanceWrapper, group);
                connections.Add(conn);
            }

            return connections.ToArray();
        }

        private bool AttachJunction(out TrackConnection trackConnection, SubTrackGroup group, Junction[] junctions)
        {
            foreach (Junction junction in junctions.Where(j => j.ReferencesGroup(group)))
            {
                switch (junction.Direction)
                {
                    case Junction.JunctionDirection.OneToMany:
                        trackConnection = new TrackConnection(group,
                            new JunctionConnectionFilter.StartFilter(group, junction),
                            new OneToOneConnectionFilter.EndFilter(group));
                        return true;
                    case Junction.JunctionDirection.ManyToOne:
                        trackConnection = new TrackConnection(group,
                            new OneToOneConnectionFilter.StartFilter(group),
                            new JunctionConnectionFilter.EndFilter(group, junction));
                        return true;
                }
            }

            trackConnection = null;
            return false;
        }

        private TrackConnection AttachOneToOneConnection(SubTrackGroup group)
        {
           return new TrackConnection(group, 
                new OneToOneConnectionFilter.StartFilter(group), 
                new OneToOneConnectionFilter.EndFilter(group));
        }

        public SubTrackGroup GetNext(SubTrackGroup last, out Transformation transform)
        {
            TrackConnection lastConnection = _connectionsMap[last];
            foreach (TrackConnection conn in _connections)
            {
                if(conn == lastConnection) continue;

                IStartConnectionFilter connStart = conn.StartFilter;
                IEndConnectionFilter lastEnd = lastConnection.EndFilter; 

                if ((connStart.AllowPrevious(lastEnd)) && (lastEnd.AllowNext(connStart)))
                {
                    transform = conn.Transformation;
                    return conn.TrackGroup;
                }
            }

            transform = null;
            return null;
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            SubTrackGroup group;
            if (!_instancesMap.TryGetValue(evt.rotatedInstance, out group))
            {
                return;
            }

            TrackConnection connection = _connectionsMap[group];
            connection.Transformation = evt.transformation;
            connection.StartFilter.Rotate(evt.transformation);
            connection.EndFilter.Rotate(evt.transformation);
        }

        private class TrackConnection
        {
            internal readonly SubTrackGroup TrackGroup;
            internal readonly IStartConnectionFilter StartFilter;
            internal readonly IEndConnectionFilter EndFilter;
            internal Transformation Transformation;

            public TrackConnection(SubTrackGroup trackGroup, IStartConnectionFilter startFilter, IEndConnectionFilter endFilter)
            {
                TrackGroup = trackGroup;
                StartFilter = startFilter;
                EndFilter = endFilter;
                Transformation = IdentityTransform.IDENTITY;
            }
        }
    }
}
