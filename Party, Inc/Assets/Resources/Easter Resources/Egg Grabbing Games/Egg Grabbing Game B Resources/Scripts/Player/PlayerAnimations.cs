using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace PlayInc
{
    namespace EGG
    {
        public class PlayerAnimations : MonoBehaviourPun
        {
            #region Player Components
            [SerializeField] private Player player;
            [SerializeField] private PlayerInputManager inputManager;
            private Animator anim;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                if (!photonView.IsMine) return;

                anim = GetComponent<Animator>();
                Player[] players = FindObjectsOfType<Player>();
                PlayerInputManager[] playersI = FindObjectsOfType<PlayerInputManager>();

                foreach (var p in players)
                {
                    if (p.photonView.IsMine)
                    {
                        player = p;
                    }
                }

                foreach (var p in playersI)
                {
                    if (p.photonView.IsMine)
                    {
                        inputManager = p;
                    }
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (!photonView.IsMine) return;

                if (player.GetIfStunned())
                {
                    //Stunned
                    anim.SetBool(Constants.BOOL_STUNNED_ANIM, true);
                }
                else
                {
                    anim.SetBool(Constants.BOOL_STUNNED_ANIM, false);
                    anim.SetInteger(Constants.INT_MOVDIR_ANIM, inputManager.MovementDirection);
                }
            }
        }

    }

}
