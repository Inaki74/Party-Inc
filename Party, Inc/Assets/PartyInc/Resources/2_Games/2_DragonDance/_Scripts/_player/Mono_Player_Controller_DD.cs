using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace PlayInc
{
    namespace DD
    {
        /// <summary>
        /// In charge of all things player, its animations, its input management and its synchronization over the network.
        /// </summary>
        public class Mono_Player_Controller_DD : MonoBehaviourPun, IPunObservable
        {
            public int health;
            public PlayerResults<int> myResults;

            private int[] currentSequence;

            #region Events

            public delegate void ActionShowMove(bool rightMove, int moveNumber);
            public static event ActionShowMove onShowMove;

            public delegate void ActionWrongMove(int health);
            public static event ActionWrongMove onWrongMove;

            #endregion

            #region Serialized Components

            [SerializeField] private Mono_Player_Input_DD inputManager;
            [SerializeField] private MeshRenderer Mr;
            [SerializeField] private Animator anim;

            [SerializeField] private Text playerName;
            [SerializeField] private GameObject demonstrationUIHolder;

            #endregion

            public bool hasLost = false;

            #region Unity Callbacks

            // Start is called before the first frame update
            void Start()
            {
                if (inputManager == null) inputManager = GetComponent<Mono_Player_Input_DD>();
                if (anim == null) anim = GetComponent<Animator>();
                if (Mr == null) Mr = GetComponent<MeshRenderer>();

                if (photonView.IsMine)
                {
                    myResults.playerId = PhotonNetwork.LocalPlayer.ActorNumber;
                    myResults.scoring = 0;
                    health = Mng_GameManager_DD.Current.playersHealth;
                    playerName.text = PhotonNetwork.NickName;
                    photonView.RPC("RPC_SendName", RpcTarget.Others, PhotonNetwork.NickName);
                } 
            }

            private void Awake()
            {
                Mng_GameManager_DD.onNextPhase += OnPhaseTransit;
            }

            private void OnDestroy()
            {
                Mng_GameManager_DD.onNextPhase -= OnPhaseTransit;
            }

            #endregion

            /// <summary>
            /// Function run when the "next phase" event is triggered.
            /// </summary>
            /// <param name="nextPhase">The int representing the next phase</param>
            private void OnPhaseTransit(int nextPhase)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                if (nextPhase == 0) return;

                if(nextPhase == 1)
                {
                    if (hasLost) { Mng_GameManager_DD.Current.NotifyOfRemotePlayerReady(PhotonNetwork.LocalPlayer.ActorNumber); return; }
                    // Entered input section of loop
                    inputManager.enabled = true;
                }
                else if(inputManager.enabled){
                    currentSequence = inputManager.GetCurrentSequence();
                    inputManager.enabled = false;
                }

                if(nextPhase == 2)
                {
                    if (hasLost) { Debug.Log("Player notify finish"); Mng_GameManager_DD.Current.NotifyOfLocalPlayerReady(); return; }
                    //Demonstration phase
                    StartCoroutine(DemonstrationCo());
                }

                if(nextPhase == 3)
                {
                    if(health <= 0 && !hasLost)
                    {
                        hasLost = true;
                        inputManager.enabled = false;

                        Mng_GameManager_DD.Current.NotifyOfPlayerLost(PhotonNetwork.LocalPlayer.ActorNumber);
                    }

                    Mng_GameManager_DD.Current.NotifyOfRemotePlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
                }

                if(nextPhase == 4)
                {
                    // Game finished
                    inputManager.enabled = false;
                    if(photonView.IsMine) Mng_GameManager_DD.Current.isHighScore = HighScoreHelpers.DetermineHighScoreInt(Constants.DD_KEY_HISCORE, myResults.scoring, true);
                }
            }

            /// <summary>
            /// The Coroutine that takes care of the demonstration of the movements of each player.
            /// </summary>
            /// <returns></returns>
            private IEnumerator DemonstrationCo()
            {
                for(int i = 0; i < Mng_GameManager_DD.Current.amountOfMovesThisRound; i++)
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
                    bool isRight = currentSequence[i] == Mng_GameManager_DD.Current.sequenceMap[i];

                    if(health > 0)
                    {
                        if (!isRight)
                        {
                            health--;
                            onWrongMove?.Invoke(health);
                        }
                        else
                        {
                            myResults.scoring++;
                        }
                    }
                    
                    // Send notification to UI
                    onShowMove?.Invoke(isRight, i);

                    yield return new WaitForSeconds(1f);
                }

                inputManager.ResetCurrentSequence();
                Mr.material.color = new Color(1, 1, 1);

                //Mng_GameManager_DD.Current.NotifyOfRemotePlayerReady(PhotonNetwork.LocalPlayer.ActorNumber);
                Mng_GameManager_DD.Current.NotifyOfLocalPlayerReady();
            }

            #region PUN

            [PunRPC]
            public void RPC_SendName(string name)
            {
                playerName.text = name;
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(new Vector3(Mr.material.color.r, Mr.material.color.g, Mr.material.color.b));
                    stream.SendNext(myResults.playerId);
                    stream.SendNext(myResults.scoring);
                }
                else
                {
                    Vector3 temp = (Vector3)stream.ReceiveNext();
                    Mr.material.color = new Color(temp.x, temp.y, temp.z);

                    PlayerResults<int> aux = new PlayerResults<int>();

                    aux.playerId = (int)stream.ReceiveNext();
                    aux.scoring = (int)stream.ReceiveNext();

                    myResults = aux;
                }
            }

            #endregion
        }
    }
}


