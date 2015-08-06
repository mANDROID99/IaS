using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.Domain;
using IaS.GameState;
using IaS.GameState.Creators;
using IaS.GameState.WorldTree;
using UnityEngine;

namespace IaS.Controllers
{
    class JunctionController : Controller
    {
        private const float ArrowSpawnInterval = 0.75f;
        
        private readonly Junction _junction;
        private readonly TrackConnectionResolver _connectionResolver;
        private readonly GameObject _arrowPrefab;
        private readonly BaseTree _particlesLeaf;
        private readonly Queue<ArrowController> _arrows = new Queue<ArrowController>();  

        private float _arrowSpawnTimer = -ArrowSpawnInterval;

        public JunctionController(GroupBranch branch, Prefabs prefabs, TrackConnectionResolver connectionResolver, Junction junction)
        {
            _junction = junction;
            _connectionResolver = connectionResolver;
            _arrowPrefab = prefabs.ArrowPrefab;
            _particlesLeaf = branch.ParticlesLeaf;
        }

        public void Update(MonoBehaviour mono)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                _junction.SwitchDirection();
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
            
            _arrows.Enqueue(new ArrowController(time, _arrowPrefab, _particlesLeaf));
            _arrowSpawnTimer += ArrowSpawnInterval;
        }
    }
}
