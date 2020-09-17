using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        /// <summary>
        /// In charge of all things player, its animations, its input management and its synchronization over the network.
        /// </summary>
        public class Player : MonoBehaviour
        {
            public int health = 3;

            private int[] currentSequence;

            public delegate void ActionShowMove(bool rightMove, int moveNumber);
            public static event ActionShowMove onShowMove;

            public delegate void ActionWrongMove(int health);
            public static event ActionWrongMove onWrongMove;

            [SerializeField] private Text playerName;
            [SerializeField] private GameObject demonstrationUIHolder;

            [SerializeField] private InputManager inputManager;
            [SerializeField] private MeshRenderer Mr;
            [SerializeField] private Animator anim;

            public bool hasLost = false;
            // Start is called before the first frame update
            void Start()
            {
                if (inputManager == null) inputManager = GetComponent<InputManager>();
                if (anim == null) anim = GetComponent<Animator>();
                if (Mr == null) Mr = GetComponent<MeshRenderer>();

                playerName.text = PhotonNetwork.NickName;
            }

            private void Awake()
            {
                GameManager.onNextPhase += OnPhaseTransit;
            }

            private void OnDestroy()
            {
                GameManager.onNextPhase -= OnPhaseTransit;
            }

            private void OnPhaseTransit(int nextPhase)
            {
                if (hasLost) {
                    GameManager.Current.NotifyOfPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
                    return;
                }

                if(nextPhase == 1)
                {
                    // Entered input section of loop
                    inputManager.enabled = true;
                }
                else if(inputManager.enabled){
                    currentSequence = inputManager.GetCurrentSequence();
                    inputManager.enabled = false;
                }

                if(nextPhase == 2)
                {
                    //Demonstration phase
                    StartCoroutine(DemonstrationCo());
                }

                if(nextPhase == 3)
                {
                    if(health <= 0)
                    {
                        hasLost = true;
                        inputManager.enabled = false;

                        // Check if theres atleast two players standing
                        // If true
                        // The game goes on, you as a spectator TODO: Prompt the player to stay or leave.
                        // else
                        // The game ends abruptly, the last one standing wins.
                    }

                    GameManager.Current.NotifyOfPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
                }

                // If necessary, we can deactivate mesh renderer / animations when leaving their view. 
            }

            private IEnumerator DemonstrationCo()
            {
                for(int i = 0; i < GameManager.Current.amountOfMovesThisRound; i++)
                {
                    // Show animation (change color for now)
                    switch (currentSequence[i])
                    {
                        case 1:
                            Mr.material.color = new Color(1, 0, 0);
                            break;
                        case 2:
                            Mr.material.color = new Color(0, 1, 0);
                            break;
                        case 3:
                            Mr.material.color = new Color(0, 0, 1);
                            break;
                        case 4:
                            Mr.material.color = new Color(1, 1, 0);
                            break;
                        default:
                            break;
                    }

                    // Check if move was right or wrong
                    bool isRight = currentSequence[i] == GameManager.Current.sequenceMap[i];

                    if (!isRight) {
                        health--;
                        onWrongMove?.Invoke(health);
                    }

                    // Send notification to UI
                    onShowMove?.Invoke(isRight, i);

                    yield return new WaitForSeconds(1f);
                }

                Mr.material.color = new Color(1, 1, 1);

                GameManager.Current.NotifyOfPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }
}


