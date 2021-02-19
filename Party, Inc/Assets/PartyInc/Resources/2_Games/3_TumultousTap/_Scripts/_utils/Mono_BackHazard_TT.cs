using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace PartyInc
{
    namespace TT
    {
        public class Mono_BackHazard_TT : MonoBehaviourPun
        {
            [SerializeField] private Rigidbody Rb;
            [SerializeField] private GameObject cameraFocalPoint;

            private Rigidbody[] playersRB;
            private Transform[] playersTransforms;

            private Vector3 currentVelocity;

            private bool gameStarted;
            private bool reachedMinimumSpeed;

            private Vector3 firstPos;
            private float k;

            // Start is called before the first frame update
            void Start()
            {
                if (Rb == null) Rb = GetComponent<Rigidbody>();
                gameStarted = false;
                currentVelocity = Vector3.zero;

                cameraFocalPoint.transform.localPosition = Vector3.forward * Mng_GameManager_TT.maxDistance * 4;
            }

            private void Update()
            {
                if (gameStarted)
                {
                    //var aux = playersTransforms.OrderByDescending(t => t.position.z);

                    //firstPos = aux.First().position;

                    //k = Vector3.Distance(gameObject.transform.position, firstPos);

                    //cameraFocalPoint.gameObject.transform.position = gameObject.transform.position;
                    //UpdateCameraFoci(gameObject.transform.position, firstPos);
                }
                
            }

            void FixedUpdate()
            {
                if (gameStarted)
                {
                    var aux = playersTransforms.OrderByDescending(t => t.position.z);

                    if(playersTransforms.Length > 0)
                    {
                        firstPos = aux.First().position;

                        k = Vector3.Distance(gameObject.transform.position, firstPos);

                        UpdateVelocity();
                    }
                }

                Rb.velocity = currentVelocity;
            }

            private void Awake()
            {
                Mng_GameManager_TT.onGameStart += OnGameStart;
                Mng_GameManager_TT.onGameFinish += OnGameFinished;
            }

            private void OnDestroy()
            {
                Mng_GameManager_TT.onGameStart -= OnGameStart;
                Mng_GameManager_TT.onGameFinish -= OnGameFinished;
            }

            private void GetPlayersInformation()
            {
                Mono_Player_TT[] aux = FindObjectsOfType<Mono_Player_TT>();
                playersRB = GetPlayersRigidBodies(aux);
                playersTransforms = GetPlayersTransforms(aux);
            }

            private Rigidbody[] GetPlayersRigidBodies(Mono_Player_TT[] aux)
            {
                Rigidbody[] ret = new Rigidbody[aux.Length];

                for(int i = 0; i < aux.Length; i++)
                {
                    ret[i] = aux[i].GetRigidbody();
                }

                return ret;
            }

            private Transform[] GetPlayersTransforms(Mono_Player_TT[] aux)
            {
                Transform[] ret = new Transform[aux.Length];

                for (int i = 0; i < aux.Length; i++)
                {
                    ret[i] = aux[i].GetTransform();
                }

                return ret;
            }

            private void UpdateVelocity()
            {
                var aux = playersRB.OrderByDescending(r => r.velocity.magnitude);

                Vector3 firstVel = aux.First().velocity;

                
                if (k > Mng_GameManager_TT.maxDistance)
                {
                    currentVelocity = Vector3.Lerp(firstVel, Rb.velocity, Time.fixedDeltaTime);
                }
                else if (k == Mng_GameManager_TT.maxDistance)
                {
                    currentVelocity = firstVel + firstVel / 100;
                }
                else
                {
                    if (reachedMinimumSpeed)
                    {
                       currentVelocity = Mng_GameManager_TT.hazardMinimumVelocity * Mng_GameManager_TT.forwardVector;
                    }
                    else
                    {
                       currentVelocity += (Mng_GameManager_TT.hazardMinimumVelocity * Mng_GameManager_TT.forwardVector) * Time.fixedDeltaTime / 5;

                       if (currentVelocity.magnitude >= (Mng_GameManager_TT.hazardMinimumVelocity * Mng_GameManager_TT.forwardVector).magnitude)
                       {
                           reachedMinimumSpeed = true;
                       }
                    }
                }
            }

            private void UpdateCameraFoci(Vector3 hazardPos, Vector3 firstPos)
            {
                Vector3 dist = firstPos - hazardPos;

                Vector3 aux = firstPos;

                aux.x = 0f;

                Debug.Log("Camera focal pos:" + (cameraFocalPoint.gameObject.transform.position.magnitude - aux.magnitude));

                cameraFocalPoint.gameObject.transform.position = aux;
            }

            private void OnGameStart()
            {
                gameStarted = true;
                GetPlayersInformation();
            }

            private void OnGameFinished()
            {
                gameStarted = false;
                currentVelocity = Vector3.zero;
            }
        }
    }
}


