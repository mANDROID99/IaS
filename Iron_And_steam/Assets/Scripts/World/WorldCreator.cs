using Assets.Scripts.Controllers;
using IaS.Domain;
using IaS.Domain.WorldTree;
using IaS.Domain.XmlToDomainMapper;
using IaS.GameState;
using IaS.GameState.Events;
using IaS.World.Builder;
using IaS.Xml;
using UnityEngine;

namespace IaS.World
{
    public class WorldCreator : MonoBehaviour, EventConsumer<GameEvent>
    {

        [SerializeField]
        public TextAsset LevelXmlAsset;
        [SerializeField]
        public TextAsset LevelXmlSchemaAsset;
        [SerializeField]
        public GameObject BlockPrefab;
        [SerializeField]
        public GameObject TrackPrefab;
        [SerializeField]
        public GameObject TrainPrefab;
        [SerializeField]
        public GameObject ArrowPrefab;
        [SerializeField]
        public GameObject GoalPrefab;
        [SerializeField]
        public GameObject PointerPrefab;

        private LevelTree _levelTree;
        private bool _paused = false;
        private float _timePauseBegin;
        private float _timePausedOffset = 0;

        void Start()
        {
            LoadWorld();
        }

        void Update()
        {
            if (_paused) return;

            float time = Time.time - _timePausedOffset;
            GlobalGameState globalGameState = new GlobalGameState(time);

            foreach (Controller controller in _levelTree.Controllers)
            {
                controller.Update(this, globalGameState);
            }
        }

        public void LoadWorld()
        {
            RemoveAllChildren();
            var prefabs = new Prefabs(TrackPrefab, BlockPrefab, TrainPrefab, ArrowPrefab, GoalPrefab, PointerPrefab);

            XmlLevelParser levelParser = new XmlLevelParser();
            LevelMapper levelMapper = new LevelMapper();
            LevelTreeBuilder worldBuilder = new LevelTreeBuilder();

            XmlLevel xLevel = levelParser.ParseLevel(LevelXmlAsset);
            Level level = levelMapper.MapXmlToDomain(xLevel);
            _levelTree = worldBuilder.BuildFromDomain(level, this.transform, prefabs);
            _levelTree.EventRegistry.RegisterConsumer(this);
        }

        private void RemoveAllChildren()
        {
            for (var i = 0; i < transform.childCount; i += 1)
            {
                var childGo = transform.GetChild(i).gameObject;
                DestroyImmediate(childGo);
            }
        }

        public void OnEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case GameEvent.Type.PAUSED:
                    _timePauseBegin = Time.time;
                    _paused = true;
                    break;
                case GameEvent.Type.RESUMED:
                    _timePausedOffset += Time.time - _timePauseBegin;
                    _paused = false;
                    break;
            }
        }
    }
}
