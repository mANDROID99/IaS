﻿using Assets.Scripts.Controllers;
using IaS.Domain;
using IaS.GameState;
using IaS.GameState.TrackConnections;
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

        private InterpolateableConnection _currentConnection = null;
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
            _currentConnection = _trackConnectionResolver.GetStartingConnection(trackContext);
            _currentConnection.Step(0);
        }

        private GameObject InstantiateGameObject(Transform parent, GameObject prefab)
        {
            return GameObjectUtils.AsChildOf(parent, new Vector3(), UnityEngine.Object.Instantiate(prefab));
        }

        private void SetTrackPosition(Vector3 trackPosition)
        {
            _train.transform.position = trackPosition;
        }

        public void Update(MonoBehaviour mono)
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                _started = true;
            }

            if (_started && !_paused)
            {
                _currentConnection.Step(1.5f * Time.deltaTime);
                if (_currentConnection.ReachedEnd)
                {
                    _currentConnection = _trackConnectionResolver.GetNext(_currentConnection, out _transformation);
                    _currentConnection.Step(0);
                }
            }
            
            _train.transform.localPosition = _transformation.Transform(_currentConnection.CurrentPos);
            Vector3 splineForward = _currentConnection.CurrentForward;
            _bezierRotation = Quaternion.FromToRotation(_lastSplineForward, splineForward) * _bezierRotation;
            _train.transform.localRotation = _worldRotation * _bezierRotation;
            _lastSplineForward = splineForward;
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            if (evt.rotatedInstance != _currentConnection.WrappedInstance) return;

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
