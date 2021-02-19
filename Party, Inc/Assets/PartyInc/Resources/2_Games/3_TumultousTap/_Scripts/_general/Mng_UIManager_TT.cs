using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace PlayInc
{
    namespace TT
    {
        public class Mng_UIManager_TT : MonoBehaviour
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
                StartCoroutine(UIHelper.ShowCountdownCo(this, countdownText, Mng_GameManager_TT.Current.gameStartCountdown));
                timerText.text = "00:00.00";
            }

            // Update is called once per frame
            void Update()
            {
                if(gameRunning) timerText.text = UIHelper.ShowInMinutes(Mng_GameManager_TT.Current.InGameTime);
            }

            private void Awake()
            {
                Mng_GameManager_TT.onGameStart += OnGameStart;
                Mng_GameManager_TT.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                Mng_GameManager_TT.onGameStart -= OnGameStart;
                Mng_GameManager_TT.onGameFinish -= OnGameFinish;
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
