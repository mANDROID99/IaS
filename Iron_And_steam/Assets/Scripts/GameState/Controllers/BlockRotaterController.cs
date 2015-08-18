using System.Collections;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameState.Rotation;
using IaS.Domain.WorldTree;
using IaS.GameState;
using IaS.GameState.Events;
using IaS.GameState.Rotation;
using IaS.Helpers;
using IaS.WorldBuilder;
using UnityEngine;

namespace IaS.GameObjects{

    public class BlockRotaterController : Controller
    {

        private readonly TreeRotater _treeRotater;
        private readonly SplitSide[] _splitSides;
        private EventRegistry _eventRegistry;
        private bool _readyToRot = true;

        public BlockRotaterController(LevelTree levelTree, EventRegistry eventRegistry, Split[] splits)
        {
            _eventRegistry = eventRegistry;
            _treeRotater = new TreeRotater(levelTree, eventRegistry);
            _splitSides = splits.SelectMany(split => split.GetActiveSides(levelTree)).ToArray();
        }

        public void Update(MonoBehaviour mono, GlobalGameState gameState)
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
                bool alternate = Input.GetKey(KeyCode.LeftShift);

                if ((rotX != 0) || (rotY != 0) || (rotZ != 0))
                {
                    foreach (SplitSide splitRotation in _splitSides)
                    {
                        int w =
                            rotX != 0 && splitRotation.Axis.Equals(Vector3.right) ? rotX :
                            rotY != 0 && splitRotation.Axis.Equals(Vector3.up) ? rotY :
                            rotZ != 0 && splitRotation.Axis.Equals(Vector3.forward) ? rotZ : 0;

                        if ((w != 0) && (splitRotation.Lhs != alternate))
                        {
                            TreeRotater.Direction direction = TreeRotater.DirectionFromInt(w);
                            RotationAnimator[] animators = _treeRotater.Rotate(splitRotation, direction);

                            _readyToRot = false;
                            _eventRegistry.Notify(new GameEvent(GameEvent.Type.PAUSED));
                            mono.StartCoroutine(Rotate90Degrees(splitRotation, animators));
                            //break;
                        }
                    }
                }
            }
        }

        IEnumerator Rotate90Degrees(SplitSide splitRotation, RotationAnimator[] animators)
        {
            float startTime = Time.time;
            float deltaTime;
            do
            {
                deltaTime = Mathf.Clamp(Time.time - startTime, 0, 1);
                foreach (RotationAnimator animator in animators)
                {
                    Quaternion lerpRotation = Quaternion.Lerp(animator.From, animator.To, Mathf.SmoothStep(0, 1, deltaTime));
                    Transformation transformation = new RotateAroundPivotTransform(splitRotation.Pivot, lerpRotation);
                    Vector3 localPosition = transformation.Transform(new Vector3());

                    animator.UpdateTransform(lerpRotation, localPosition);
                }

                yield return null;
            } while (deltaTime < 1);

            _eventRegistry.Notify(new GameEvent(GameEvent.Type.RESUMED));
            _readyToRot = true;
        }

    }

    /*private readonly HalfSplitRotation[] _splitHalfRotations;
        private readonly EventRegistry _eventRegistry;
        private readonly GroupBranch _groupBranch;
        private bool _readyToRot = true;
        
        public BlockRotaterController(EventRegistry eventRegistry, Split[] splits, GroupBranch groupBranch)
        {
            _eventRegistry = eventRegistry;
            _groupBranch = groupBranch;

            _splitHalfRotations = splits.SelectMany(split => split.GetActiveSides().Select(side => new HalfSplitRotation(split, side))).ToArray();
        }

        IEnumerator Rotate90Degrees(int direction, HalfSplitRotation splitRotation)
        {
            float startTime = Time.time;
            float deltaTime;
            do
            {
                deltaTime = Mathf.Clamp(Time.time - startTime, 0, 1);
                foreach (RotateableBranch branch in splitRotation.Branches)
                {
                    RotationState rotation = branch.Data.RotationState;

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
            return _groupBranch.RotateableBounds.All(bounds =>
            {
                RotateableBranch branch = _groupBranch.GetRotateableBranch(bounds);
                Split.ConstraintResult constraintResult;
                float value = halfSplit.Split.Constrains(halfSplit.Lhs, branch.Data.RotationState.RotatedBounds, out constraintResult);

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
            foreach (RotateableBranch rotated in splitRotation.Branches)
            {
                RotationState rotation = rotated.BranchRotation;
                rotation.StartRotation = rotation.EndRotation;
                rotation.EndRotation = rotateAmt * rotation.EndRotation;

                rotation.RotatedBounds.SetToRotationFrom(rotation.EndRotation, splitRotation.Split.Pivot, rotated.OriginalBounds);
                Transformation transform = new RotateAroundPivotTransform(splitRotation.Split.Pivot, rotation.EndRotation);
                _eventRegistry.Notify(new BlockRotationEvent(rotated.OriginalBounds, transform, BlockRotationEvent.EventType.BeforeRotation));
            }
        }

        public void Update(MonoBehaviour mono, GlobalGameState gameState)
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
                bool alternate = Input.GetKey(KeyCode.LeftShift);

                if ((rotX != 0) || (rotY != 0) || (rotZ != 0))
                {
                    foreach (HalfSplitRotation splitRotation in _splitHalfRotations)
                    {
                        int w = 
                            rotX != 0 && splitRotation.Split.Axis.Equals(Vector3.right) ? rotX :
                            rotY != 0 && splitRotation.Split.Axis.Equals(Vector3.up) ? rotY :
                            rotZ != 0 && splitRotation.Split.Axis.Equals(Vector3.forward) ? rotZ : 0;

                        if ((w != 0) && (splitRotation.Lhs != alternate) && CanRotate(splitRotation))
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
            internal readonly bool Lhs;
            internal List<RotateableBranch> Branches = new List<RotateableBranch>();

            internal HalfSplitRotation(Split split, bool lhs)
            {
                Split = split;
                Rotation = 0;
                Lhs = lhs;
            }

            internal void ResetRotatedBranches()
            {
                Branches = new List<RotateableBranch>();
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
    }*/
}
