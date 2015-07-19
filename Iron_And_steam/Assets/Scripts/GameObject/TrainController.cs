using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.Helpers;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Splines;
using IaS.GameState;

namespace IaS.GameObjects
{
    public class TrainController : MonoBehaviour, InstanceEventHandler
    {
        private GameContext gameContext;
        private int trackIndex;
        private BezierSpline.LinearInterpolator splineInterpolator;

        private SubTrackGroup currentSTGroup = null;
        private Transformation transformation = IdentityTransform.IDENTITY;
        private Vector3 lastForward;
        private bool started = false;

        public static void AddToWorld(Transform parent, GameObject trainPrefab, GameContext context, int trackIndex)
        {
            GameObject trainGo = GameObject.Instantiate(trainPrefab);
            TrainController train = trainGo.GetComponent<TrainController>();
            train.Init(context, trackIndex);
            GameObjectUtils.AsChildOf(parent, new Vector3(), trainGo);
        }

        private void Init(GameContext gameContext, int trackIndex)
        {
            Debug.Log(gameContext);
            this.gameContext = gameContext;
            this.trackIndex = trackIndex;
        }

        private void SetTrackPosition(Vector3 trackPosition, Vector3 forward)
        {
            this.transform.position = trackPosition;
            this.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, forward);
        }

        public void Start()
        {
            TrackContext trackContext = gameContext.tracks[trackIndex];
            this.currentSTGroup = trackContext.splitTrack.firstTrackNode.group;
            this.currentSTGroup.subTrack.instanceWrapper.eventHandlers.Add(this);
            this.splineInterpolator = currentSTGroup.spline.linearInterpolator();

            Vector3 startPos = currentSTGroup.spline.pts[0].startPos;
            SetTrackPosition(startPos, currentSTGroup[0].forward);
        }

        private void NextSubTrack()
        {
            if (currentSTGroup == null)
                return;

            SubTrackGroup lastSTGroup = this.currentSTGroup;
            TrackContext trackContext = gameContext.tracks[trackIndex];
            this.currentSTGroup = trackContext.connections.GetNext(currentSTGroup, out this.transformation);
            
            if (currentSTGroup == null)
                return;


            lastSTGroup.subTrack.instanceWrapper.eventHandlers.Remove(this);
            this.currentSTGroup.subTrack.instanceWrapper.eventHandlers.Add(this);
            splineInterpolator.UpdateSpline(this.currentSTGroup.spline);
        }

        public void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                started = true;
            }

            if (started)
            {
                BezierSpline.BezierPtInfo? pt = splineInterpolator.Step(1.5f * Time.deltaTime);

                if (pt.HasValue)
                {
                    this.transform.localPosition = transformation.Transform(pt.Value.pt);

                    Vector3 splineForward = transformation.TransformVector(pt.Value.firstDerivative.normalized);
                    this.transform.localRotation = Quaternion.FromToRotation(lastForward, splineForward) * transform.localRotation;
                    lastForward = splineForward;
                }
                else
                {
                    NextSubTrack();
                    if (currentSTGroup != null)
                    {
                        Update();
                    }
                }
            }
        }

        public void OnUpdateTransform(InstanceWrapper instance, Transformation transform)
        {
            this.transformation = transform;
        }

        public void OnEndTransform(InstanceWrapper instance, Transformation transform) { }

    }
}
