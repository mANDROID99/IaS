
using IaS.GameState.Creators;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.GameState
{
    class LevelCreator
    {
        private readonly GroupCreator _groupCreator = new GroupCreator();

        public LevelTree CreateLevel(LevelXML level, Transform parentTransform, Prefabs prefabs)
        {
            var levelData = new LevelTree.LevelData(prefabs);
            var levelTree = new LevelTree(level.LevelId, parentTransform, levelData);

            _groupCreator.CreateGroups(levelTree, level.Groups);
            return levelTree;
        }
    }
}
