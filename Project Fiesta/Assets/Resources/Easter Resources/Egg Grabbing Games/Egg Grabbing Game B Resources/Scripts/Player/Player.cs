using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace EGG
    {
        public class Player : MonoBehaviourPun, IPunObservable
        {
            #region Components
            [SerializeField]
            public Camera MainCamera { get; private set; }
            private PlayerInputManager InputManager;
            private PlayerPreferencesManager playerUI;
            private MeshRenderer Mr;
            private BoxCollider Bc;
            #endregion

            public float movementSpeed;
            public float stunTime;

            private PlayerResults<int> myResults;
            public PlayerResults<int> MyResults {
                get
                {
                    return myResults;
                }
                private set
                {
                    myResults = value;
                }
            }

            #region Instance Variables
            private Vector3 railLeft;
            private Vector3 railMiddle;
            private Vector3 railRight;

            private Vector3 moveToVector;

            private bool runOnce;
            private bool isStunned;
            #endregion

            #region Unity Callback Functions
            private void Start()
            {
                if (InputManager == null)
                {
                    InputManager = GetComponent<PlayerInputManager>();
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
                foreach(PlayerPreferencesManager p in FindObjectsOfType<PlayerPreferencesManager>())
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

                    myResults.playerId = PhotonNetwork.LocalPlayer.ActorNumber;
                    myResults.scoring = 0;
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
                EasterEgg.onObtainEgg += OnEggObtain;
                GameManager.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                EasterEgg.onObtainEgg -= OnEggObtain;
                GameManager.onGameFinish -= OnGameFinish;
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
                if (!photonView.IsMine) return;

                if (scoreModifier == -1)
                {
                    StartCoroutine("StunnedCo");
                }
                myResults.scoring += scoreModifier;
                playerUI.UpdateScore(myResults.scoring);
            }

            public bool GetIfStunned()
            {
                return isStunned;
            }

            public void OnGameFinish()
            {
                if (photonView.IsMine) GameManager.Current.isHighScore = GeneralHelperFunctions.DetermineHighScoreInt(FiestaTime.Constants.EGG_KEY_HISCORE, MyResults.scoring, true);

                StopAllCoroutines();
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(myResults.playerId);
                    stream.SendNext(myResults.scoring);
                }
                else
                {
                    PlayerResults<int> aux = new PlayerResults<int>();

                    aux.playerId = (int)stream.ReceiveNext();
                    aux.scoring = (int)stream.ReceiveNext();

                    myResults = aux;
                }
            }
        }

    }
}
