using Assets.Scripts.Controllers;
using Assets.Scripts.GameState.TrackConnections;
using IaS.Controllers.GO;
using IaS.Domain;
using IaS.GameState;
using IaS.GameState.WorldTree;
using UnityEngine;

namespace IaS.GameObjects
{
    public class TrainController : Controller
    {
        private readonly TrackFollowingGameObject _trackFollowingGO;
        private readonly TrackRunner _trackRunner;

        private bool _started = false;

        public TrainController(LevelTree level, SplitTrack startTrack, GameObject trainPrefab, TrackConnectionResolver connectionResolver)
        {
            SubTrackGroup stGroup = startTrack.FirstSubTrack.FirstGroup;
            _trackRunner = new TrackRunner(connectionResolver, stGroup, false);

            GameObject go = Object.Instantiate(trainPrefab);
            _trackFollowingGO = new TrackFollowingGameObject(go, _trackRunner, Vector3.forward);
        }

        public void Update(MonoBehaviour mono, GlobalGameState gameState)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _started = true;
            }

            if (_started)
            {
                _trackFollowingGO.MoveUpdate(1.5f * Time.deltaTime);
            }
        }

    }
}
