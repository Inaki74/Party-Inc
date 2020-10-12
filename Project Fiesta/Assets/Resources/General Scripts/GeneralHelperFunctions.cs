using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    public class GeneralHelperFunctions
    {
        /// <summary>
        /// Checks if the score is a high score and overwrites it if it is.
        /// </summary>
        public static bool DetermineHighScoreInt(string hiScoreKey, int newScore, bool needsToBeBigger)
        {
            Debug.Log(newScore);
            int highScore = 0;
            if (PlayerPrefs.HasKey(hiScoreKey))
            {
                highScore = PlayerPrefs.GetInt(hiScoreKey);
                Debug.Log("Old high score" + highScore);
                // Had high score, check
                if (needsToBeBigger)
                {
                    if (newScore > highScore)
                    {
                        // New High score
                        highScore = newScore;
                        PlayerPrefs.SetInt(hiScoreKey, highScore);
                        Debug.Log("New high score" + highScore);
                        return true;
                    }
                }
                else
                {
                    if (newScore < highScore)
                    {
                        // New High score
                        highScore = newScore;
                        PlayerPrefs.SetInt(hiScoreKey, highScore);
                        Debug.Log("New high score" + highScore);
                        return true;
                    }
                }
                
            }
            else
            {
                highScore = newScore;
                PlayerPrefs.SetInt(hiScoreKey, highScore);
                Debug.Log("New high score" + highScore);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the score is a high score and overwrites it if it is.
        /// </summary>
        public static bool DetermineHighScoreFloat(string hiScoreKey, float newScore, bool needsToBeBigger)
        {
            Debug.Log(newScore);
            float highScore = 0;
            if (PlayerPrefs.HasKey(hiScoreKey))
            {
                highScore = PlayerPrefs.GetFloat(hiScoreKey);
                Debug.Log("Old high score" + highScore);
                // Had high score, check
                if (needsToBeBigger)
                {
                    if (newScore > highScore)
                    {
                        // New High score
                        highScore = newScore;
                        PlayerPrefs.SetFloat(hiScoreKey, highScore);
                        Debug.Log("New high score" + highScore);
                        return true;
                    }
                }
                else
                {
                    if (newScore < highScore)
                    {
                        // New High score
                        highScore = newScore;
                        PlayerPrefs.SetFloat(hiScoreKey, highScore);
                        Debug.Log("New high score" + highScore);
                        return true;
                    }
                }
                
            }
            else
            {
                highScore = newScore;
                PlayerPrefs.SetFloat(hiScoreKey, highScore);
                Debug.Log("New high score" + highScore);
                return true;
            }

            return false;
        }

        public static string ShowInMinutes(float time)
        {
            int timeInInt = (int)time;
            int minutes = timeInInt / 60;
            int seconds = timeInInt - minutes * 60;
            float miliseconds = (time - timeInInt) * 100;

            return string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", seconds) + "." + string.Format("{0:00}", miliseconds);
        }
    }
}

