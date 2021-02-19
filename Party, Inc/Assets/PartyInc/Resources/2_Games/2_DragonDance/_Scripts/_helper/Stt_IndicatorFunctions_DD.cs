using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInc
{
    namespace DD
    {
        public static class IndicatorFunctions
        {
            public static void DisableAllIndicators(GameObject[] indicatorList)
            {
                foreach (var ind in indicatorList)
                {
                    ind.SetActive(false);
                }
            }

            public static void EnableIndicators(GameObject[] indicatorList, int upUntil)
            {
                for (int i = 0; i < upUntil; i++)
                {
                    indicatorList[i].SetActive(true);
                }
            }

            public static void ResetColors(Image[] indicatorImagesList, Color resetColor)
            {
                foreach (Image i in indicatorImagesList)
                {
                    i.color = resetColor;
                }
            }

            public static void TriggerInputIndicator(Image indicatorImage, Color theColor)
            {
                indicatorImage.color = theColor;
            }
        }

    }
}

