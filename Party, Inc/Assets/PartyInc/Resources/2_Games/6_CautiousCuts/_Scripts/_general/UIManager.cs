using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace PlayInc
{
    namespace CC
    {
        public class UIManager : MonoSingleton<UIManager>
        {
            [SerializeField] private GameObject _finishScreen;

            [SerializeField] private Text _heightText;
            [SerializeField] private Text _angleText;
            [SerializeField] private Text _totalText;

            [SerializeField] private Text countdownText;

            /// TESTING
            [SerializeField] private GameObject _testingButtonGO;
            [SerializeField] private Button _testingButton;
            [SerializeField] private GameObject _testingInputWindow;
            [SerializeField] private InputField _testingFieldWindow;
            ///

            private bool _runOnce = false;

            // Start is called before the first frame update
            void Start()
            {
                if (GameManager.Current.playerCount == 1 || !PhotonNetwork.IsConnected)
                {
                    _testingButtonGO.SetActive(true);
                    if (GameManager.Current.Testing) _testingButton.GetComponentInChildren<Text>().text = "Change to Normal";
                    else _testingButton.GetComponentInChildren<Text>().text = "Change to Testing";
                }

                countdownText.enabled = true;
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
                    _testingInputWindow.SetActive(true);
                }
                else
                {
                    _testingInputWindow.SetActive(false);
                }
            }

            private void DisplayScore(float height, float angle, float total)
            {
                _heightText.text = "P: " + height.ToString("0.00") + "%";
                _angleText.text = "A: " + angle.ToString("0.00") + "%";
                _totalText.text = "TOTAL: " + total.ToString("0.00") + "%";
            }

            private void OnGameFinish()
            {
                _finishScreen.SetActive(true);
            }

            public override void Init()
            {
                base.Init();

                Player.onLogSlicedScore += DisplayScore;
                GameManager.onGameFinish += OnGameFinish;
            }

            private void OnDestroy()
            {
                Player.onLogSlicedScore -= DisplayScore;
                GameManager.onGameFinish -= OnGameFinish;
            }

            public void OnInputDoneWindow()
            {
                GameManager.Current.TestSetWindowTime(float.Parse(_testingFieldWindow.text));
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


