
using System.Linq;
using IaS.Domain;
using IaS.GameObjects;
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

            TrackConnectionResolver connectionResolver = new TrackConnectionResolver(levelTree.EventRegistry);

            _groupCreator.CreateGroups(levelTree, connectionResolver, level.Groups);
            SplitTrack startTrack = levelTree.AllSplitTracks().First(t => t.TrackXml == level.StartTrack);
            SplitTrack endTrack = levelTree.AllSplitTracks().First(t => t.TrackXml == level.EndTrack);

            levelTree.RegisterController(CreateTrainController(levelTree, startTrack, connectionResolver));

            return levelTree;
        }

        private TrainController CreateTrainController(LevelTree level, SplitTrack startTrack, TrackConnectionResolver trackConnectionResolver)
        {
            GameObject trainPrefab = level.Prefabs.TrainPrefab;
            return new TrainController(level, startTrack, trainPrefab, trackConnectionResolver);
        }
    }
}
