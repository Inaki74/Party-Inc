using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PartyInc
{
    namespace RR
    {
        [RequireComponent(typeof(Rigidbody))]
        public class Mono_Player_Controller_RR : MonoBehaviourPun
        {
            public delegate void ActionPlayerLost(int playerId, int score);
            public static event ActionPlayerLost onPlayerDied;

            [SerializeField] public Rigidbody Rb;
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
            public bool isGrounded;
            public float DistanceFromGround { get; private set; }

            public LayerMask whatIsGround;
            private float distanceGround = 0.3f;
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
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    return;
                }

                CheckForInput();
            }

            private void FixedUpdate()
            {
                float dist;
                isGrounded = CheckIfGrounded(out dist);
                DistanceFromGround = dist;

                if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

                
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

            private bool CheckIfGrounded(out float dist)
            {
                RaycastHit hit;

                if(Physics.Raycast(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y/10, 0f), transform.TransformDirection(Vector3.down), out hit, distanceGround, whatIsGround)) {
                    Debug.DrawRay(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down) * hit.distance, Color.red, 1f);
                    dist = hit.distance;
                    return true;
                }
                else
                {
                    Debug.DrawRay(transform.position - new Vector3(0f, Cc.bounds.extents.y - Cc.bounds.extents.y / 10, 0f), transform.TransformDirection(Vector3.down) * distanceGround, Color.green);
                    dist = 0f;
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
                Rb.AddForce(force * 5, ForceMode.Impulse);
                hasLost = true;

                photonView.RPC("RPC_InformPlayerLost", RpcTarget.Others);
                onPlayerDied?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, Mng_GameManager_RR.Current.currentJump);
            }

            [PunRPC]
            public void RPC_InformPlayerLost()
            {
                hasLost = true;
            }
        }
    }
}
