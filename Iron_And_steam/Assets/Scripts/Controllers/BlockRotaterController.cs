using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.GameState;
using IaS.GameState.Events;
using IaS.GameState.WorldTree;
using IaS.Helpers;
using IaS.Scripts.Domain;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.GameObjects{

    public class BlockRotaterController : Controller {

        private readonly HalfSplitRotation[] _splitHalfRotations;
        private readonly EventRegistry _eventRegistry;
        private readonly GroupBranch _groupBranch;
        private bool _readyToRot = true;
        
        public BlockRotaterController(EventRegistry eventRegistry, Split[] splits, GroupBranch groupBranch)
        {
            _eventRegistry = eventRegistry;
            _groupBranch = groupBranch;
            _splitHalfRotations = splits.SelectMany(split => new[]{
                new HalfSplitRotation(split, true),
                new HalfSplitRotation(split, false)}).ToArray();
        }

        IEnumerator Rotate90Degrees(int direction, HalfSplitRotation splitRotation)
        {
            float startTime = Time.time;
            float deltaTime;
            do
            {
                deltaTime = Mathf.Clamp(Time.time - startTime, 0, 1);
                foreach (SplitBoundsBranch branch in splitRotation.Branches)
                {
                    BranchRotation rotation = branch.Data.BranchRotation;

                    Quaternion lerpRotation = Quaternion.Lerp(rotation.StartRotation, rotation.EndRotation, Mathf.SmoothStep(0, 1, deltaTime));
                    Transformation transformation = new RotateAroundPivotTransform(splitRotation.Split.Pivot, lerpRotation);

                    branch.Node.transform.localRotation = lerpRotation;

                    Vector3 position = transformation.Transform(new Vector3());
                    branch.Node.transform.localPosition = position;
                    //_eventRegistry.Notify(new BlockRotationEvent(rotated, transformation, BlockRotationEvent.EventType.Update));

//                    if (deltaTime == 1)
//                    {
//                        _eventRegistry.Notify(new BlockRotationEvent(rotated, transformation, BlockRotationEvent.EventType.AfterRotation));
//                    }
                }

                yield return null;
            } while (deltaTime < 1);

            _eventRegistry.Notify(new GameEvent(GameEvent.Type.RESUMED));
            _readyToRot = true;
        }
        
        private bool CanRotate(HalfSplitRotation halfSplit)
        {
            halfSplit.ResetRotatedBranches();
            return _groupBranch.SplitBounds.All(bounds =>
            {
                SplitBoundsBranch branch = _groupBranch.GetSplitBoundsBranch(bounds);
                Split.ConstraintResult constraintResult;
                float value = halfSplit.Split.Constrains(true, branch.Data.BranchRotation.RotatedBounds, out constraintResult);

                if (constraintResult == Split.ConstraintResult.Blocked)
                {
                    return false;
                }

                if (value >= 0)
                {
                    halfSplit.Branches.Add(branch);
                }
                return true;
            });
        }

        private void UpdateBranchRotation(int direction, HalfSplitRotation splitRotation)
        {
            Quaternion rotateAmt = Quaternion.Euler(splitRotation.Split.Axis * 90 * direction);
            foreach (SplitBoundsBranch rotated in splitRotation.Branches)
            {
                BranchRotation rotation = rotated.BranchRotation;
                rotation.StartRotation = rotation.EndRotation;
                rotation.EndRotation = rotateAmt * rotation.EndRotation;

                rotation.RotatedBounds.SetToRotationFrom(rotation.EndRotation, splitRotation.Split.Pivot, rotated.OriginalBounds);
                Transformation transform = new RotateAroundPivotTransform(splitRotation.Split.Pivot, rotation.EndRotation);
                _eventRegistry.Notify(new BlockRotationEvent(rotated.OriginalBounds, transform, BlockRotationEvent.EventType.BeforeRotation));
            }
        }

        public void Update(MonoBehaviour mono)
        {
            int rotX = 0;
            int rotY = 0;
            int rotZ = 0;
            if (_readyToRot)
            {
                rotX = Input.GetKey(KeyCode.S) ? -1 : rotX;
                rotX = Input.GetKey(KeyCode.W) ? 1 : rotX;
                rotY = Input.GetKey(KeyCode.A) ? 1 : rotY;
                rotY = Input.GetKey(KeyCode.D) ? -1 : rotY;
                rotZ = Input.GetKey(KeyCode.E) ? -1 : rotZ;
                rotZ = Input.GetKey(KeyCode.Q) ? 1 : rotZ;

                if ((rotX != 0) || (rotY != 0) || (rotZ != 0))
                {
                    foreach (HalfSplitRotation splitRotation in _splitHalfRotations)
                    {
                        int w = 
                            rotX != 0 && splitRotation.Split.Axis.Equals(Vector3.right) ? rotX :
                            rotY != 0 && splitRotation.Split.Axis.Equals(Vector3.up) ? rotY :
                            rotZ != 0 && splitRotation.Split.Axis.Equals(Vector3.forward) ? rotZ : 0;

                        if ((w != 0) && (splitRotation.Lhs) && CanRotate(splitRotation))
                        {
                            _readyToRot = false;
                            _eventRegistry.Notify(new GameEvent(GameEvent.Type.PAUSED));
                            UpdateBranchRotation(w, splitRotation);
                            mono.StartCoroutine(Rotate90Degrees(w, splitRotation));
						    break;
                        }
                    }
                }
            }
        }

        private class HalfSplitRotation
        {
            internal readonly Split Split;
            internal int Rotation;
            internal readonly bool Lhs;
            internal List<SplitBoundsBranch> Branches = new List<SplitBoundsBranch>();

            internal HalfSplitRotation(Split split, bool isLhs)
            {
                Split = split;
                Rotation = 0;
                Lhs = isLhs;
            }

            internal void ResetRotatedBranches()
            {
                Branches = new List<SplitBoundsBranch>();
            }
        }
    }

    public struct BlockRotationEvent : IEvent
    {
        public enum EventType
        {
            Update, BeforeRotation, AfterRotation
        }

        public readonly BlockBounds RotatedBounds;
        public readonly Transformation Transformation;
        public readonly EventType Type;

        public BlockRotationEvent(BlockBounds rotatedBounds, Transformation transformation, EventType type)
        {
            RotatedBounds = rotatedBounds;
            Transformation = transformation;
            Type = type;
        }
    }
}
