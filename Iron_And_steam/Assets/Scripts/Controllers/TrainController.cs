using Assets.Scripts.Controllers;
using IaS.Domain;
using IaS.GameState;
using IaS.Helpers;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Tracks;
using UnityEngine;

namespace IaS.GameObjects
{
    public class TrainController : EventConsumer<BlockRotationEvent>, Controller
    {
        private readonly GameObject _train;
        private readonly TrackConnectionResolver _trackConnectionResolver;
        private readonly int _trackIndex;

        private readonly BezierSpline.LinearInterpolator _splineInterpolator;

        private SubTrackGroup _currentStGroup = null;
        private Transformation _transformation = IdentityTransform.IDENTITY;
        private bool _started = false;
        private bool _paused = false;

        private Vector3 _forward = Vector3.forward;
        private Vector3 _up = Vector3.up;
        private Vector3 _lastSplineForward = Vector3.forward;

        private Quaternion _bezierRotation = Quaternion.identity;
        private Quaternion _worldRotation = Quaternion.identity;

        public TrainController(Transform parent, GameObject trainPrefab, EventRegistry eventRegistry, TrackConnectionResolver trackConnectionResolver, GroupContext groupContext, int trackIndex)
        {
            _train = InstantiateGameObject(parent, trainPrefab);
            _trackConnectionResolver = trackConnectionResolver;
            _trackIndex = trackIndex;
            eventRegistry.RegisterConsumer(this);

            TrackContext trackContext = groupContext.Tracks[_trackIndex];
            _currentStGroup = trackContext.SplitTrack.FirstSubTrack.FirstGroup;
            _splineInterpolator = _currentStGroup.spline.linearInterpolator();

            var startPos = _currentStGroup.spline.pts[0].startPos;
            SetTrackPosition(startPos);
        }

        private GameObject InstantiateGameObject(Transform parent, GameObject prefab)
        {
            return GameObjectUtils.AsChildOf(parent, new Vector3(), UnityEngine.Object.Instantiate(prefab));
        }

        private void SetTrackPosition(Vector3 trackPosition)
        {
            _train.transform.position = trackPosition;
        }

        private void NextSubTrack()
        {
            if (_currentStGroup == null)
                return;

            _currentStGroup = _trackConnectionResolver.GetNext(_currentStGroup, out _transformation);
            
            if (_currentStGroup == null)
                return;
            _splineInterpolator.UpdateSpline(_currentStGroup.spline);
        }

        public void Update(MonoBehaviour mono)
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                _started = true;
            }

            if (_started && !_paused)
            {
                _splineInterpolator.Step(1.5f * Time.deltaTime);
            }

            BezierSpline.BezierPtInfo? pt = _splineInterpolator.Value();
            if(pt.HasValue)
            {
                _train.transform.localPosition = _transformation.Transform(pt.Value.pt);
                Vector3 splineForward = pt.Value.firstDerivative.normalized;
                _bezierRotation = Quaternion.FromToRotation(_lastSplineForward, splineForward) * _bezierRotation;
                _train.transform.localRotation = _worldRotation * _bezierRotation;
                _lastSplineForward = splineForward;
            }
            else
            {
                NextSubTrack();
                if (_currentStGroup != null)
                {
                    Update(mono);
                }
            }
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            if (evt.rotatedInstance != _currentStGroup.subTrack.InstanceWrapper) return;

            switch (evt.type)
            {
                case BlockRotationEvent.EventType.Update:
                    _transformation = evt.transformation;

                    _forward = evt.transformation.TransformVector(Vector3.forward);
                    _up = evt.transformation.TransformVector(Vector3.up);

                    _worldRotation = Quaternion.LookRotation(_forward, _up);
                    _train.transform.localRotation = _worldRotation * _bezierRotation;

                    //Update();
                    break;
                case BlockRotationEvent.EventType.BeforeRotation:
                    _paused = true;
                    break;
                case BlockRotationEvent.EventType.AfterRotation:
                    _paused = false;
                    break;
            }
        }
    }
}
