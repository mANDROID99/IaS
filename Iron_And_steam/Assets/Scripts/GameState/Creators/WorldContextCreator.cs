
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.GameState.Creators;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.GameState
{
    class WorldContextCreator
    {
        private readonly GroupContextCreator _groupContextCreator = new GroupContextCreator();

        public WorldContext CreateWorld(LevelDTO levelDto)
        {
            GroupContext[] groups = levelDto.Groups.Select(group => _groupContextCreator.CreateGroupContext(group)).ToArray();
            var worldContext = new WorldContext(groups);
            foreach (GroupContext group in groups)
            {
                group.World = worldContext;
            }
            return worldContext;
        }

        public void CreateWorldControllers(WorldContext worldContext, EventRegistry eventRegistry, List<Controller> controllers, Prefabs prefabs)
        {
            foreach (GroupContext group in worldContext.Groups)
            {
                _groupContextCreator.CreateGroupControllers(group, eventRegistry, controllers, prefabs);
            }
        }
    }
}
