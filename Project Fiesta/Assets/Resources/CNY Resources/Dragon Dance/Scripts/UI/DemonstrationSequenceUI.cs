using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace FiestaTime
{
    namespace DD
    {
        public class DemonstrationSequenceUI : MonoBehaviour
        {
            public Image[] feedbackIndicatorsImages;

            public GameObject[] feedbackIndicators;

            public GameObject holder;

            private bool startup = true;

            private void OnEnable()
            {
                if (!startup)
                {
                    holder.SetActive(true);
                    IndicatorFunctions.DisableAllIndicators(feedbackIndicators);
                    IndicatorFunctions.EnableIndicators(feedbackIndicators, GameManager.Current.amountOfMovesThisRound);
                }
                startup = false;
            }

            private void OnDisable()
            {
                IndicatorFunctions.ResetColors(feedbackIndicatorsImages, new Color(1, 1, 1));
                holder.SetActive(false);
            }

            public void TriggerFeedbackIndicator(bool isRight, int moveNumber)
            {
                if (isRight)
                {
                    IndicatorFunctions.TriggerInputIndicator(feedbackIndicatorsImages[moveNumber], new Color(0f, 1f, 0f));
                }
                else
                {
                    IndicatorFunctions.TriggerInputIndicator(feedbackIndicatorsImages[moveNumber], new Color(1f, 0f, 0f));
                }
            }
        }
    }
}


