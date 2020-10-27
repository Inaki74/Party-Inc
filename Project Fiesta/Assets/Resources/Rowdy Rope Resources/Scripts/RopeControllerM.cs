using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace RR
    {
        /// <summary>
        /// A rope thats managed mathematically in opposition to physics.
        /// </summary>
        public class RopeControllerM : MonoBehaviourPun, IPunObservable
        {
            public delegate void ActionLoopCompleted();
            public static event ActionLoopCompleted onLoopComplete;

            [SerializeField] private GameObject linkPrefab;
            [SerializeField] private GameObject startPoint;
            [SerializeField] private GameObject endPoint;

            [SerializeField] private float separation = 1.4f;

            private RopeLinkM[] ropeLinks;

            private float distanceStartEnd;
            private float startingAngle;

            private bool gameStarted;
            private bool firstRun;
            public bool loopCompleted = true;



            public float angle;
            public float rotationSpeed = 1f;
            public float lastRotationSign;

            #region Unity Callbacks

            // Update is called once per frame
            void Update()
            {
                if (gameStarted)
                {
                    if (firstRun)
                    {
                        rotationSpeed = GameManager.Current.startingSpeed;
                        firstRun = false;
                    }

                    Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 5f), Color.red);
                    angle += Time.deltaTime * rotationSpeed;

                    if (angle > Mathf.PI * 2)
                    {
                        loopCompleted = false;
                        angle = 0f;
                    }

                    if (angle < Mathf.PI && angle > 175f * Mathf.Deg2Rad && rotationSpeed < 0f)
                    {
                        loopCompleted = false;
                    }

                    if (angle < 0f)
                    {
                        loopCompleted = false;
                        angle = Mathf.PI * 2;
                    }

                    if (rotationSpeed < 0f && angle < Mathf.PI / 4 && !loopCompleted)
                    {
                        loopCompleted = true;
                        onLoopComplete?.Invoke();
                    }
                    if (rotationSpeed > 0f && angle > 3 * Mathf.PI / 4 && !loopCompleted)
                    {
                        loopCompleted = true;
                        onLoopComplete?.Invoke();
                    }
                }

            }

            private void OnEnable()
            {
                firstRun = true;
                gameStarted = false;

                distanceStartEnd = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);

                startingAngle = GameManager.Current.startingAngle;
                angle = startingAngle;
                rotationSpeed = 0f;
                InstantiateRope();
            }

            private void Awake()
            {
                GameManager.onGameStart += OnGameStart;
                GameManager.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                GameManager.onGameStart -= OnGameStart;
                GameManager.onGameFinish -= OnGameFinish;
            }

            #endregion

            private void InstantiateRope()
            {
                // Instantiate the objects and store them in respective arrays.
                // Position objects
                float z = 0;
                int i = 0;
                List<RopeLinkM> ropeAux = new List<RopeLinkM>();
                while (z <= distanceStartEnd)
                {
                    GameObject GO = Instantiate(linkPrefab);

                    GO.transform.parent = transform;

                    float radius = Mathf.Pow(z, 2) / 40 - z / 2;

                    ropeAux.Add(GO.GetComponent<RopeLinkM>().Init(this, radius, z, i, startingAngle));

                    z += separation;
                    i++;
                }

                ropeLinks = ropeAux.ToArray();
            }

            #region Mechanic Functions

            /// <summary>
            /// The public function that calls the rope inversion.
            /// </summary>
            /// <param name="where"></param>
            public void InvertRope(float where)
            {
                StartCoroutine(InvertCo(where));
            }

            /// <summary>
            /// The function waits until the rope has reached a fair place to invert.
            /// </summary>
            /// <param name="where"></param>
            /// <returns></returns>
            private IEnumerator InvertCo(float where)
            {
                if (rotationSpeed > 0f)
                    yield return new WaitUntil(() => angle > where && angle < where + 10f * Mathf.Deg2Rad);
                else if (rotationSpeed < 0f)
                    yield return new WaitUntil(() => angle < where && angle > where - 10f * Mathf.Deg2Rad);

                rotationSpeed *= -1;
                loopCompleted = true;
                photonView.RPC("RPC_SendInversion", RpcTarget.Others);
            }

            /// <summary>
            /// The public function that calls the rope speed burst.
            /// </summary>
            /// <param name="where"></param>
            public void BurstRope(float amount)
            {
                StartCoroutine(BurstCo(amount));
            }

            /// <summary>
            /// The function waits until the rope has reached a fair place to speed up.
            /// </summary>
            /// <param name="where"></param>
            /// <returns></returns>
            private IEnumerator BurstCo(float amount)
            {
                if (rotationSpeed > 0f)
                    yield return new WaitUntil(() => angle > 100f * Mathf.Deg2Rad && angle < 100f * Mathf.Deg2Rad + 10f * Mathf.Deg2Rad);
                else if (rotationSpeed < 0f)
                    yield return new WaitUntil(() => angle < 80f * Mathf.Deg2Rad && angle > 80f * Mathf.Deg2Rad - 10f * Mathf.Deg2Rad);

                rotationSpeed += amount * Mathf.Sign(rotationSpeed);
                photonView.RPC("RPC_SendBurst", RpcTarget.Others, amount);
            }

            #endregion

            #region Event Functions

            private void OnGameStart()
            {
                gameStarted = true;
            }

            private void OnGameFinish()
            {
                gameStarted = false;
                //loopCompleted = true;
                rotationSpeed = 0f;
            }

            #endregion

            #region Photon Sync

            [PunRPC]
            public void RPC_SendInversion()
            {
                rotationSpeed *= -1;
                loopCompleted = true;
            }

            [PunRPC]
            public void RPC_SendBurst(float amount)
            {
                rotationSpeed += amount * Mathf.Sign(rotationSpeed);
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    if(gameStarted && GameManager.Current.MyPlayerHasLost())
                        stream.SendNext(rotationSpeed);
                }
                else
                {
                    if (gameStarted && GameManager.Current.MyPlayerHasLost())
                        rotationSpeed = (float)stream.ReceiveNext();
                }
            }

            #endregion
        }
    }
}


