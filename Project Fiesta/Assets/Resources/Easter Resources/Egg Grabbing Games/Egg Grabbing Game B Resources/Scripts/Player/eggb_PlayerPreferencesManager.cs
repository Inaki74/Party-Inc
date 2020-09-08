using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class eggb_PlayerPreferencesManager : MonoBehaviourPun
{
    public static GameObject LocalPlayerInstance;

    public Animator animations;
    public Text nameText;

    private string decidedCharacter;
    private string playerName;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            decidedCharacter = PlayerPrefs.GetString(Constants.CHRCTR_KEY_NETWRK);
            playerName = PlayerPrefs.GetString(Constants.NAME_KEY_NETWRK);
        }
        else
        {
            playerName = photonView.Owner.NickName;
            //Get the animation prefs from CustomProperities in photonView
        }
        

        nameText.text = playerName;

        switch (decidedCharacter)
        {
            case Constants.BUNNY_NAME_CHRCTR:
                //Put on Easter Bunny animations and sprites
                break;
            case Constants.SANTA_NAME_CHRCTR:
                //Put on Santa Claus animations and sprites
                break;
        }
    }
}
