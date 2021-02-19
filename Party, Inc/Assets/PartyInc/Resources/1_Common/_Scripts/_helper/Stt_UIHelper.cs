using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInc
{
    public static class UIHelper
    {
        public static IEnumerator GrowthAnimationCo(RectTransform toScale, float finalScale, float seconds)
        {
            float scaleDelta = finalScale - toScale.localScale.x;
            Vector3 stepVector = new Vector3( scaleDelta / (seconds * 100), scaleDelta / (seconds * 100), scaleDelta / (seconds * 100));

            for (int i = 0; i < seconds * 100; i++)
            {
                toScale.localScale += stepVector;

                yield return new WaitForSeconds(0.01f);
            }
        }

        public static IEnumerator ShowCountdownCo(MonoBehaviour obj, Text countdownText, float length)
        {
            Vector3 originalScale = countdownText.rectTransform.localScale;

            while (length > 0)
            {
                countdownText.text = string.Format("{0:0}", length);
                obj.StartCoroutine(GrowthAnimationCo(countdownText.rectTransform, 2.5f, 0.95f));

                yield return new WaitForSeconds(1.0f);

                countdownText.rectTransform.localScale = originalScale;
                length--;
            }

            countdownText.text = "START!";
            obj.StartCoroutine(GrowthAnimationCo(countdownText.rectTransform, 2.5f, 0.5f));
            yield return new WaitForSeconds(1.0f);

            countdownText.rectTransform.localScale = originalScale;
            countdownText.enabled = false;
        }

        public static string ShowInMinutes(float time)
        {
            int timeInInt = (int)time;
            int minutes = timeInInt / 60;
            int seconds = timeInInt - minutes * 60;
            float miliseconds = (time - timeInInt) * 100;

            return string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", seconds) + "." + string.Format("{0:00}", miliseconds);
        }

        public static string ShowInPercentage(float per)
        {
            return per.ToString("0.00") + "%";
        }
    }
}


