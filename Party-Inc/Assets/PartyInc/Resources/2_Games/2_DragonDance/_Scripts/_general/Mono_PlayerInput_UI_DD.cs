﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace DD
    {
        public class Mono_PlayerInput_UI_DD : MonoBehaviour
        {
            public float countdown;

            public Text countdownText;

            public Image[] inputIndicatorsImages;

            public GameObject[] inputIndicators;

            #region Unity Callbacks

            // Start is called before the first frame update
            void Start()
            {
                countdown = Mng_GameManager_DD.Current.timeForInput;
            }

            // Update is called once per frame
            void Update()
            {
                countdown -= Time.deltaTime;

                countdownText.text = string.Format("{0:0}", countdown);
            }

            private void OnEnable()
            {
                countdown = Mng_GameManager_DD.Current.timeForInput;
                IndicatorFunctions.DisableAllIndicators(inputIndicators);
                IndicatorFunctions.EnableIndicators(inputIndicators, Mng_GameManager_DD.Current.amountOfMovesThisRound);
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


