using IaS.GameState;
using IaS.Helpers;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Tracks;
using UnityEngine;

namespace IaS.GameObjects
{
    public class TrainController : MonoBehaviour, EventConsumer<BlockRotationEvent>
    {
        private WorldContext _gameContext;
        private int _trackIndex;
        private BezierSpline.LinearInterpolator _splineInterpolator;

        private SubTrackGroup _currentStGroup = null;
        private Transformation _transformation = IdentityTransform.IDENTITY;
        private bool _started = false;
        private bool _paused = false;

        private Vector3 _forward = Vector3.forward;
        private Vector3 _up = Vector3.up;
        private Vector3 _lastSplineForward = Vector3.forward;

        private Quaternion _bezierRotation = Quaternion.identity;
        private Quaternion _worldRotation = Quaternion.identity;

        public static void AddToWorld(Transform parent, GameObject trainPrefab, WorldContext context, int trackIndex)
        {
            GameObject trainGo = Instantiate(trainPrefab);
            TrainController train = trainGo.GetComponent<TrainController>();
            train.Init(context, trackIndex);
            GameObjectUtils.AsChildOf(parent, new Vector3(), trainGo);
        }

        private void Init(WorldContext gameContext, int trackIndex)
        {
            Debug.Log(gameContext);
            this._gameContext = gameContext;
            this._trackIndex = trackIndex;
            this._gameContext.eventRegistry.RegisterConsumer(this);
        }

        private void SetTrackPosition(Vector3 trackPosition, Vector3 forward)
        {
            this.transform.position = trackPosition;
        }

        public void Start()
        {
            TrackContext trackContext = _gameContext.tracks[_trackIndex];
            this._currentStGroup = trackContext.splitTrack.firstTrackNode.group;
            this._splineInterpolator = _currentStGroup.spline.linearInterpolator();

            var startPos = _currentStGroup.spline.pts[0].startPos;
            SetTrackPosition(startPos, _currentStGroup[0].forward);
        }

        private void NextSubTrack()
        {
            if (_currentStGroup == null)
                return;

            var trackContext = _gameContext.tracks[_trackIndex];
            this._currentStGroup = trackContext.connections.GetNext(_currentStGroup, out this._transformation);
            
            if (_currentStGroup == null)
                return;
            _splineInterpolator.UpdateSpline(this._currentStGroup.spline);
        }

        public void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                _started = true;
            }

            if (_started && !_paused)
            {
                _splineInterpolator.Step(1.5f * Time.deltaTime);
            }

            var pt = _splineInterpolator.Value();
            if(pt.HasValue)
            {
                this.transform.localPosition = _transformation.Transform(pt.Value.pt);
                var splineForward = pt.Value.firstDerivative.normalized;
                this._bezierRotation = Quaternion.FromToRotation(_lastSplineForward, splineForward) * this._bezierRotation;
                this.transform.localRotation = _worldRotation * _bezierRotation;
                _lastSplineForward = splineForward;
            }
            else
            {
                NextSubTrack();
                if (_currentStGroup != null)
                {
                    Update();
                }
            }
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            if (evt.rotatedInstance != this._currentStGroup.subTrack.instanceWrapper) return;

            switch (evt.type)
            {
                case BlockRotationEvent.EventType.Update:
                    this._transformation = evt.transformation;

                    this._forward = evt.transformation.TransformVector(Vector3.forward);
                    this._up = evt.transformation.TransformVector(Vector3.up);

                    this._worldRotation = Quaternion.LookRotation(_forward, _up);
                    this.transform.localRotation = _worldRotation * _bezierRotation;

                    //Update();
                    break;
                case BlockRotationEvent.EventType.BeforeRotation:
                    this._paused = true;
                    break;
                case BlockRotationEvent.EventType.AfterRotation:
                    this._paused = false;
                    break;
            }
        }
    }
}
