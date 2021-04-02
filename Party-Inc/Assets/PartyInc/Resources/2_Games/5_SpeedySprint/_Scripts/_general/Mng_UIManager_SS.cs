using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace PartyInc
{
    namespace SS
    {
        public class Mng_UIManager_SS : MonoBehaviour
        {
            [SerializeField] private Text countdownText;
            [SerializeField] private Text timerText;

            [SerializeField] private GameObject finishScreen;

            //TESTING
            [SerializeField] private GameObject _testingInputSub;
            [SerializeField] private InputField _testingFieldSub;
            [SerializeField] private GameObject _testingInputMS;
            [SerializeField] private InputField _testingFieldMS;
            [SerializeField] private GameObject _testingButtonGO;
            [SerializeField] private Button _testingButton;
            //TESTING

            private bool gameRunning = true;
            private bool _runOnce = false;

            // Start is called before the first frame update
            void Start()
            {
                if(Mng_GameManager_SS.Current.playerCount == 1 || !PhotonNetwork.IsConnected)
                {
                    _testingButtonGO.SetActive(true);
                    if(Mng_GameManager_SS.Current.Testing) _testingButton.GetComponentInChildren<Text>().text = "Change to Normal";
                    else _testingButton.GetComponentInChildren<Text>().text = "Change to Testing";
                }

                Debug.Log("UIManager start");
                countdownText.enabled = true;

                timerText.text = "00:00.00";
            }

            // Update is called once per frame
            void Update()
            {
                if (Mng_GameManager_SS.Current.PlayersConnectedAndReady && !_runOnce)
                {
                    _runOnce = true;
                    StartCoroutine(UIHelper.ShowCountdownCo(this, countdownText, Mng_GameManager_SS.Current.gameStartCountdown));
                }

                if (Mng_GameManager_SS.Current.Testing)
                {
                    _testingInputSub.SetActive(true);
                    _testingInputMS.SetActive(true);
                }
                else
                {
                    _testingInputSub.SetActive(false);
                    _testingInputMS.SetActive(false);
                }

                if (Mng_GameManager_SS.Current.GameBegan && gameRunning) timerText.text = UIHelper.ShowInMinutes(Mng_GameManager_SS.Current.InGameTime);
            }

            private void Awake()
            {
                Mng_GameManager_SS.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                Mng_GameManager_SS.onGameFinish -= OnGameFinish;
            }

            private void OnGameFinish()
            {
                Debug.Log("Finish Screen ");
                gameRunning = false;
                finishScreen.SetActive(true);
            }


            // FOR TESTING
            public delegate void ActionGameStart(string inp);
            public static event ActionGameStart onInputDone;

            public void OnInputDoneSub()
            {
                string inp = _testingFieldSub.text;

                inp = inp.ToUpper();

                onInputDone.Invoke(inp);
            }

            public void OnInputDoneMS()
            {
                Mng_GameManager_SS.Current.TestSetMoveSpeed(float.Parse(_testingFieldMS.text));
            }

            public void ActivateTestingMode()
            {
                Mng_GameManager_SS.Current.Testing = !Mng_GameManager_SS.Current.Testing;

                if (Mng_GameManager_SS.Current.Testing) _testingButton.GetComponentInChildren<Text>().text = "Change to Normal";
                else _testingButton.GetComponentInChildren<Text>().text = "Change to Testing";
            }
        }
    }

}