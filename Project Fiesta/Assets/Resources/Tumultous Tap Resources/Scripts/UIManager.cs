using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace FiestaTime
{
    namespace TT
    {
        public class UIManager : MonoBehaviour
        {
            [SerializeField] private Text countdownText;

            [SerializeField] private float countdown;


            // Start is called before the first frame update
            void Start()
            {
                countdownText.enabled = true;
                StartCoroutine(UIFunctions.ShowCountdownCo(this, countdownText, GameManager.Current.gameStartCountdown));
            }

            // Update is called once per frame
            void Update()
            {

            }

            private void Awake()
            {
                GameManager.onGameStart += OnGameStart;
            }

            private void OnDestroy()
            {
                GameManager.onGameStart -= OnGameStart;
            }

            private void OnGameStart()
            {
                
            }
        }
    }
}
