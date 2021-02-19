using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace PartyInc
{
    namespace EGG
    {
        public class Mono_Player_Animations_EGG : MonoBehaviourPun
        {
            #region Player Components
            [SerializeField] private Mono_Player_Controller_EGG player;
            [SerializeField] private Mono_Player_Input_EGG inputManager;
            private Animator anim;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                if (!photonView.IsMine) return;

                anim = GetComponent<Animator>();
                Mono_Player_Controller_EGG[] players = FindObjectsOfType<Mono_Player_Controller_EGG>();
                Mono_Player_Input_EGG[] playersI = FindObjectsOfType<Mono_Player_Input_EGG>();

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
