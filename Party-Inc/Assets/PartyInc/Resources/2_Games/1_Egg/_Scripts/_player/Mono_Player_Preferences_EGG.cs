using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace PartyInc
{
    namespace EGG
    {
        public class Mono_Player_Preferences_EGG : MonoBehaviourPun
        {
            public static GameObject LocalPlayerInstance;

            public Animator animations;
            [SerializeField] private Text nameText;
            [SerializeField] private Text scoreText;

            private string decidedCharacter;
            private string playerName;

            // Start is called before the first frame update
            void Start()
            {
                if (photonView.IsMine)
                {
                    decidedCharacter = PlayerPrefs.GetString(PartyInc.Constants.CHRCTR_KEY_NETWRK);
                    playerName = PlayerPrefs.GetString(PartyInc.Constants.NAME_KEY_NETWRK);
                }
                else
                {
                    playerName = photonView.Owner.NickName;
                    //Get the animation prefs from CustomProperities in photonView
                }


                nameText.text = playerName;

                switch (decidedCharacter)
                {
                    case PartyInc.Constants.BUNNY_NAME_CHRCTR:
                        //Put on Easter Bunny animations and sprites
                        break;
                    case PartyInc.Constants.SANTA_NAME_CHRCTR:
                        //Put on Santa Claus animations and sprites
                        break;
                }
            }

            public void UpdateScore(int score)
            {
                scoreText.text = string.Format("{0:0}", score);
                photonView.RPC("RPC_SendNewScore", RpcTarget.Others, score);
            }

            [PunRPC]
            public void RPC_SendNewScore(int score)
            {
                scoreText.text = string.Format("{0:0}", score);
            }
        }

    }
}
