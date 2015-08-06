using UnityEngine;

namespace IaS.GameState.Creators
{
    public struct Prefabs
    {
        public readonly GameObject BlockPrefab;
        public readonly GameObject TrackPrefab;
        public readonly GameObject TrainPrefab;
        public readonly GameObject ArrowPrefab;

        public Prefabs(
            GameObject trackPrefab, 
            GameObject blockPrefab, 
            GameObject trainPrefab, 
            GameObject arrowPrefab)
        {
            TrackPrefab = trackPrefab;
            BlockPrefab = blockPrefab;
            TrainPrefab = trainPrefab;
            ArrowPrefab = arrowPrefab;
        }
    }

    
}
