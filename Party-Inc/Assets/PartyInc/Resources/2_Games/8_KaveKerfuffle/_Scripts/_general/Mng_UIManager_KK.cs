using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace KK
    {
        public class Mng_UIManager_KK : MonoSingleton<Mng_UIManager_KK>
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
                StartCoroutine(UIHelper.ShowCountdownCo(this, countdownText, Mng_GameManager_KK.Current.gameStartCountdown));
                scoreText.text = "00";
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstaclePassCheck_KK.onPlayerPassed += OnRoundCompleted;
                Mng_GameManager_KK.onGameFinish += OnGameFinish;
            }
            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_KK.onPlayerPassed -= OnRoundCompleted;
                Mng_GameManager_KK.onGameFinish -= OnGameFinish;
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
                scoreText.text = string.Format("{0:0}",
                    Mng_GameManager_KK.Current.CurrentGate);
            }
        }
    }
}
