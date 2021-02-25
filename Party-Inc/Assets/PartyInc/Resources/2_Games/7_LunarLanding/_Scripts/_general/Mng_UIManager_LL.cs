using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace LL
    {
        public class Mng_UIManager_LL : MonoSingleton<Mng_UIManager_LL>
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
                StartCoroutine(UIHelper.ShowCountdownCo(this, countdownText, Mng_GameManager_LL.Current.gameStartCountdown));
                scoreText.text = "00";
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstaclePassCheck_LL.onPlayerPassed += OnRoundCompleted;
                Mng_GameManager_LL.onGameFinish += OnGameFinish;
            }
            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_LL.onPlayerPassed -= OnRoundCompleted;
                Mng_GameManager_LL.onGameFinish -= OnGameFinish;
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
                    Mng_GameManager_LL.Current.CurrentGate);
            }
        }
    }
}


