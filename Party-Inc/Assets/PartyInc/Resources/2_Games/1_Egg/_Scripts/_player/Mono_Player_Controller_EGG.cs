using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

namespace PartyInc
{
    namespace EGG
    {
        public class Mono_Player_Controller_EGG : MonoBehaviourPun, IPunObservable, IPlayerResultSender
        {
            #region Components
            [SerializeField]
            public Camera MainCamera { get; private set; }
            private Mono_Player_Input_EGG InputManager;
            private Mono_Player_Preferences_EGG playerUI;
            private MeshRenderer Mr;
            private BoxCollider Bc;
            #endregion
            
            public float movementSpeed;
            public float stunTime;

            #region Instance Variables
            private Vector3 railLeft;
            private Vector3 railMiddle;
            private Vector3 railRight;

            private Vector3 moveToVector;

            private bool runOnce;
            private bool isStunned;
            #endregion

            private int _score;

            #region Unity Callback Functions
            private void Start()
            {
                if (InputManager == null)
                {
                    InputManager = GetComponent<Mono_Player_Input_EGG>();
                }
                if (MainCamera == null)
                {
                    MainCamera = FindObjectOfType<Camera>();
                }
                if (Bc == null)
                {
                    Bc = GetComponent<BoxCollider>();
                }
                if (Mr == null)
                {
                    Mr = GetComponent<MeshRenderer>();
                }
                foreach(Mono_Player_Preferences_EGG p in FindObjectsOfType<Mono_Player_Preferences_EGG>())
                {
                    if (p.photonView.IsMine)
                    {
                        playerUI = p;
                        break;
                    }
                }

                if (photonView.IsMine)
                {
                    railMiddle = transform.position;
                    railRight = railMiddle + Vector3.right * 1.5f;
                    railLeft = railMiddle + Vector3.left * 1.5f;
                    transform.position = railMiddle;

                    runOnce = true;
                    isStunned = false;
                }
            }

            private void FixedUpdate()
            {
                if (!photonView.IsMine)
                {
                    return;
                }

                if (runOnce && !isStunned)
                {
                    runOnce = false;
                    StartCoroutine(MoveToCo(movementSpeed, InputManager.MovementDirection));
                }
                else if (isStunned)
                {
                    StartCoroutine(MoveToCo(movementSpeed, 0));
                }
            }

            private void Awake()
            {
                PhotonNetwork.NetworkingClient.EventReceived += SendMyResults;
                Mono_Egg_EGG.onObtainEgg += OnEggObtain;
                Mng_GameManager_EGG.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= SendMyResults;
                Mono_Egg_EGG.onObtainEgg -= OnEggObtain;
                Mng_GameManager_EGG.onGameFinish -= OnGameFinish;
            }
            #endregion

            #region CoRoutine Functions
            /// <summary>
            /// Moves the player to the desired position
            /// </summary>
            private IEnumerator MoveToCo(float velocity, float direction)
            {
                if (direction > 0f)
                {
                    moveToVector = railRight;
                }
                else if (direction < 0f)
                {
                    moveToVector = railLeft;
                }
                else moveToVector = railMiddle;

                Bc.enabled = false;
                while (!CheckIfReachedPosition(moveToVector))
                {
                    transform.position = Vector3.MoveTowards(transform.position, moveToVector, velocity * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }

                if (!isStunned)
                    Bc.enabled = true;

                yield return new WaitForEndOfFrame();
                runOnce = true;
            }

            /// <summary>
            /// A Coroutine that waits to trigger if stunned.
            /// </summary>
            /// <returns></returns>
            private IEnumerator StunnedCo()
            {
                isStunned = true;
                Bc.enabled = false;
                yield return new WaitForSeconds(stunTime);
                Bc.enabled = true;
                isStunned = false;
            }
            #endregion

            /// <summary>
            /// Check if the player has reached a certain vector3 position.
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            private bool CheckIfReachedPosition(Vector3 v)
            {
                return transform.position.x == v.x;
            }

            private void OnEggObtain(int scoreModifier)
            {
                /// ESTO
                if (!photonView.IsMine) return;

                if (scoreModifier == -1)
                {
                    StartCoroutine("StunnedCo");
                }
                _score++;
                playerUI.UpdateScore(_score);
            }

            public bool GetIfStunned()
            {
                return isStunned;
            }

            public void OnGameFinish()
            {
                if (photonView.IsMine) Mng_GameManager_EGG.Current.IsHighScore = HighScoreHelpers.DetermineHighScoreInt(PartyInc.Constants.EGG_KEY_HISCORE, _score, true);

                StopAllCoroutines();
            }

            public void SendMyResults(EventData eventData)
            {
                if (eventData.Code == 74 && photonView.IsMine)
                {
                    int finalScore = _score;
                    bool isInt = true;

                    object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, finalScore, isInt };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(75, content, raiseEventOptions, SendOptions.SendReliable);
                }
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(isStunned);
                    stream.SendNext(transform.position);
                }
                else
                {
                    isStunned = (bool)stream.ReceiveNext();
                    transform.position = (Vector3)stream.ReceiveNext();
                }
            }
        }

    }
}
