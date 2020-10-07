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

            private int highScore;

            private void OnEnable()
            {
                DetermineHighScore();

                if (GameManager.Current.winner == -1)
                {
                    // One player mode
                    DisplayPlayerResults();
                }
                else
                {
                    // Two players
                    DisplayWinner();
                }
            }

            /// <summary>
            /// Checks if the score is a high score and overwrites it if it is.
            /// </summary>
            private void DetermineHighScore()
            {
                if (PlayerPrefs.HasKey(FiestaTime.Constants.EGG_KEY_HISCORE))
                {
                    highScore = PlayerPrefs.GetInt(FiestaTime.Constants.EGG_KEY_HISCORE);
                    // Had high score, check
                    if (GameManager.Current.playerScore > highScore)
                    {
                        // New High score
                        highScore = GameManager.Current.playerScore;
                        PlayerPrefs.SetInt(FiestaTime.Constants.EGG_KEY_HISCORE, highScore);
                        if (GameManager.Current.winner == -1) flavourTitle.text = "CONGRATULATIONS! New high score!";
                    }
                    else if(GameManager.Current.winner == -1)
                    {
                        flavourTitle.text = "You'll get that high score someday!";
                    }
                }
                else
                {
                    highScore = GameManager.Current.playerScore;
                    PlayerPrefs.SetInt(FiestaTime.Constants.EGG_KEY_HISCORE, highScore);
                    if(GameManager.Current.winner == -1) flavourTitle.text = "CONGRATULATIONS! New high score!";
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
                playerTwoScore.text = highScore.ToString(); // Player two here will act as the high score (you play against yourself)
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


