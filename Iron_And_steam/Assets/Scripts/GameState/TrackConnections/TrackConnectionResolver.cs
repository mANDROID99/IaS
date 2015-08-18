using System;
using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.Domain.WorldTree;
using IaS.GameObjects;
using IaS.GameState.Rotation;
using IaS.GameState.TrackConnections;
using IaS.Helpers;
using IaS.WorldBuilder;

namespace IaS.GameState
{
    public class TrackConnectionResolver : EventConsumer<BlockRotationEvent>
    {

        private readonly Dictionary<SubTrackGroup, TrackConnection> _connectionsMap = new Dictionary<SubTrackGroup, TrackConnection>();
        private readonly Dictionary<BlockBounds, List<SubTrackGroup>> _blockBoundsMap = new Dictionary<BlockBounds, List<SubTrackGroup>>(); 
        private readonly List<TrackConnection> _connections = new List<TrackConnection>();



        public TrackConnectionResolver(EventRegistry eventRegistry)
        {
            eventRegistry.RegisterConsumer(this);
        }

        public void AddSplitTracks(SplitTrack[] splitTracks, Junction[] junctions, GroupBranch groupBranch)
        {
            foreach (SubTrack subTrack in splitTracks.SelectMany(t => t.SubTracks))
            {
                List<SubTrackGroup> stGroups;
                if (!_blockBoundsMap.TryGetValue(subTrack.SplitBounds, out stGroups))
                {
                    _blockBoundsMap.Add(subTrack.SplitBounds, (stGroups = new List<SubTrackGroup>()));
                }

                foreach (SubTrackGroup stGroup in subTrack.TrackGroups)
                {
                    stGroups.Add(stGroup);
                    TrackConnection conn;
                    if (!AttachJunction(out conn, stGroup, junctions))
                    {
                        conn = AttachOneToOneConnection(stGroup);
                    }

                    _connectionsMap.Add(stGroup, conn);
                    _connections.Add(conn);
                }
            }
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

        public TrackConnection GetFirst(SubTrackGroup firstSubTrackGroup)
        {
            return _connections.First(c => c.TrackGroup == firstSubTrackGroup);
        }

        public TrackConnection GetNext(TrackConnection last, bool lastReversed, out bool nextReversed)
        {
            foreach (TrackConnection conn in _connections)
            {
                if (conn == last) continue;

                IStartConnectionFilter connStart = conn.StartFilter;
                IEndConnectionFilter connEnd = conn.EndFilter;
                IStartConnectionFilter lastStart = last.StartFilter;
                IEndConnectionFilter lastEnd = last.EndFilter;

                bool? newReverse = null;

                if (!lastReversed)
                {
                    if (connStart.AllowConnection(lastEnd))
                    {
                        newReverse = false;
                    }

                    if (connEnd.AllowReversed(lastEnd))
                    {
                        newReverse = true;
                    }
                }
                else
                {
                    if (connEnd.AllowConnection(lastStart))
                    {
                        newReverse = true;
                    }

                    if (connStart.AllowReversed(lastStart))
                    {
                        newReverse = false;
                    }
                }

                if (newReverse.HasValue)
                {
                    nextReversed = newReverse.Value;
                    return conn;
                }
            }

            nextReversed = lastReversed;
            return null;
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            List<SubTrackGroup> groups;
            if (!_blockBoundsMap.TryGetValue(evt.RotatedBounds, out groups))
            {
                return;
            }

            foreach (TrackConnection conn in groups.Select(g => _connectionsMap[g]))
            {
                conn.Transformation = evt.Transformation;
                conn.StartFilter.Rotate(evt.Transformation);
                conn.EndFilter.Rotate(evt.Transformation);
            }
        }

        public class TrackConnection
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
                Transformation = Transformation.None;
            }
        }
    }
}
