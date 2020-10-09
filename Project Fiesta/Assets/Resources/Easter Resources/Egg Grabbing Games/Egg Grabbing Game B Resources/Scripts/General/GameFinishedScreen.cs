using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;

namespace FiestaTime
{
    namespace EGG
    {
        /// <summary>
        /// Class in charge of the game over screen.
        /// </summary>
        public class GameFinishedScreen : MonoBehaviour
        {
            [SerializeField] private Text flavourTitle;
            [SerializeField] private Text playerOneName;
            [SerializeField] private Text playerOneScore;
            [SerializeField] private Text playerTwoName;
            [SerializeField] private Text playerTwoScore;

            private void OnEnable()
            {
                if (GameManager.Current.winner == -1)
                {
                    // One player mode
                    DisplayPlayerResults();

                    if(GameManager.Current.isHighScore)
                    {
                        flavourTitle.text = "CONGRATULATIONS! New high score!";
                    }
                    else
                    {
                        flavourTitle.text = "You'll get that high score someday!";
                    }
                    
                }
                else
                {
                    // Two players
                    DisplayWinner();
                }
            }

            /// <summary>
            /// Displays one player mode results.
            /// </summary>
            private void DisplayPlayerResults()
            {
                playerOneName.text = PhotonNetwork.LocalPlayer.NickName;
                playerOneName.color = new Color(1f, 1f, 0f);
                playerOneScore.text = GameManager.Current.playerScore.ToString();

                playerTwoName.text = "High Score:";
                playerTwoScore.text = PlayerPrefs.GetInt(FiestaTime.Constants.EGG_KEY_HISCORE).ToString(); // Player two here will act as the high score (you play against yourself)
            }

            /// <summary>
            /// Displays two player mode results.
            /// </summary>
            private void DisplayWinner()
            {
                int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                if (GameManager.Current.winner == playerNumber)
                {
                    flavourTitle.text = "CONGRATULATIONS! You win!";
                }
                else
                {
                    flavourTitle.text = "Better luck next time!";
                }

                if (GameManager.Current.winner == 0)
                {
                    flavourTitle.text = "Unbelievable! Draw!";
                }

                playerOneName.text = PhotonNetwork.LocalPlayer.NickName;
                playerOneName.color = new Color(1f, 1f, 0f);
                playerOneScore.text = GameManager.Current.playerScore.ToString();

                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.ActorNumber != playerNumber)
                    {
                        playerTwoName.text = player.NickName;
                        playerTwoScore.text = GameManager.Current.enemyScore.ToString();
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


