﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

namespace PlayInc
{
    namespace CC
    {
        public class FinishScreenUI : MonoBehaviour
        {
            // me duele la cabeza
            [Header("Players in the list, in order")]
            [SerializeField] private GameObject[] players;
            [SerializeField] private GameObject highScores;
            [SerializeField] private GameObject timeAchieved;

            [SerializeField] private Text highScore;
            [SerializeField] private Text flavourTitle;

            [Header("Players in the list, in order")]
            [SerializeField] private Text[] playerPlacings;
            [SerializeField] private Text[] playerNames;
            [SerializeField] private Text[] playerScores;

            private void OnEnable()
            {
                // Set active for the amount of players playing.
                for (int i = 0; i < GameManager.Current.playerCount; i++)
                {
                    players[i].SetActive(true);
                }

                if (GameManager.Current.playerCount == 1)
                {
                    highScores.SetActive(true);
                    highScore.text = GeneralHelperFunctions.ShowInPercentage(PlayerPrefs.GetFloat(PlayInc.Constants.CC_KEY_HISCORE));

                    playerNames[0].text = PhotonNetwork.LocalPlayer.NickName;
                    playerPlacings[0].enabled = false;
                    playerScores[0].text = GeneralHelperFunctions.ShowInPercentage(GameManager.Current.playerResults[0].scoring);

                    if (GameManager.Current.IsHighScore)
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
                    if (GameManager.Current.WinnerId == -1)
                    {
                        // Don think a draw is possible, but ynever know so im leaving this.
                        flavourTitle.text = "Unbelievable! Its a draw!";
                    }
                    else
                    {
                        if (GameManager.Current.WinnerId == PhotonNetwork.LocalPlayer.ActorNumber)
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
                // Results are already ordered for us
                PlayerResults<float>[] res = GameManager.Current.playerResults;
                int place = 0;
                float lastScore = -1;
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
                            Debug.Log("CC/FinishScreenUI, result not possible");
                            break;
                    }

                    playerScores[i].text = GeneralHelperFunctions.ShowInPercentage(res[i].scoring);

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