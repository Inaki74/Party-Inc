using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace TT
    {
        [RequireComponent(typeof(Rigidbody))]
        public class PlayerController : MonoBehaviourPun, IPunObservable
        {
            [SerializeField] private Rigidbody Rb;
            [SerializeField] private MeshRenderer Mr;

            public Material mine;

            private Vector3 lastPos;
            private Vector3 lastVel;
            private bool isActive = true;

            // Boolean that is true in the frame that you tapped.
            private bool tap;
            private bool bufferedTap;
            private bool infoReceived;

            [SerializeField] private float tapForce;

            // Start is called before the first frame update
            void Start()
            {
                if(Rb == null) Rb = GetComponent<Rigidbody>();
                if (Mr == null) Mr = GetComponent<MeshRenderer>();

                if(photonView.IsMine) Mr.material = mine;
            }

            // Update is called once per frame
            void Update()
            {
                if(!photonView.IsMine && PhotonNetwork.IsConnected && infoReceived)
                {
                    transform.position = Vector3.Lerp(transform.position, lastPos, Time.deltaTime);
                    Rb.velocity = lastVel;
                    return;
                }

                if (GameManager.Current.gameBegan)
                {
                    CheckForInput();
                    CheckForInputPC();
                }
            }

            private void LateUpdate()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                if (GameManager.Current.gameBegan) { CheckForInput(); CheckForInputPC(); }
            }

            private void FixedUpdate()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                if (tap || bufferedTap)
                {
                    MoveForward();

                    if (tap && bufferedTap)
                    {
                        tap = false;
                    }else if (bufferedTap)
                    {
                        bufferedTap = false;
                    }
                    else
                    {
                        tap = false;
                    }
                }
            }

            private void OnDisable()
            {
                Rb.velocity = Vector3.zero;
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                if (collision.gameObject.tag == "DeathPlane")
                {
                    gameObject.SetActive(false);
                    photonView.RPC("RPC_Disable", RpcTarget.All);
                }
            }

            private void MoveForward()
            {
                Rb.AddForce(tapForce * GameManager.forwardVector, ForceMode.Impulse);
            }

            private void CheckForInput()
            {
                if(Input.touchCount > 0)
                {
                    if (tap) bufferedTap = true;
                    else tap = Input.touches[0].phase == TouchPhase.Began;
                }
            }

            private void CheckForInputPC()
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (tap) bufferedTap = true;
                    else tap = true;
                }
            }

            public Rigidbody GetRigidbody()
            {
                return Rb;
            }

            public Transform GetTransform()
            {
                return gameObject.transform;
            }

            [PunRPC]
            public void RPC_Disable()
            {
                gameObject.SetActive(false);
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(transform.position);
                    stream.SendNext(Rb.velocity);
                }
                else
                {
                    lastPos = (Vector3)stream.ReceiveNext();
                    lastVel = (Vector3)stream.ReceiveNext();

                    infoReceived = true;
                }
            }
        }
    }
}


