﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace DD
    {
        public class Mono_Player_Input_DD : MonoBehaviourPun
        {
            public delegate void ActionTakeMove(int numberMove);
            public static event ActionTakeMove onMoveMade;

            private int[] sequenceGenerated = new int[12];

            private int currentMoves = 0;
            private int presentMove;

            private float timeStationary;
            private float timeout;

            public Vector2 fingerMovement;

            private bool inputAllowed;
            private bool runOnce;

            #region Unity Callbacks

            void Start()
            {
                timeout = Mng_GameManager_DD.Current.timeForInput;
                inputAllowed = true;
                runOnce = true;
            }

            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                timeout -= Time.deltaTime;
                if (currentMoves != Mng_GameManager_DD.Current.amountOfMovesThisRound && timeout > 0)
                {
                    if (inputAllowed && Input.touchCount > 0) TakeInput();
                }
                else if (runOnce)
                {
                    // Took all possible moves or out of time, transition phase
                    runOnce = false;
                    Mng_GameManager_DD.Current.NotifyOfRemotePlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
                }
            }

            private void OnEnable()
            {
                timeout = Mng_GameManager_DD.Current.timeForInput;
            }

            private void OnDisable()
            {
                inputAllowed = true;
                runOnce = true;
                currentMoves = 0;
                presentMove = 0;
                timeStationary = 0;
                fingerMovement = Vector2.zero;
            }

            #endregion

            /// <summary>
            /// Returns the current sequence that was determined on this round.
            /// </summary>
            /// <returns></returns>
            public int[] GetCurrentSequence()
            {
                return sequenceGenerated;
            }

            public void ResetCurrentSequence()
            {
                for(int i = 0; i < sequenceGenerated.Length; i++)
                {
                    sequenceGenerated[i] = 0;
                }
            }

            #region Private Functions

            /// <summary>
            /// Saves the move made and activates the onMoveMade event.
            /// </summary>
            private void TakeMove()
            {
                sequenceGenerated[currentMoves] = presentMove;
                onMoveMade?.Invoke(currentMoves);
                currentMoves++;

                StartCoroutine(SwipeCooldownCo());
            }

            /// <summary>
            /// Function that takes in the touch input.
            /// </summary>
            private void TakeInput()
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    fingerMovement = Vector2.zero;
                }

                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    fingerMovement += Input.touches[0].deltaPosition;
                }

                if (Input.touches[0].phase == TouchPhase.Stationary)
                {
                    timeStationary += Time.fixedDeltaTime;
                }
                else
                {
                    timeStationary = 0;
                }

                if (Input.touches[0].phase == TouchPhase.Ended || timeStationary > 0.2f)
                {
                    if (Mathf.Abs(fingerMovement.x) > 200f || Mathf.Abs(fingerMovement.y) > 200f)
                    {
                        inputAllowed = false;
                        presentMove = ManageInput();
                        TakeMove();
                    }
                }
            }

            /// <summary>
            /// Interprets the swipe and returns the result. (1 is right, 2 is up, 3 is left and 4 is down)
            /// </summary>
            /// <returns></returns>
            private int ManageInput()
            {
                if (Mathf.Abs(fingerMovement.x) > Mathf.Abs(fingerMovement.y))
                {
                    if (fingerMovement.x > 0)
                    {
                        //Debug.Log("You swiped right");
                        return 1;
                    }
                    else
                    {
                        //Debug.Log("You swiped left");
                        return 3;
                    }
                }
                else
                {
                    if (fingerMovement.y > 0)
                    {
                        //Debug.Log("You swiped up");
                        return 2;
                    }
                    else
                    {
                        //Debug.Log("You swiped down");
                        return 4;
                    }
                }
            }

            /// <summary>
            /// The input cooldown coroutine.
            /// </summary>
            /// <returns></returns>
            private IEnumerator SwipeCooldownCo()
            {
                yield return new WaitForSeconds(0.2f);
                inputAllowed = true;
            }

            #endregion

            //private int TakeInputPC()
            //{
            //    if (Input.GetKeyDown(KeyCode.RightArrow))
            //    {
            //        return 1;
            //    }
            //    else
            //    if (Input.GetKeyDown(KeyCode.UpArrow))
            //    {
            //        return 2;
            //    }
            //    else
            //    if (Input.GetKeyDown(KeyCode.LeftArrow))
            //    {
            //        return 3;
            //    }
            //    else
            //    if (Input.GetKeyDown(KeyCode.DownArrow))
            //    {
            //        return 4;
            //    }
            //}
        }
    }
}
