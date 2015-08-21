using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.GameState;
using IaS.Domain;
using UnityEngine;

namespace IaS.Domain.WorldTree
{
    public class LevelTree : BaseTree
    {
        public readonly LevelData Data;
        private readonly Dictionary<Group, GroupBranch> _groupBranches = new Dictionary<Group, GroupBranch>();

        public string LevelName { get { return Data.Level.LevelName; } }
        public EventRegistry EventRegistry { get { return Data.EventRegistry; } }
        public Prefabs Prefabs { get { return Data.Prefabs; } }
        public List<Controller> Controllers { get { return Data.Controllers; } }
        public TrackConnectionResolver ConnectionResolver { get { return Data.ConnectionResolver; } }
        public Level Level { get { return Data.Level; } }

        public class LevelData 
        {
            public readonly List<Controller> Controllers = new List<Controller>();
            public readonly EventRegistry EventRegistry;
            public readonly TrackConnectionResolver ConnectionResolver;
            public readonly Level Level;
            public readonly Prefabs Prefabs;

            public LevelData(Level level, EventRegistry eventRegistry, TrackConnectionResolver connectionResolver, Prefabs prefabs)
            {
                Prefabs = prefabs;
                EventRegistry = eventRegistry;
                ConnectionResolver = connectionResolver;
                Level = level;
            }
        }


        public void RegisterController(params Controller[] controllers)
        {
            Data.Controllers.AddRange(controllers);
        }  

        public LevelTree(Transform parentTransform, LevelData data) : base(data.Level.LevelName, new Vector3(), parentTransform)
        {
            Data = data;
        }

        public void AddGroupBranch(GroupBranch groupBranch)
        {
            _groupBranches.Add(groupBranch.GroupData.Group, groupBranch);
        }

        public GroupBranch GetGroupBranch(Group group)
        {
            return _groupBranches[group];
        }

        public IEnumerable<SplitTrack> AllSplitTracks()
        {
           return _groupBranches.Values.SelectMany(group => group.Tracks);
        }
    }

    
}
