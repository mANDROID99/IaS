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
    public class TrainController : MonoBehaviour, EventConsumer<BlockRotationEvent>
    {
        private WorldContext gameContext;
        private int trackIndex;
        private BezierSpline.LinearInterpolator splineInterpolator;

        private SubTrackGroup currentSTGroup = null;
        private Transformation transformation = IdentityTransform.IDENTITY;
        private bool started = false;
        private bool paused = false;

        Vector3 forward = Vector3.forward;
        Vector3 up = Vector3.up;
        Vector3 lastSplineForward = Vector3.forward;

        Quaternion bezierRotation = Quaternion.identity;
        Quaternion worldRotation = Quaternion.identity;

        public static void AddToWorld(Transform parent, GameObject trainPrefab, WorldContext context, int trackIndex)
        {
            GameObject trainGo = GameObject.Instantiate(trainPrefab);
            TrainController train = trainGo.GetComponent<TrainController>();
            train.Init(context, trackIndex);
            GameObjectUtils.AsChildOf(parent, new Vector3(), trainGo);
        }

        private void Init(WorldContext gameContext, int trackIndex)
        {
            Debug.Log(gameContext);
            this.gameContext = gameContext;
            this.trackIndex = trackIndex;
            this.gameContext.eventRegistry.RegisterConsumer(this);
        }

        private void SetTrackPosition(Vector3 trackPosition, Vector3 forward)
        {
            this.transform.position = trackPosition;
        }

        public void Start()
        {
            TrackContext trackContext = gameContext.tracks[trackIndex];
            this.currentSTGroup = trackContext.splitTrack.firstTrackNode.group;
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
            splineInterpolator.UpdateSpline(this.currentSTGroup.spline);
        }

        public void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                started = true;
            }

            if (started && !paused)
            {
                splineInterpolator.Step(1.5f * Time.deltaTime);
            }

            BezierSpline.BezierPtInfo? pt = splineInterpolator.Value();
            if(pt.HasValue)
            {
                this.transform.localPosition = transformation.Transform(pt.Value.pt);
                Vector3 splineForward = pt.Value.firstDerivative.normalized;
                this.bezierRotation = Quaternion.FromToRotation(lastSplineForward, splineForward) * this.bezierRotation;
                this.transform.localRotation = worldRotation * bezierRotation;
                lastSplineForward = splineForward;
            }
            else
            {
                NextSubTrack();
                if (currentSTGroup != null)
                {
                    Update();
                }
            }


            /*BezierSpline.BezierPtInfo? pt = splineInterpolator.Value();
            if (pt.HasValue)
            {
                this.transform.localPosition = transformation.Transform(pt.Value.pt);
                Vector3 splineForward = transformation.TransformVector(pt.Value.firstDerivative.normalized);

                this.transform.localRotation = Quaternion.FromToRotation(transform.forward, splineForward) * transform.localRotation;
            }
            else
            {
                NextSubTrack();
                if (currentSTGroup != null)
                {
                    Update();
                }
            }*/
        }

        public void OnEvent(BlockRotationEvent evt)
        {
            if (evt.rotatedInstance == this.currentSTGroup.subTrack.instanceWrapper)
            {
                switch (evt.type)
                {
                    case BlockRotationEvent.EventType.Update:
                        this.transformation = evt.transformation;

                        this.forward = evt.transformation.TransformVector(Vector3.forward);
                        this.up = evt.transformation.TransformVector(Vector3.up);

                        this.worldRotation = Quaternion.LookRotation(forward, up);
                        this.transform.localRotation = worldRotation * bezierRotation;

                        //Update();
                        break;
                    case BlockRotationEvent.EventType.BeforeRotation:
                        this.paused = true;
                        break;
                    case BlockRotationEvent.EventType.AfterRotation:
                        this.paused = false;
                        break;
                }
            }
        }

    }
}
