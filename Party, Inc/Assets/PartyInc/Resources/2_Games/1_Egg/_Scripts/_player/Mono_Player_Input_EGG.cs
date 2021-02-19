using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun;

namespace PlayInc
{
    namespace EGG
    {
        public class Mono_Player_Input_EGG : MonoBehaviourPun
        {
            public int MovementDirection { get; private set; }
            public int lastCount;
            public int lastFingerID;
            private float widthMiddlePoint;
            private Vector3 lastTouchPosition = Vector3.zero;

            void Start()
            {
                widthMiddlePoint = Screen.width / 2;
            }

            private void Update()
            {
                if (Mng_GameManager_EGG.Current.isGameFinished)
                {
                    return;
                }

                DetermineLastTouchPosition();

                // TOUCH
                if (lastTouchPosition == Vector3.zero)
                {
                    MovementDirection = 0;
                }
                else if (CheckSideOfTouch(lastTouchPosition))
                {
                    MovementDirection = 1;
                }
                else
                {
                    MovementDirection = -1;
                }


                //// PC OSX
                //if (Input.GetKey(KeyCode.LeftArrow))
                //{
                //    MovementDirection = -1;
                //}
                //else if (Input.GetKey(KeyCode.RightArrow))
                //{
                //    MovementDirection = 1;
                //}
                //else
                //{
                //    MovementDirection = 0;
                //}
            }

            /// <summary>
            /// Checks if the touch was on the left or right side of the screen
            /// </summary>
            /// <param name="touchPosition"></param>
            /// <returns>True if the screen was touched in the right side, false if otherwise</returns>
            private bool CheckSideOfTouch(Vector3 touchPosition)
            {
                return touchPosition.x > widthMiddlePoint;
            }

            /// <summary>
            /// Gets the last touches position in the screen.
            /// </summary>
            private void DetermineLastTouchPosition()
            {
                if (Input.touchCount > 0)
                {

                    if (Input.touches[Input.touchCount - 1].phase == UnityEngine.TouchPhase.Began ||
                        Input.touches[Input.touchCount - 1].phase == UnityEngine.TouchPhase.Stationary ||
                        Input.touches[Input.touchCount - 1].phase == UnityEngine.TouchPhase.Moved)
                    {
                        lastTouchPosition = Input.touches[Input.touchCount - 1].position;
                    }
                    else
                    {
                        lastTouchPosition = Vector3.zero;
                    }
                }
            }
        }

    }
}
