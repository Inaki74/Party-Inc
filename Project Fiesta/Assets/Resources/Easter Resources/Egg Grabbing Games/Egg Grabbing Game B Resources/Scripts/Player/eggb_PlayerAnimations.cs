using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggb_PlayerAnimations : MonoBehaviour
{
    #region Player Components
    private eggb_Player player;
    private eggb_PlayerInputManager inputManager;
    private Animator anim;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<eggb_Player>();
        inputManager = FindObjectOfType<eggb_PlayerInputManager>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetIfStunned()){
            //Stunned
            anim.SetBool(Constants.BOOL_STUNNED_ANIM, true);
        }else{
            anim.SetBool(Constants.BOOL_STUNNED_ANIM, false);
            anim.SetInteger(Constants.INT_MOVDIR_ANIM, inputManager.MovementDirection);
        }
    }
}
