using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace FiestaTime
{
    namespace EGG
    {
        /// <summary>
        /// The manager in charge of the UI.
        /// </summary>
        public class UIManager : MonoBehaviour
        {
            public int playerScore;
            public int totalScore = 0;

            #region UI Elements
            public Text playerScoreText;
            public Text enemyScoreText;
            public Text totalScoreText;
            public Text countdownText;

            public GameObject finishScreen;
            public GameObject normalScreen;
            #endregion

            #region Unity Callbacks
            void Start()
            {
                playerScore = 0;
                GetTotalScore();

                InitializeUI();
            }

            private void Awake()
            {
                EasterEgg.onObtainEgg += OnEggObtain;
                EasterEgg.onSpawnEgg += OnEggSpawn;
                GameManager.onGameStart += GameStartCountdown;
                GameManager.onGameFinish += GameFinishDisplay;
                GameManager.onEnemyScore += OnEnemyScoreChange;
            }

            private void OnDestroy()
            {
                EasterEgg.onObtainEgg -= OnEggObtain;
                EasterEgg.onSpawnEgg -= OnEggSpawn;
                GameManager.onGameStart -= GameStartCountdown;
                GameManager.onGameFinish -= GameFinishDisplay;
                GameManager.onEnemyScore -= OnEnemyScoreChange;
            }
            #endregion

            private void InitializeUI()
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        playerScoreText.rectTransform.anchorMin = new Vector2(0, 1);
                        playerScoreText.rectTransform.anchorMax = new Vector2(0, 1);
                        playerScoreText.rectTransform.anchoredPosition = new Vector3(Constants.XPOS_PLYRONESCORE_UI, -50, 0);

                        enemyScoreText.rectTransform.anchorMin = new Vector2(1, 1);
                        enemyScoreText.rectTransform.anchorMax = new Vector2(1, 1);
                        enemyScoreText.rectTransform.anchoredPosition = new Vector3(Constants.XPOS_PLYRTWOSCORE_UI, -50, 0);
                    }
                    else
                    {
                        enemyScoreText.rectTransform.anchorMin = new Vector2(0, 1);
                        enemyScoreText.rectTransform.anchorMax = new Vector2(0, 1);
                       enemyScoreText.rectTransform.anchoredPosition = new Vector3(Constants.XPOS_PLYRONESCORE_UI, -50, 0);

                        playerScoreText.rectTransform.anchorMin = new Vector2(1, 1);
                        playerScoreText.rectTransform.anchorMax = new Vector2(1, 1);
                        playerScoreText.rectTransform.anchoredPosition = new Vector3(Constants.XPOS_PLYRTWOSCORE_UI, -50, 0);
                    }
                }
                else
                {
                    //-1242, +992
                    playerScoreText.rectTransform.anchoredPosition = new Vector3(Constants.XPOS_PLYRONESCORE_UI, -50, 0);
                    enemyScoreText.rectTransform.anchoredPosition = new Vector3(Constants.XPOS_PLYRONESCORE_UI, 500, 0);
                }
            }

            /// <summary>
            /// Countdown coroutine starter.
            /// </summary>
            private void GameStartCountdown()
            {
                StartCoroutine(UIFunctions.ShowCountdownCo(this, countdownText, 3f));
            }

            /// <summary>
            /// Game finish coroutine starter.
            /// </summary>
            private void GameFinishDisplay()
            {
                StartCoroutine(GameFinishCo());
            }

            /// <summary>
            /// The game finish coroutine.
            /// </summary>
            /// <returns></returns>
            private IEnumerator GameFinishCo()
            {
                countdownText.enabled = true;
                countdownText.text = "FINISH!";
                yield return new WaitForSeconds(1.0f);

                finishScreen.SetActive(true);
                normalScreen.SetActive(false);
            }

            /// <summary>
            /// A Coroutine that waits for the 
            /// </summary>
            /// <returns></returns>
            private void GetTotalScore()
            {
                totalScore = GameManager.Current.GetEggCount();
                string str = "LEFT@" + totalScore;
                str = str.Replace("@", System.Environment.NewLine);
                totalScoreText.text = str;
            }

            /// <summary>
            /// What happens when the player obtains an egg.
            /// </summary>
            /// <param name="score"></param>
            private void OnEggObtain(int score)
            {
                playerScore += score;
                string str = "SCORE@" + playerScore;
                str = str.Replace("@", System.Environment.NewLine);
                playerScoreText.text = str;
            }

            private void OnEnemyScoreChange(int score)
            {
                string str = "SCORE@" + score;
                str = str.Replace("@", System.Environment.NewLine);
                enemyScoreText.text = str;
            }

            /// <summary>
            /// What happens when an egg is spawned.
            /// </summary>
            /// <param name="score"></param>
            private void OnEggSpawn(int score)
            {
                totalScore -= score;
                string str = "LEFT@" + totalScore;
                str = str.Replace("@", System.Environment.NewLine);
                totalScoreText.text = str;
            }

        }

    }
}
