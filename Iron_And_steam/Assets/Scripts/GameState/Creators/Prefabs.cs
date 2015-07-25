using UnityEngine;

namespace IaS.GameState.Creators
{
    public struct Prefabs
    {
        public readonly GameObject BlockPrefab;
        public readonly GameObject TrackPrefab;
        public readonly GameObject TrainPrefab;
        public readonly Transform RootTransform;

        public Prefabs(GameObject trackPrefab, GameObject blockPrefab, GameObject trainPrefab, Transform rootTransform)
        {
            TrackPrefab = trackPrefab;
            BlockPrefab = blockPrefab;
            TrainPrefab = trainPrefab;
            RootTransform = rootTransform;
        }
    }
}
