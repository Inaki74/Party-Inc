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
        public static bool DetermineHighScoreInt(string hiScoreKey, int newScore)
        {
            Debug.Log(newScore);
            int highScore = 0;
            if (PlayerPrefs.HasKey(hiScoreKey))
            {
                highScore = PlayerPrefs.GetInt(hiScoreKey);
                Debug.Log("Old high score" + highScore);
                // Had high score, check
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
                highScore = newScore;
                PlayerPrefs.SetInt(hiScoreKey, highScore);
                Debug.Log("New high score" + highScore);
                return true;
            }

            return false;
        }
    }
}

