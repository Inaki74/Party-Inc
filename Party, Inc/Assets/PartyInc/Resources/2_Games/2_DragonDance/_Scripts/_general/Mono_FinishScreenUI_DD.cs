using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

namespace PlayInc
{
    namespace DD
    {
        public class Mono_FinishScreenUI_DD : MonoBehaviour
        {
            [Header("Players in the list, in order")]
            [SerializeField] private GameObject[] players;
            [SerializeField] private GameObject highScores;

            [SerializeField] private Text highScore;
            [SerializeField] private Text flavourTitle;

            [Header("Players in the list, in order")]
            [SerializeField] private Text[] playerPlacings;
            [SerializeField] private Text[] playerNames;
            [SerializeField] private Text[] playerScores;

            private void OnEnable()
            {
                // Set active for the amount of players playing.
                for (int i = 0; i < Mng_GameManager_DD.Current.playerCount; i++)
                {
                    players[i].SetActive(true);
                }

                if(Mng_GameManager_DD.Current.playerCount == 1)
                {
                    highScores.SetActive(true);
                    highScore.text = PlayerPrefs.GetInt(Constants.DD_KEY_HISCORE).ToString();

                    playerNames[0].text = PhotonNetwork.LocalPlayer.NickName;
                    playerPlacings[0].enabled = false;
                    playerScores[0].text = Mng_GameManager_DD.Current.playerResults[0].scoring.ToString();

                    if (Mng_GameManager_DD.Current.isHighScore)
                    {
                        flavourTitle.text = "CONGRATULATIONS! New high score!";
                        highScore.color = Color.yellow;
                    }
                    else
                    {
                        flavourTitle.text = "You'll get that high score someday!";
                    }
                }
                else
                {
                    // Set the list with winners and losers
                    SetPositionsList();

                    // Set flavour text
                    if (Mng_GameManager_DD.Current.winnerId == -1)
                    {
                        flavourTitle.text = "Unbelievable! Its a draw!";
                    }
                    else
                    {
                        if (Mng_GameManager_DD.Current.winnerId == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            flavourTitle.text = "CONGRATULATIONS! You win!";
                        }
                        else
                        {
                            flavourTitle.text = "Welp... Can't have them all buddy.";
                        }
                    }
                }

                
            }

            private void SetPositionsList()
            {
                var aux = Mng_GameManager_DD.Current.playerResults.OrderByDescending(r => r.scoring);
                PlayerResults<int>[] res = aux.ToArray();
                int place = 0;
                int lastScore = -1;
                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i].scoring != lastScore)
                    {
                        place++;
                    }
                    lastScore = res[i].scoring;

                    switch (place)
                    {
                        case 1:
                            playerPlacings[i].text = "1st";
                            break;
                        case 2:
                            playerPlacings[i].text = "2nd";
                            break;
                        case 3:
                            playerPlacings[i].text = "3rd";
                            break;
                        case 4:
                            playerPlacings[i].text = "4th";
                            break;
                        default:
                            Debug.Log("FinishScreenUI, result not possible");
                            break;
                    }

                    playerScores[i].text = res[i].scoring.ToString();

                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (res[i].playerId == player.ActorNumber)
                        {
                            playerNames[i].text = player.NickName;
                        }
                    }

                    if (res[i].playerId == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        playerNames[i].color = Color.yellow;
                        playerPlacings[i].color = Color.yellow;
                        playerScores[i].color = Color.yellow;
                    }
                }
            }

            public void BtnExitToMainMenu()
            {
                PhotonNetwork.LeaveRoom();

                SceneManager.LoadScene(1);
            }
        }
    }
}


