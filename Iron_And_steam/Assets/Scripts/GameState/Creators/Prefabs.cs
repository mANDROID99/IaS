using IaS.Domain;
using UnityEngine;

namespace IaS.GameState.Creators
{
    public struct Prefabs
    {
        public readonly GameObject BlockPrefab;
        public readonly GameObject TrackPrefab;
        public readonly GameObject TrainPrefab;
        public readonly GameObject ArrowPrefab;
        public readonly GameObject GoalPrefab;
        public readonly GameObject PointerPrefab;

        public Prefabs(
            GameObject trackPrefab, 
            GameObject blockPrefab, 
            GameObject trainPrefab, 
            GameObject arrowPrefab,
            GameObject goalPrefab,
            GameObject pointerPrefab)
        {
            TrackPrefab = trackPrefab;
            BlockPrefab = blockPrefab;
            TrainPrefab = trainPrefab;
            ArrowPrefab = arrowPrefab;
            GoalPrefab = goalPrefab;
            PointerPrefab = pointerPrefab;
        }
    }

    
}
