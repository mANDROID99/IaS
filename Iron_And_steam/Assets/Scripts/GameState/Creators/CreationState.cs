using UnityEngine;

namespace IaS.GameState.Creators
{
    public class CreationState
    {
        public readonly GameObject BlockPrefab;
        public readonly GameObject TrackPrefab;
        public readonly GameObject TrainPrefab;
        public readonly GameObject ArrowPrefab;

        public readonly Transform RootTransform;
        public Transform GroupTransform { get; internal set; }
        public Transform BlocksTransform { get; internal set; }
        public Transform TracksTransform { get; internal set; }
        public Transform ParticlesTransform { get; internal set; }

        public CreationState(
            GameObject trackPrefab, 
            GameObject blockPrefab, 
            GameObject trainPrefab, 
            GameObject arrowPrefab, 
            Transform rootTransform)
        {
            TrackPrefab = trackPrefab;
            BlockPrefab = blockPrefab;
            TrainPrefab = trainPrefab;
            RootTransform = rootTransform;
            ArrowPrefab = arrowPrefab;
        }
    }

    
}
