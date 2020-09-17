using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace EGG
    {
        public class Player : MonoBehaviourPun
        {
            #region Components
            [SerializeField]
            public Camera MainCamera { get; private set; }
            private PlayerInputManager InputManager;
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

            #region Unity Callback Functions
            private void Start()
            {
                InputManager = GetComponent<PlayerInputManager>();
                MainCamera = FindObjectOfType<Camera>();
                Bc = GetComponent<BoxCollider>();
                Mr = GetComponent<MeshRenderer>();

                if (!photonView.IsMine) return;

                railMiddle = transform.position;
                railRight = railMiddle + Vector3.right * 2;
                railLeft = railMiddle + Vector3.left * 2;

                runOnce = true;
                isStunned = false;

                transform.position = railMiddle;
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
                EasterEgg.onObtainEgg += OnRottenObtain;
            }

            private void OnDestroy()
            {
                EasterEgg.onObtainEgg -= OnRottenObtain;
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

            private void OnRottenObtain(int scoreModifier)
            {
                if (!photonView.IsMine) return;

                if (scoreModifier == -1)
                {
                    StartCoroutine("StunnedCo");
                }
            }

            public bool GetIfStunned()
            {
                return isStunned;
            }
        }

    }
}
