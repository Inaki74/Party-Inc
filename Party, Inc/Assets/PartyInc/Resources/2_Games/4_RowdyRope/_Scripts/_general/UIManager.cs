﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInc
{
    namespace RR
    {
        public class UIManager : MonoSingleton<UIManager>
        {
            [SerializeField] private GameObject finishScreen;
            [SerializeField] private Text finishText;
            [SerializeField] private Text scoreText;
            [SerializeField] private Text countdownText;

            // Start is called before the first frame update
            void Start()
            {
                countdownText.enabled = true;
                finishText.enabled = false;
                StartCoroutine(UIFunctions.ShowCountdownCo(this, countdownText, GameManager.Current.gameStartCountdown));
                scoreText.text = "00";
            }

            public override void Init()
            {
                base.Init();
                RopeControllerM.onLoopComplete += OnRoundCompleted;
                GameManager.onGameFinish += OnGameFinish;
            }
            private void OnDestroy()
            {
                RopeControllerM.onLoopComplete -= OnRoundCompleted;
                GameManager.onGameFinish -= OnGameFinish;
            }

            private IEnumerator ActivateFinishScreenCo()
            {
                yield return new WaitForSeconds(2f);

                finishScreen.SetActive(true);
            }

            private void OnGameFinish()
            {
                finishText.enabled = true;
                StartCoroutine("ActivateFinishScreenCo");
            }

            public void OnRoundCompleted()
            {
                scoreText.text = string.Format("{0:0}", GameManager.Current.currentJump);
            }
        }
    }
}