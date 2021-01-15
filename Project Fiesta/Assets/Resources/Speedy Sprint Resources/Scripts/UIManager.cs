using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FiestaTime
{
    namespace SS
    {
        public class UIManager : MonoBehaviour
        {
            [SerializeField] private Text countdownText;
            [SerializeField] private Text timerText;

            [SerializeField] private GameObject finishScreen;

            private bool gameRunning = true;
            private bool _runOnce = false;

            // Start is called before the first frame update
            void Start()
            {
                Debug.Log("UIManager start");
                countdownText.enabled = true;
                
                timerText.text = "00:00.00";
            }

            // Update is called once per frame
            void Update()
            {
                if (GameManager.Current.PlayersConnectedAndReady && !_runOnce)
                {
                    _runOnce = true;
                    StartCoroutine(UIFunctions.ShowCountdownCo(this, countdownText, GameManager.Current.gameStartCountdown));
                }
                if (GameManager.Current.GameBegan && gameRunning) timerText.text = GeneralHelperFunctions.ShowInMinutes(GameManager.Current.InGameTime);
            }

            private void Awake()
            {
                GameManager.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                GameManager.onGameFinish -= OnGameFinish;
            }

            private void OnGameFinish()
            {
                gameRunning = false;
                finishScreen.SetActive(true);
            }
        }
    }
}


