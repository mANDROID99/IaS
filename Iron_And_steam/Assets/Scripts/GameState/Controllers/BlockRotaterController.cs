using System.Collections;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameState.Rotation;
using IaS.World.WorldTree;
using IaS.GameState;
using IaS.GameState.Events;
using IaS.GameState.Rotation;
using IaS.Domain;
using IaS.Helpers;
using UnityEngine;
using System.Collections.Generic;

namespace IaS.GameObjects{

    public class BlockRotaterController : Controller
    {

        private readonly TreeRotater _treeRotater;
        private readonly SplitSide[] _splitSides;
        private EventRegistry _eventRegistry;
        private bool _readyToRot = true;

        public BlockRotaterController(LevelTree levelTree, EventRegistry eventRegistry)
        {
            _eventRegistry = eventRegistry;
            _treeRotater = new TreeRotater(levelTree, eventRegistry);
            _splitSides = levelTree.Level.AllSplits.SelectMany(split => split.GetActiveSides(levelTree)).ToArray();
        }

        public void Update(MonoBehaviour mono, UpdateState gameState)
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
                            IList<RotationAnimator> animators = _treeRotater.Rotate(splitRotation, direction);

                            _readyToRot = false;
                            _eventRegistry.Notify(new GameEvent(GameEvent.Type.PAUSED));
                            mono.StartCoroutine(Rotate90Degrees(splitRotation, animators));
                            //break;
                        }
                    }
                }
            }
        }

        IEnumerator Rotate90Degrees(SplitSide splitRotation, IList<RotationAnimator> animators)
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
}
