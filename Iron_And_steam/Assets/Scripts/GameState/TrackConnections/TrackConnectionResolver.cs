using System;
using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.World.WorldTree;
using IaS.GameObjects;
using IaS.GameState.Rotation;
using IaS.GameState.TrackConnections;
using IaS.Helpers;
using IaS.Scripts.Domain;
using UnityEngine;

namespace IaS.GameState
{
    public class TrackConnectionResolver : EventConsumer<BlockRotationEvent>, EventConsumer<GroupRotationEvent>
    {
        private List<GroupConnections> _groupConnectionsList = new List<GroupConnections>();
        private Dictionary<Group, GroupConnections> _groupConnections = new Dictionary<Group, GroupConnections>();

        public TrackConnectionResolver(EventRegistry eventRegistry)
        {
            eventRegistry.RegisterConsumer(this as EventConsumer<BlockRotationEvent>);
            eventRegistry.RegisterConsumer(this as EventConsumer<GroupRotationEvent>);
        }

        private V GetOrCreateDefaultFromDict<K, V>(K key, Dictionary<K, V> dict, out bool created) where V : class, new()
        {
            bool _created = false;
            V value;
            if(!dict.TryGetValue(key, out value))
            {
                _created = true;
                value = new V();
                dict.Add(key, value);
            }

            created = _created;
            return value;
        }

        public void AddConnectionsFromGroup(Group group)
        {
            bool created;
            GroupConnections groupConnections = GetOrCreateDefaultFromDict(group, _groupConnections, out created);
            if (created) _groupConnectionsList.Add(groupConnections);
            
            foreach(SubTrack subTrack in group.Tracks.SelectMany(t => t.SubTracks))
            {
                BlockBounds subTrackBounds = subTrack.SplitBounds;
                BlockConnections blockConnections = GetOrCreateDefaultFromDict(subTrackBounds, groupConnections.BlockConnections, out created);
                if (created) groupConnections.BlockConnectionsList.Add(blockConnections);

                foreach(SubTrackGroup stGroup in subTrack.TrackGroups)
                {
                    Connection conn = new Connection(stGroup, null, null);
                    blockConnections.Connections.Add(stGroup, conn);
                    blockConnections.ConnectionsList.Add(conn);
                }
            }
        }

        public ConnectionContext FirstContext(SubTrackGroup stGroup, bool reverseInitially)
        {
            foreach (KeyValuePair<Group, GroupConnections> kvGroupConns in _groupConnections)
            {
                foreach (KeyValuePair<BlockBounds, BlockConnections> kvBlockConns in kvGroupConns.Value.BlockConnections)
                {
                    Connection conn;
                    if (kvBlockConns.Value.Connections.TryGetValue(stGroup, out conn))
                    {
                        return new ConnectionContext(conn, kvBlockConns.Value, kvGroupConns.Value, reverseInitially);
                    }
                }
            }

            throw new Exception("Connection could not be found!");
        }
        
        public ConnectionContext GetNext(ConnectionContext lastCtx)
        {
            foreach(GroupConnections gConns in _groupConnectionsList)
            {

                foreach(BlockConnections bConns in gConns.BlockConnectionsList)
                {

                    foreach(Connection conn in bConns.ConnectionsList)
                    {
                        if (conn == lastCtx.Connection) continue;

                        ConnectionContext toCtx = new ConnectionContext(conn, bConns, gConns, false);
                        toCtx = Compare(lastCtx, toCtx);
                        if (toCtx != null)
                        {
                            return toCtx;
                        }
                        
                    }
                }
            }

            return null;
        }

        private ConnectionContext Compare(ConnectionContext from, ConnectionContext to)
        {
            Transformation fromGroupTransform = from.GroupConnections.Transformation;
            Transformation toGroupTransform = to.GroupConnections.Transformation;
            Transformation fromBlockTransform = from.BlockConnections.Transformation;
            Transformation toBlockTransform = to.BlockConnections.Transformation;

            SubTrackGroup fromSubTrackGroup = from.Connection.SubTrackGroup;
            SubTrackGroup toSubTrackGroup = to.Connection.SubTrackGroup;

            bool fromReversed = from.Reversed;
            foreach(bool toReversed in new[] { fromReversed, !fromReversed })
            {

                Vector3 startPos = !fromReversed ? fromSubTrackGroup.EndBezierPos : fromSubTrackGroup.StartBezierPos;
                Vector3 endPos = !toReversed ? toSubTrackGroup.StartBezierPos : toSubTrackGroup.EndBezierPos;

                Vector3 startForward = !fromReversed ? fromSubTrackGroup.EndForward : -fromSubTrackGroup.StartForward;
                Vector3 endForward = !toReversed ? toSubTrackGroup.StartForward : -toSubTrackGroup.EndForward;

                IConnectionFilter startFilter = !toReversed ? to.Connection.StartFilter : to.Connection.EndFilter;
                IConnectionFilter endFilter = !fromReversed ? from.Connection.EndFilter : from.Connection.StartFilter;

                startPos = fromGroupTransform.Transform( 
                    fromBlockTransform.Transform(startPos));

                endPos = toGroupTransform.Transform(
                    toGroupTransform.Transform(endPos));

                startForward = fromGroupTransform.TransformVector( 
                    fromBlockTransform.TransformVector(startForward));

                endForward = toGroupTransform.TransformVector( 
                    toBlockTransform.TransformVector(endForward));

                if(
                    Vector3.Distance(startPos, endPos) < 0.1f 
                    && Vector3.Angle(startForward, endForward) < 0.1f
                    && ((startFilter == null) || (!toReversed ? startFilter.AllowPrevious(fromSubTrackGroup) : startFilter.AllowNext(fromSubTrackGroup)))
                    && ((endFilter == null) || (!fromReversed ? endFilter.AllowNext(toSubTrackGroup) : endFilter.AllowPrevious(toSubTrackGroup))))
                {
                    return to.CopyButSetReverseTo(toReversed);
                }
            }
            return null;
        }

        public void OnEvent(GroupRotationEvent evt)
        {
            GroupConnections gConns;
            if (!_groupConnections.TryGetValue(evt.Group, out gConns)) throw new Exception("Couldn't find rotated group!");

            gConns.Transformation = evt.Transformation;
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            GroupConnections gConns;
            if (!_groupConnections.TryGetValue(evt.Group, out gConns)) throw new Exception("Couldn't find rotated group!");

            BlockConnections conn;
            if (!gConns.BlockConnections.TryGetValue(evt.Block, out conn)) throw new Exception("Couldn't find rotated block!");

            conn.Transformation = evt.Transformation;
        }


        internal class GroupConnections
        {
            internal readonly Dictionary<BlockBounds, BlockConnections> BlockConnections = new Dictionary<BlockBounds, BlockConnections>();
            internal readonly List<BlockConnections> BlockConnectionsList = new List<BlockConnections>();
            internal Transformation Transformation;

            internal Vector3 Transform(Vector3 transformed)
            {
                return Transformation.Transform(transformed);
            }

            internal Vector3 TransformVector(Vector3 vec)
            {
                return Transformation.TransformVector(vec);
            }
        }

        internal class BlockConnections
        {
            internal readonly Dictionary<SubTrackGroup, Connection> Connections = new Dictionary<SubTrackGroup, Connection>();
            internal readonly List<Connection> ConnectionsList = new List<Connection>();
            internal Transformation Transformation;

            internal Vector3 Transform(Vector3 transformed)
            {
                return Transformation.Transform(transformed);
            }

            internal Vector3 TransformVector(Vector3 vec)
            {
                return Transformation.TransformVector(vec);
            }
        }

        internal class Connection
        {
            internal readonly IConnectionFilter StartFilter;
            internal readonly IConnectionFilter EndFilter;
            internal readonly SubTrackGroup SubTrackGroup;

            internal Connection(SubTrackGroup stGroup, IConnectionFilter startFilter, IConnectionFilter endFilter)
            {
                SubTrackGroup = stGroup;
                StartFilter = startFilter;
                EndFilter = endFilter;
            }
        }

        public class ConnectionContext
        {
            internal readonly Connection Connection;
            internal readonly BlockConnections BlockConnections;
            internal readonly GroupConnections GroupConnections;
            public readonly bool Reversed;
            public SubTrackGroup TrackGroup { get { return Connection.SubTrackGroup; } }

            internal ConnectionContext(Connection connection, BlockConnections blockConnections, GroupConnections groupConnections, bool reversed)
            {
                Connection = connection;
                BlockConnections = blockConnections;
                GroupConnections = groupConnections;
                Reversed = reversed;
            }

            internal ConnectionContext CopyButSetReverseTo(bool reversed)
            {
                return new ConnectionContext(Connection, BlockConnections, GroupConnections, reversed);
            }
        }
    }
}
