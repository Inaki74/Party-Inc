using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    namespace RR
    {
        [RequireComponent(typeof(Rigidbody))]
        public class Player : MonoBehaviourPun, IPunObservable
        {
            public delegate void ActionPlayerLost(int playerId, int score);
            public static event ActionPlayerLost onPlayerDied;

            [SerializeField] private Rigidbody Rb;
            [SerializeField] private CapsuleCollider Cc;
            [SerializeField] private MeshRenderer Mr;

            [SerializeField] private float jumpForce;
            [SerializeField] private float jumpingDrag;
            [SerializeField] private Material mine;

            private float originalDrag;
            private float fallingDrag = -5;

            private bool cheatInput;
            private bool jumpInput;
            public bool hasLost;
            private bool isGrounded;

            public LayerMask whatIsGround;
            private float distanceGround = 0.3f;

            private Vector3 lastPos;
            private Vector3 lastVel;
            private Quaternion lastRot;
            private float lastDrag;
            private bool infoReceived = false;
            // Start is called before the first frame update
            void Start()
            {
                if(Rb == null) Rb = GetComponent<Rigidbody>();
                if (Cc == null) Cc = GetComponent<CapsuleCollider>();
                if (Mr == null) Mr = GetComponent<MeshRenderer>();

                jumpInput = false;
                hasLost = false;
                isGrounded = false;
                originalDrag = Rb.drag;

                if (photonView.IsMine) Mr.material = mine;
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected && infoReceived)
                {
                    transform.position = Vector3.Lerp(transform.position, lastPos, Time.deltaTime);
                    Rb.velocity = lastVel;
                    Rb.drag = lastDrag;
                    if (hasLost)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, lastRot, Time.deltaTime);
                    }
                    return;
                }

                CheckForInput();
            }

            private void FixedUpdate()
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                isGrounded = CheckIfGrounded();
                if ((!hasLost && jumpInput && isGrounded) || cheatInput)
                {
                    Jump();
                }

                if(!hasLost) DecideDrag();
            }

            private void CheckForInput()
            {
                if (Application.isMobilePlatform)
                {
                    if (Input.touchCount > 0)
                    {
                        jumpInput = true;
                    }
                    else
                    {
                        jumpInput = false;
                    }
                }
                else
                {
                    cheatInput = true;

                    jumpInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
                }
            }

            private void Jump()
            {
                Rb.velocity = Vector3.up * jumpForce;
            }

            private bool CheckIfGrounded()
            {
                RaycastHit hit;

                if(Physics.Raycast(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y/10, 0f), transform.TransformDirection(Vector3.down), out hit, distanceGround, whatIsGround)) {
                    Debug.DrawRay(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), Color.red, hit.distance);
                    return true;
                }
                else
                {
                    Debug.DrawRay(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down), Color.green, distanceGround);
                    return false;
                }
            }

            private void DecideDrag()
            {
                if (Rb.velocity.y < 0f)
                {
                    Rb.drag = fallingDrag;
                }
                else if (!hasLost && jumpInput)
                {
                    Rb.drag = jumpingDrag;
                }
                else
                {
                    Rb.drag = originalDrag;
                }
            }

            public void Die(Vector3 force)
            {
                if ((!photonView.IsMine && PhotonNetwork.IsConnected) || hasLost)
                {
                    return;
                }
                
                Rb.drag = 0f;
                Rb.constraints = RigidbodyConstraints.None;
                Rb.AddForce(force, ForceMode.Impulse);
                hasLost = true;

                photonView.RPC("RPC_InformPlayerLost", RpcTarget.Others);
                onPlayerDied?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameManager.Current.currentJump);
            }

            [PunRPC]
            public void RPC_InformPlayerLost()
            {
                hasLost = true;
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(Rb.velocity);
                    stream.SendNext(transform.position);
                    stream.SendNext(Rb.drag);
                    if(hasLost) stream.SendNext(transform.rotation);
                }
                else
                {
                    lastVel = (Vector3)stream.ReceiveNext();
                    lastPos = (Vector3)stream.ReceiveNext();
                    lastDrag = (float)stream.ReceiveNext();

                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    lastPos += lag * lastVel;

                    infoReceived = true;

                    if (hasLost)
                    {
                        Rb.constraints = RigidbodyConstraints.None;
                        lastRot = (Quaternion)stream.ReceiveNext();
                    }
                }
            }
        }
    }
}
