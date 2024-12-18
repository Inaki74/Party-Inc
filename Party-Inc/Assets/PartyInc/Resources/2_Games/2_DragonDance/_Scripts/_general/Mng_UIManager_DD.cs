﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace PartyInc
{
    namespace DD
    {
        public class Mng_UIManager_DD : MonoBehaviour
        {
            private bool startup = true;

            #region UI GameObjects

            [SerializeField] private GameObject showSequenceUIHolder;
            [SerializeField] private GameObject playerInputUIHolder;
            [SerializeField] private GameObject finishUIHolder;
            private Mono_PlayerInput_UI_DD playerInputUI;

            #endregion

            [SerializeField] private Text countdownText;
            [SerializeField] private Text finishText;

            [SerializeField] private float countdown;

            private Mono_Player_Controller_DD myPlayer;

            #region Unity Callbacks
            // Start is called before the first frame update
            void Start()
            {
                playerInputUI = playerInputUIHolder.GetComponent<Mono_PlayerInput_UI_DD>();
                FindMyPlayer();

                countdownText.enabled = true;
                StartCoroutine(UIHelper.ShowCountdownCo(this, countdownText, Mng_GameManager_DD.Current.countdownGameStart));
            }

            private void Awake()
            {
                Mng_GameManager_DD.onNextPhase += OnPhaseTransit;
                Mono_Player_Input_DD.onMoveMade += OnInputTaken;
            }

            private void OnDestroy()
            {
                Mng_GameManager_DD.onNextPhase -= OnPhaseTransit;
                Mono_Player_Input_DD.onMoveMade -= OnInputTaken;
            }
            #endregion

            /// <summary>
            /// Finds the player object bound to the local player.
            /// </summary>
            private void FindMyPlayer()
            {
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (g.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        myPlayer = g.GetComponent<Mono_Player_Controller_DD>();
                    }
                }
            }

            private IEnumerator FinishGameCo()
            {
                finishText.enabled = true;
                yield return new WaitForSeconds(2f);
                finishText.enabled = false;

                finishUIHolder.SetActive(true);
            }

            #region Event Functions

            /// <summary>
            /// Function run when the player gives input to the game.
            /// </summary>
            /// <param name="number">The int representing the move.</param>
            private void OnInputTaken(int number)
            {
                playerInputUI.TriggerInputIndicator(number);
            }

            /// <summary>
            /// Function run when the "next phase" event is triggered.
            /// </summary>
            /// <param name="nextPhase">The int representing the next phase</param>
            private void OnPhaseTransit(int nextPhase)
            {
                Debug.Log("1 OnPhaseTransit, UIManager: phase " + nextPhase);
                // 0 -> entered showSeq, 1 -> entered playerInput, 2 -> entered demoSeq, 3 -> entered results
                if (!startup)
                {
                    if (myPlayer.hasLost && nextPhase != 0 && nextPhase != 4) return;
                }

                startup = false;

                Debug.Log("2 OnPhaseTransit, UIManager: phase " + nextPhase);

                switch (nextPhase)
                {
                    case 0:
                        // Activate all pieces of Showing Sequence phase
                        showSequenceUIHolder.SetActive(true);

                        break;
                    case 1:
                        // Deactivate all pieces of Showing Sequence phase
                        showSequenceUIHolder.SetActive(false);

                        // Activate all pieces of Input Phase phase
                        playerInputUIHolder.SetActive(true);

                        break;
                    case 2:
                        // Deactivate all pieces of Input phase
                        playerInputUIHolder.SetActive(false);

                        break;
                    case 4:
                        // Case 3 irrelevant to UIManager, relevant to PlayerUI
                        // Game finished
                        Debug.Log("Received invocation");
                        StartCoroutine("FinishGameCo");
                        break;
                    default:
                        Debug.Log("ITS NOT POSSIBLEEEEE!");
                        break;
                }
            }

            #endregion
        }
    }
}

