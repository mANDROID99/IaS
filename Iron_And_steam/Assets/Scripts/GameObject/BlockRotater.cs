using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder;
using IaS.GameState;
using IaS.Helpers;

namespace IaS.GameObjects{

    public class BlockRotater {

        private WorldContext world;
        private HalfSplitRotation[] splitHalfRotations;
        private InstanceWrapper[] allInstances;
        private bool readyToRot = true;

        public BlockRotater(WorldContext world, Split[] splits, InstanceWrapper[] instances)
        {
            this.world = world;
            this.allInstances = instances;
            splitHalfRotations = splits.SelectMany(split =>
            {
                return new HalfSplitRotation[]{
                    new HalfSplitRotation(split, true),
                    new HalfSplitRotation(split, false)};
            }).ToArray();
        }

        IEnumerator Rotate90Degrees(int direction, HalfSplitRotation splitRotation)
        {
            float startTime = Time.time;
            float deltaTime;
            do
            {
                deltaTime = Mathf.Clamp(Time.time - startTime, 0, 1);
                foreach (InstanceWrapper rotated in splitRotation.instances)
                {
                    Quaternion lerpRotation = Quaternion.Lerp(rotated.startRotation, rotated.endRotation, Mathf.SmoothStep(0, 1, deltaTime));
                    Transformation transformation = new RotateAroundPivotTransform(splitRotation.split.pivot, lerpRotation);

                    Vector3 position = transformation.Transform(rotated.bounds.Position);
                    rotated.gameObject.transform.localRotation = lerpRotation;
                    rotated.gameObject.transform.localPosition = position;
                    world.eventRegistry.Notify(new BlockRotationEvent(rotated, transformation, BlockRotationEvent.EventType.Update));

                    if(deltaTime == 1)
                    {
                        world.eventRegistry.Notify(new BlockRotationEvent(rotated, transformation, BlockRotationEvent.EventType.AfterRotation));
                    }
                }

                yield return null;
            } while (deltaTime < 1);
            readyToRot = true;
        }

        private bool CanRotate(HalfSplitRotation halfSplit)
        {
            halfSplit.resetMeshBlocks();
            foreach (InstanceWrapper instance in allInstances)
            {
                float constrainResult = halfSplit.split.Constrains(halfSplit.lhs, instance.rotatedBounds);
                if (constrainResult == Split.CONSTRAIN_BLOCKED)
                {
                    return false;
                }else if (constrainResult >= 0)
                {
                    halfSplit.instances.Add(instance);
                }
            }
            return true;
        }

        private void UpdateInstanceRotation(int direction, HalfSplitRotation splitRotation)
        {
            Quaternion rotateAmt = Quaternion.Euler(splitRotation.split.axis * 90 * direction);
            foreach (InstanceWrapper rotated in splitRotation.instances)
            {
                rotated.startRotation = rotated.endRotation;
                rotated.endRotation = rotateAmt * rotated.endRotation;
                rotated.rotatedBounds.SetToRotationFrom(rotated.endRotation, splitRotation.split.pivot, rotated.bounds);

                Transformation transform = new RotateAroundPivotTransform(splitRotation.split.pivot, rotated.endRotation);
                world.eventRegistry.Notify(new BlockRotationEvent(rotated, transform, BlockRotationEvent.EventType.BeforeRotation));
            }
        }

        public void Update(MonoBehaviour goContext)
        {
            int rotX = 0;
            int rotY = 0;
            int rotZ = 0;
            if (readyToRot)
            {
                rotX = Input.GetKey(KeyCode.S) ? -1 : rotX;
                rotX = Input.GetKey(KeyCode.W) ? 1 : rotX;
                rotY = Input.GetKey(KeyCode.A) ? 1 : rotY;
                rotY = Input.GetKey(KeyCode.D) ? -1 : rotY;
                rotZ = Input.GetKey(KeyCode.E) ? -1 : rotZ;
                rotZ = Input.GetKey(KeyCode.Q) ? 1 : rotZ;

                if ((rotX != 0) || (rotY != 0) || (rotZ != 0))
                {
                    foreach (HalfSplitRotation splitRotation in splitHalfRotations)
                    {
                        int w = 
                            rotX != 0 && splitRotation.split.axis.Equals(Vector3.right) ? rotX :
                            rotY != 0 && splitRotation.split.axis.Equals(Vector3.up) ? rotY :
                            rotZ != 0 && splitRotation.split.axis.Equals(Vector3.forward) ? rotZ : 0;

                        if ((w != 0) && (splitRotation.lhs) && CanRotate(splitRotation))
                        {
                            readyToRot = false;
                            UpdateInstanceRotation(w, splitRotation);
                            goContext.StartCoroutine(Rotate90Degrees(w, splitRotation));
						    break;
                        }
                    }
                }
            }
        }

        private class HalfSplitRotation
        {
            internal Split split;
            internal int rotation;
            internal bool lhs;
            internal List<InstanceWrapper> instances = new List<InstanceWrapper>();

            internal HalfSplitRotation(Split split, bool isLhs)
            {
                this.split = split;
                this.rotation = 0;
                this.lhs = isLhs;
            }

            internal void resetMeshBlocks()
            {
                instances = new List<InstanceWrapper>();
            }
        }
    }

    public struct BlockRotationEvent : IEvent
    {
        public enum EventType
        {
            Update, BeforeRotation, AfterRotation
        }

        public InstanceWrapper rotatedInstance;
        public Transformation transformation;
        public EventType type;

        public BlockRotationEvent(InstanceWrapper rotatedInstance, Transformation transformation, EventType type)
        {
            this.rotatedInstance = rotatedInstance;
            this.transformation = transformation;
            this.type = type;
        }
    }
}
