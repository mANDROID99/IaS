using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameState.TrackConnections;
using IaS.Domain;
using IaS.GameState;
using IaS.GameState.Creators;
using IaS.GameState.WorldTree;
using UnityEngine;

namespace IaS.Controllers
{
    class JunctionController : Controller
    {
        private const float ArrowSpawnInterval = 0.2f;
        
        private readonly Junction[] _junctions;
        private readonly TrackConnectionResolver _connectionResolver;
        private readonly GameObject _arrowPrefab;
        private readonly GroupBranch _groupBranch;
        private readonly Queue<ArrowController> _arrows = new Queue<ArrowController>();  

        private float _arrowSpawnTimer = -ArrowSpawnInterval;

        public JunctionController(GroupBranch groupBranch, GameObject arrowPrefab, TrackConnectionResolver connectionResolver, Junction[] junctions)
        {
            _junctions = junctions;
            _connectionResolver = connectionResolver;
            _arrowPrefab = arrowPrefab;
            _groupBranch = groupBranch;
        }

        public void Update(MonoBehaviour mono)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                foreach (Junction junction in _junctions)
                    junction.SwitchDirection();
            }

            SpawnArrows();
            UpdateArrows();
        }

        private void UpdateArrows()
        {
            float time = Time.time;
            bool pop = _arrows.Aggregate(false, (current, arrow) => !arrow.Update(time) || current);

            if (pop)
            {
                _arrows.Peek().Destroy();
                _arrows.Dequeue();
            }
        }

        private void SpawnArrows()
        {
            float time = Time.time;
            if (time - _arrowSpawnTimer < ArrowSpawnInterval) return;

            foreach (Junction junction in _junctions)
            {
                TrackRunner trackRunner = new TrackRunner(_connectionResolver, junction.NextBranch, false);
                _arrows.Enqueue(new ArrowController(time, _arrowPrefab, trackRunner, _groupBranch));
                _arrowSpawnTimer += ArrowSpawnInterval;
            }
        }
    }
}
