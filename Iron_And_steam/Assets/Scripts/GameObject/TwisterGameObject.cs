using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder;
using IaS.Helpers;

namespace IaS.GameObjects{

    public class TwisterGameObject : MonoBehaviour {

        public class TwisterGameObjectBuilder
        {
            private Split[] splits;
            private InstanceWrapper[] instances;
            private Transform parent;

            public TwisterGameObjectBuilder With(Split[] splits, InstanceWrapper[] instances, Transform parent)
            {
                this.splits = splits;
                this.instances = instances;
                this.parent = parent;
                return this;
            }

            public GameObject Build(){
                TwisterGameObject splitController;
                GameObject splitControllerGameObj = GameObjectUtils.GameObjectScript<TwisterGameObject>("twister", parent, new Vector3(), out splitController);
                splitController.InitFor(splits, instances);
                return splitControllerGameObj;
            }
        }

        private SplitHalfRotation[] splitHalfRotations;
        private TwisterWrapper[] allRubixInstances;
        private bool readyToRot = true;

        public void InitFor(Split[] splits, InstanceWrapper[] instances)
        {
            this.allRubixInstances = instances.Select(instance => new TwisterWrapper(instance)).ToArray();
            splitHalfRotations = splits.SelectMany(split => {
                return new SplitHalfRotation[]{
                    new SplitHalfRotation(split, true),
                    new SplitHalfRotation(split, false)}; 
            }).ToArray();
        }

        IEnumerator Rotate90Degrees(int direction, SplitHalfRotation splitRotation)
        {
            int nMeshBlocks = splitRotation.instances.Count;
            Quaternion[] startRotations = new Quaternion[nMeshBlocks];
            Quaternion[] endRotations = new Quaternion[nMeshBlocks];

            Quaternion rotateAmt = Quaternion.Euler(splitRotation.split.axis * 90 * direction);
            for(int i = 0; i < nMeshBlocks; i++){
                TwisterWrapper twisted = splitRotation.instances[i];
                startRotations[i] = twisted.rotation;
                endRotations[i] = rotateAmt * startRotations[i];
                twisted.rotation = endRotations[i];
                twisted.bounds.SetToRotationFrom(endRotations[i], splitRotation.split.pivot, twisted.originalBounds);
            }

            float startTime = Time.time;
            float deltaTime;
            do
            {
                deltaTime = Time.time - startTime;
                for (int i = 0; i < nMeshBlocks; i++)
                {
                    Quaternion lerpRotation = Quaternion.Lerp(startRotations[i], endRotations[i], Mathf.SmoothStep(0, 1, deltaTime));
                    TwisterWrapper twisted = splitRotation.instances[i];
                    Vector3 position = MathHelper.RotateAroundPivot(twisted.originalBounds.Position, splitRotation.split.pivot, lerpRotation);
                    twisted.instance.gameObject.transform.localRotation = lerpRotation;
                    twisted.instance.gameObject.transform.localPosition = position;
                }

                yield return null;
            } while (deltaTime <= 1);
            readyToRot = true;
        }

        private bool CanRotate(SplitHalfRotation halfSplit)
        {
            halfSplit.resetMeshBlocks();
            foreach (TwisterWrapper instance in allRubixInstances)
            {
                float constrainResult = halfSplit.split.Constrains(halfSplit.lhs, instance.bounds);
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

        void Update()
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
                    foreach (SplitHalfRotation splitRotation in splitHalfRotations)
                    {
                        int w = 
                            rotX != 0 && splitRotation.split.axis.Equals(Vector3.right) ? rotX :
                            rotY != 0 && splitRotation.split.axis.Equals(Vector3.up) ? rotY :
                            rotZ != 0 && splitRotation.split.axis.Equals(Vector3.forward) ? rotZ : 0;

                        if ((w != 0) && (splitRotation.lhs) && CanRotate(splitRotation))
                        {
                            readyToRot = false;
                            StartCoroutine(Rotate90Degrees(w, splitRotation));
						    break;
                        }
                    }
                }
            }
        }

        private class TwisterWrapper
        {
            internal InstanceWrapper instance;
            internal Quaternion rotation;
            internal BlockBounds originalBounds;
            internal BlockBounds bounds;

            internal TwisterWrapper(InstanceWrapper instance)
            {
                this.instance = instance;
                this.originalBounds = instance.bounds;
                this.bounds = instance.bounds.Copy();
                this.rotation = Quaternion.identity;
            }
        }


        private class SplitHalfRotation
        {
            internal Split split;
            internal int rotation;
            internal bool lhs;
            internal List<TwisterWrapper> instances = new List<TwisterWrapper>();

            internal SplitHalfRotation(Split split, bool isLhs)
            {
                this.split = split;
                this.rotation = 0;
                this.lhs = isLhs;
            }

            internal void resetMeshBlocks()
            {
                instances = new List<TwisterWrapper>();
            }
        }
    }
}
