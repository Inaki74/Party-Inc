using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FiestaTime
{
    namespace RR
    {
        public class UIManager : MonoBehaviour
        {
            [SerializeField] private Text scoreText;
            [SerializeField] private Text countdownText;

            // Start is called before the first frame update
            void Start()
            {
                countdownText.enabled = true;
                StartCoroutine(UIFunctions.ShowCountdownCo(this, countdownText, GameManager.Current.gameStartCountdown));
                scoreText.text = "00";
            }

            private void Awake()
            {
                RopeControllerM.onLoopComplete += OnRoundCompleted;
            }

            private void OnDestroy()
            {
                RopeControllerM.onLoopComplete -= OnRoundCompleted;
            }

            private void OnRoundCompleted()
            {
                scoreText.text = string.Format("{0:0}", GameManager.Current.currentJump);
            }
        }
    }
}