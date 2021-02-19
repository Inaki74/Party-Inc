using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace PlayInc
{
    namespace TT
    {
        public class UIManager : MonoBehaviour
        {
            [SerializeField] private Text countdownText;
            [SerializeField] private Text timerText;

            [SerializeField] private GameObject finishScreen;

            private bool gameRunning = false;

            // Start is called before the first frame update
            void Start()
            {
                Debug.Log("UIManager start");
                countdownText.enabled = true;
                StartCoroutine(UIFunctions.ShowCountdownCo(this, countdownText, GameManager.Current.gameStartCountdown));
                timerText.text = "00:00.00";
            }

            // Update is called once per frame
            void Update()
            {
                if(gameRunning) timerText.text = GeneralHelperFunctions.ShowInMinutes(GameManager.Current.InGameTime);
            }

            private void Awake()
            {
                GameManager.onGameStart += OnGameStart;
                GameManager.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                GameManager.onGameStart -= OnGameStart;
                GameManager.onGameFinish -= OnGameFinish;
            }

            private void OnGameStart()
            {
                gameRunning = true;
            }

            private void OnGameFinish()
            {
                gameRunning = false;
                finishScreen.SetActive(true);
            }
        }
    }
}
