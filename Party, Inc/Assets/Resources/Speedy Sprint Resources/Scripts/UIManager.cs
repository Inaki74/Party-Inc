using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        public class UIManager : MonoBehaviour
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
                if(GameManager.Current.playerCount == 1 || !PhotonNetwork.IsConnected)
                {
                    _testingButtonGO.SetActive(true);
                    if(GameManager.Current.Testing) _testingButton.GetComponentInChildren<Text>().text = "Change to Normal";
                    else _testingButton.GetComponentInChildren<Text>().text = "Change to Testing";
                }

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

                if (GameManager.Current.Testing)
                {
                    _testingInputSub.SetActive(true);
                    _testingInputMS.SetActive(true);
                }
                else
                {
                    _testingInputSub.SetActive(false);
                    _testingInputMS.SetActive(false);
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
                GameManager.Current.TestSetMoveSpeed(float.Parse(_testingFieldMS.text));
            }

            public void ActivateTestingMode()
            {
                GameManager.Current.Testing = !GameManager.Current.Testing;

                if (GameManager.Current.Testing) _testingButton.GetComponentInChildren<Text>().text = "Change to Normal";
                else _testingButton.GetComponentInChildren<Text>().text = "Change to Testing";
            }
        }
    }

}