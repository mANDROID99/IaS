using IaS.Domain;
using IaS.World.WorldTree;
using IaS.GameObjects;
using IaS.GameState;
using UnityEngine;

namespace IaS.World.Builder
{
    class LevelTreeBuilder
    {
        private readonly GroupTreeBuilder _groupTreeBuilder = new GroupTreeBuilder();

        public LevelTree BuildFromDomain(Level level, Transform transform, Prefabs prefabs)
        {
            EventRegistry eventRegistry = new EventRegistry();
            TrackConnectionResolver connectionResolver = new TrackConnectionResolver(eventRegistry);

            LevelTree.LevelData data = new LevelTree.LevelData(level, eventRegistry, connectionResolver, prefabs);
            LevelTree levelTree = new LevelTree(transform, data);
            _groupTreeBuilder.BuildFromDomain(levelTree, level.Groups);

            BlockRotaterController blockRotater = new BlockRotaterController(levelTree, eventRegistry);
            levelTree.RegisterController(blockRotater);

            levelTree.RegisterController(
                CreateTrainController(levelTree, level.Start, connectionResolver));

            return levelTree;
        }

        private TrainController CreateTrainController(LevelTree levelTree, SplitTrack start, TrackConnectionResolver trackConnectionResolver)
        {
            GameObject trainPrefab = levelTree.Prefabs.TrainPrefab;
            return new TrainController(levelTree, start, trainPrefab, trackConnectionResolver);
        }
    }
}
