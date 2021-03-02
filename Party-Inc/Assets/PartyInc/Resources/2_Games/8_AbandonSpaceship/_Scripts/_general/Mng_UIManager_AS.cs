using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace AS
    {
        public class Mng_UIManager_AS : MonoSingleton<Mng_UIManager_AS>
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
                StartCoroutine(UIHelper.ShowCountdownCo(this, countdownText, Mng_GameManager_AS.Current.gameStartCountdown));
                scoreText.text = "00";
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstaclePassCheck_AS.onPlayerPassed += OnRoundCompleted;
                Mng_GameManager_AS.onGameFinish += OnGameFinish;
            }
            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_AS.onPlayerPassed -= OnRoundCompleted;
                Mng_GameManager_AS.onGameFinish -= OnGameFinish;
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
                    Mng_GameManager_AS.Current.CurrentGate);
            }
        }
    }
}
