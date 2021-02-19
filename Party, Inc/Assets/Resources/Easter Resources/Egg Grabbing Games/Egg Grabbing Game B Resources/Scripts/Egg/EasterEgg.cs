using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace EGG
    {
        /// <summary>
        /// The falling egg, in charge of itself.
        /// </summary>
        public class EasterEgg : MonoBehaviour
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

            public bool IsMine { get; set; }
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

                IsMine = false;

                SetEgg(eggType);
            }

            private void OnEnable()
            {
                Sc.enabled = true;
                Rb.useGravity = true;
            }

            private void OnDisable()
            {
                IsMine = false;
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.tag == "Player")
                {
                    Player p = collision.gameObject.GetComponent<Player>();

                    if (!p.GetIfStunned())
                    {
                        Sc.enabled = false;
                        Rb.useGravity = false;
                        Rb.velocity = Vector3.zero;

                        if (IsMine)
                        {
                            OnObtain();
                        }
                        else
                        {
                            gameObject.SetActive(false);
                        }
                    }
                }

                if (collision.gameObject.tag == "Ground" || (collision.gameObject.tag == "Egg" && collision.rigidbody.position.y < 2f))
                {
                    Sc.enabled = false;
                    Rb.useGravity = false;
                    Rb.velocity = Vector3.zero;
                    OnBreak();
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
            }

            /// <summary>
            /// What happens when an egg is broken (reaches the ground without the player touching it).
            /// </summary>
            private void OnBreak()
            {
                if (scoreModifier != -1 && IsMine)
                    onSpawnEgg?.Invoke(scoreModifier);
                //Play OnBreak animation (specific to each egg though)
                anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, true);
            }

            public void FinishBreakAnimation()
            {
                anim.SetBool(Constants.BOOL_BROKENEGG_ANIM, false);
                gameObject.SetActive(false);
            }
        }

    }
}
