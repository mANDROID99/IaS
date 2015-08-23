using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameState.TrackConnections;
using IaS.Domain;
using IaS.World.WorldTree;
using IaS.GameState;
using UnityEngine;

namespace IaS.Controllers
{
    class JunctionController : Controller
    {
        private const float ArrowSpawnInterval = 0.5f;
        private const float PointerRotationSpeed = 300;
        
        private readonly Junction _junction;
        private readonly TrackConnectionResolver _connectionResolver;
        private readonly GameObject _arrowPrefab;
        private readonly GameObject _pointerPrefab;
        private readonly GroupBranch _groupBranch;
        private readonly GameObject _pointerInstance;
        private readonly Queue<ArrowController> _arrows = new Queue<ArrowController>();  

        private float _arrowSpawnTimer = -ArrowSpawnInterval;

        public JunctionController(GroupBranch groupBranch, GameObject arrowPrefab, GameObject pointerPrefab, TrackConnectionResolver connectionResolver, Junction junction)
        {
            _junction = junction;
            _connectionResolver = connectionResolver;
            _arrowPrefab = arrowPrefab;
            _pointerPrefab = pointerPrefab;
            _groupBranch = groupBranch;
            _pointerInstance = InstantiatePointer();
        }

        private GameObject InstantiatePointer()
        {
            SubTrackGroup nextGroup = _junction.NextBranch;
            Vector3 up = -nextGroup.Nodes[0].Down;

            Quaternion rotation = Quaternion.LookRotation(_junction.NextDirection, up);
            Vector3 pos = _junction.Position + new Vector3(0.5f, 0.5f, 0.5f);

            BaseTree splitBranch = _junction.GetSplitBoundsBranch(_groupBranch);
            GameObject instance = Object.Instantiate(_pointerPrefab, pos, rotation) as GameObject;
            splitBranch.Attach(instance, true);
            return instance;
        }

        public void UpdatePointerDirection()
        {
            Vector3 up = -_junction.NextBranch.Nodes[0].Down;
            Quaternion endRotation = Quaternion.LookRotation(_junction.NextDirection, up);
            Quaternion deltaRotation = Quaternion.RotateTowards(_pointerInstance.transform.localRotation, endRotation, PointerRotationSpeed * Time.deltaTime);
            _pointerInstance.transform.localRotation = deltaRotation;
        }


        public void Update(MonoBehaviour mono, GlobalGameState gameState)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                _junction.SwitchDirection();
            }

            SpawnArrows(gameState);
            UpdateArrows(gameState);
            UpdatePointerDirection();
        }

        private void UpdateArrows(GlobalGameState gameState)
        {
            float time = gameState.Time;
            bool pop = _arrows.Aggregate(false, (current, arrow) => !arrow.Update(time) || current);

            if (pop)
            {
                _arrows.Peek().Destroy();
                _arrows.Dequeue();
            }
        }

        private void SpawnArrows(GlobalGameState gameState)
        {
            float time = gameState.Time;
            if (time - _arrowSpawnTimer < ArrowSpawnInterval) return;

            TrackRunner trackRunner = new TrackRunner(_connectionResolver, _junction.NextBranch, false);
            _arrows.Enqueue(new ArrowController(time, _arrowPrefab, trackRunner));
            _arrowSpawnTimer += ArrowSpawnInterval;
        }
    }
}
