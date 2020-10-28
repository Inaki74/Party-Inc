using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace EGG
    {
        /// <summary>
        /// The falling egg, in charge of itself.
        /// </summary>
        public class EasterEgg : MonoBehaviourPun, IPunObservable
        {
            #region Events
            public delegate void ActionObtain(int score);
            public static event ActionObtain onObtainEgg;

            public delegate void ActionSpawn(int score);
            public static event ActionSpawn onSpawnEgg;
            #endregion

            #region Egg Specifics
            private int scoreModifier;
            public EggType eggType;
            public enum EggType
            {
                normal,
                rotten,
                golden
            }
            #endregion

            #region Inspector Assignables
            public LayerMask whatIsGround;

            [SerializeField] private Material normalMaterial;
            [SerializeField] private Material rottenMaterial;
            [SerializeField] private Material goldenMaterial;
            #endregion

            #region Components
            //private MeshRenderer Mr;
            private Animator anim;
            private Rigidbody Rb;
            public SphereCollider Sc;
            #endregion

            #region Unity Callbacks
            private void Awake()
            {
                Sc = GetComponent<SphereCollider>();
                anim = GetComponent<Animator>();
                Rb = GetComponent<Rigidbody>();

                SetEgg(eggType);
            }

            private void OnEnable()
            {
                photonView.RPC("RPC_SendActive", RpcTarget.Others, true);
                Sc.enabled = true;
                Rb.useGravity = true;
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<Player>().GetIfStunned())
                {
                    Sc.enabled = false;
                    Rb.useGravity = false;
                    Rb.velocity = Vector3.zero;
                    if (photonView.IsMine) OnObtain();
                }

                if (collision.gameObject.tag == "Ground" || (collision.gameObject.tag == "Egg" && collision.rigidbody.position.y < 2f))
                {
                    Sc.enabled = false;
                    Rb.useGravity = false;
                    Rb.velocity = Vector3.zero;
                    if (photonView.IsMine) OnBreak();
                }
            }
            #endregion

            /// <summary>
            /// Sets the eggs settings depending of which type of egg it is.
            /// </summary>
            /// <param name="t"></param>
            private void SetEgg(EggType t)
            {
                switch (t)
                {
                    case EggType.normal:
                        //Mr.material = normalMaterial;
                        scoreModifier = 1;
                        break;
                    case EggType.rotten:
                        //Mr.material = rottenMaterial;
                        scoreModifier = -1;
                        break;
                    case EggType.golden:
                        //Mr.material = goldenMaterial;
                        scoreModifier = 3;
                        break;
                }
            }

            /// <summary>
            /// What happens when an egg is obtained.
            /// </summary>
            private void OnObtain()
            {
                if (scoreModifier != -1)
                    onSpawnEgg?.Invoke(scoreModifier);
                //Add a score of scoreModifier
                onObtainEgg?.Invoke(scoreModifier);
                //Play OnObtain animation (specific to each egg though)
                gameObject.SetActive(false);
                photonView.RPC("RPC_SendActive", RpcTarget.Others, false);
            }

            /// <summary>
            /// What happens when an egg is broken (reaches the ground without the player touching it).
            /// </summary>
            private void OnBreak()
            {
                if (scoreModifier != -1)
                    onSpawnEgg?.Invoke(scoreModifier);
                //Play OnBreak animation (specific to each egg though)
                anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, true);
            }

            public void FinishBreakAnimation()
            {
                anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, false);
                gameObject.SetActive(false);
                photonView.RPC("RPC_SendActive", RpcTarget.Others, false);
            }

            [PunRPC]
            public void RPC_SendActive(bool b)
            {
                gameObject.SetActive(b);
            }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    //Ours
                    stream.SendNext(transform.position);
                    stream.SendNext(anim.GetBool(Constants.BOOL_BROKENEGG_ANIM));
                    stream.SendNext(Rb.velocity);
                }
                else
                {
                    //Others
                    transform.position = (Vector3)stream.ReceiveNext();
                    anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, (bool)stream.ReceiveNext());
                    Rb.velocity = (Vector3)stream.ReceiveNext();
                }
            }
        }

    }
}
