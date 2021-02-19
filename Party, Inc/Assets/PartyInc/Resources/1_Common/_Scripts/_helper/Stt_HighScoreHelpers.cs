using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayInc
{
    public class HighScoreHelpers
    {
        /// <summary>
        /// Checks if the score is a high score and overwrites it if it is.
        /// </summary>
        public static bool DetermineHighScoreInt(string hiScoreKey, int newScore, bool needsToBeBigger)
        {
            //Debug.Log(newScore);
            int highScore = 0;
            if (PlayerPrefs.HasKey(hiScoreKey))
            {
                highScore = PlayerPrefs.GetInt(hiScoreKey);
                //Debug.Log("Old high score" + highScore);
                // Had high score, check
                if (needsToBeBigger)
                {
                    if (newScore > highScore)
                    {
                        // New High score
                        highScore = newScore;
                        PlayerPrefs.SetInt(hiScoreKey, highScore);
                        //Debug.Log("New high score" + highScore);
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
                        //Debug.Log("New high score" + highScore);
                        return true;
                    }
                }
                
            }
            else
            {
                highScore = newScore;
                PlayerPrefs.SetInt(hiScoreKey, highScore);
                //Debug.Log("New high score" + highScore);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the score is a high score and overwrites it if it is.
        /// </summary>
        public static bool DetermineHighScoreFloat(string hiScoreKey, float newScore, bool needsToBeBigger)
        {
            float highScore = 0;
            if (PlayerPrefs.HasKey(hiScoreKey))
            {
                highScore = PlayerPrefs.GetFloat(hiScoreKey);
                Debug.Log("Old high score: " + highScore);
                Debug.Log("Incoming score: " + newScore);
                // Had high score, check
                if (needsToBeBigger)
                {
                    if (newScore > highScore)
                    {
                        // New High score
                        highScore = newScore;
                        PlayerPrefs.SetFloat(hiScoreKey, highScore);
                        //Debug.Log("New high score" + highScore);
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
                        //Debug.Log("New high score" + highScore);
                        return true;
                    }
                }
                
            }
            else
            {
                highScore = newScore;
                PlayerPrefs.SetFloat(hiScoreKey, highScore);
                //Debug.Log("New high score" + highScore);
                return true;
            }

            return false;
        }
    }
}

