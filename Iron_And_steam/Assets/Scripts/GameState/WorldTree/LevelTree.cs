using System.Collections.Generic;
using Assets.Scripts.Controllers;
using IaS.GameState.Creators;
using UnityEngine;

namespace IaS.GameState.WorldTree
{
    public class LevelTree : BaseTree
    {
        public readonly string LevelName;
        public readonly LevelData Data;
        private readonly Dictionary<string, GroupBranch> _groupBranches = new Dictionary<string, GroupBranch>();

        public EventRegistry EventRegistry { get { return Data.EventRegistry; } }
        public Prefabs Prefabs { get { return Data.Prefabs; } }

        public class LevelData 
        {
            public readonly List<Controller> Controllers = new List<Controller>();
            public readonly EventRegistry EventRegistry = new EventRegistry();
            public readonly Prefabs Prefabs;

            public LevelData(Prefabs prefabs)
            {
                Prefabs = prefabs;
            }
        }

        public void RegisterController(params Controller[] controllers)
        {
            Data.Controllers.AddRange(controllers);
        }  

        public LevelTree(string levelName, Transform parentTransform, LevelData data) : base(levelName, new Vector3(), parentTransform)
        {
            LevelName = levelName;
            Data = data;
        }

        public void AddGroupBranch(string groupId, GroupBranch groupBranch)
        {
            _groupBranches.Add(groupId, groupBranch);
        }

        public GroupBranch GetGroupBranch(string groupId)
        {
            return _groupBranches[groupId];
        }
    }

    
}
