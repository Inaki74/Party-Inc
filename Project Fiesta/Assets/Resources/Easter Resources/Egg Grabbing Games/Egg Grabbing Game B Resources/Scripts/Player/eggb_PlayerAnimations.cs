using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class eggb_PlayerAnimations : MonoBehaviourPun
{
    #region Player Components
    [SerializeField] private eggb_Player player;
    [SerializeField] private eggb_PlayerInputManager inputManager;
    private Animator anim;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) return;

        anim = GetComponent<Animator>();
        eggb_Player[] players = FindObjectsOfType<eggb_Player>();
        eggb_PlayerInputManager[] playersI = FindObjectsOfType<eggb_PlayerInputManager>();

        foreach(var p in players)
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

        if (player.GetIfStunned()){
            //Stunned
            anim.SetBool(Constants.BOOL_STUNNED_ANIM, true);
        }else{
            anim.SetBool(Constants.BOOL_STUNNED_ANIM, false);
            anim.SetInteger(Constants.INT_MOVDIR_ANIM, inputManager.MovementDirection);
        }
    }
}
