using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FiestaTime
{
    namespace DD
    {
        public class PlayerInputUI : MonoBehaviour
        {
            public float countdown;

            public Text countdownText;

            public Image[] inputIndicatorsImages;

            public GameObject[] inputIndicators;

            #region Unity Callbacks

            // Start is called before the first frame update
            void Start()
            {
                countdown = GameManager.Current.timeForInput;
            }

            // Update is called once per frame
            void Update()
            {
                countdown -= Time.deltaTime;

                countdownText.text = string.Format("{0:00}", countdown);
            }

            private void OnEnable()
            {
                countdown = GameManager.Current.timeForInput;
                IndicatorFunctions.DisableAllIndicators(inputIndicators);
                IndicatorFunctions.EnableIndicators(inputIndicators ,GameManager.Current.amountOfMovesThisRound);
            }

            private void OnDisable()
            {
                IndicatorFunctions.ResetColors(inputIndicatorsImages, new Color(1f,1f,1f));
            }

            #endregion

            /// <summary>
            /// Event function that triggers when input is taken. Changes an indicators color.
            /// </summary>
            /// <param name="number"></param>
            public void TriggerInputIndicator(int number)
            {
                IndicatorFunctions.TriggerInputIndicator(inputIndicatorsImages[number], new Color(1f, 1f, 0f));
            }
        }
    }
}


