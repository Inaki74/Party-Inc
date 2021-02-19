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
            public int totalScore = 0;

            #region UI Elements
            public Text totalScoreText;
            public Text countdownText;

            public GameObject finishScreen;
            public GameObject normalScreen;
            #endregion

            #region Unity Callbacks
            void Start()
            {
                GetTotalScore();

                GameStartCountdown();
            }

            private void Awake()
            {
                EasterEgg.onSpawnEgg += OnEggSpawn;
                GameManager.onGameFinish += GameFinishDisplay;
            }

            private void OnDestroy()
            {
                EasterEgg.onSpawnEgg -= OnEggSpawn;
                GameManager.onGameFinish -= GameFinishDisplay;
            }
            #endregion

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
            /// What happens when an egg is spawned.
            /// </summary>
            /// <param name="score"></param>
            private void OnEggSpawn(int score)
            {
                ///ESTO
                totalScore -= score;
                string str = "LEFT@" + totalScore;
                str = str.Replace("@", System.Environment.NewLine);
                totalScoreText.text = str;
            }

        }

    }
}
