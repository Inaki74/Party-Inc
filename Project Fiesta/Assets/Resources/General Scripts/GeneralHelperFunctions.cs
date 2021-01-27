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

        public static string ShowInMinutes(float time)
        {
            int timeInInt = (int)time;
            int minutes = timeInInt / 60;
            int seconds = timeInInt - minutes * 60;
            float miliseconds = (time - timeInInt) * 100;

            return string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", seconds) + "." + string.Format("{0:00}", miliseconds);
        }

        public static bool CheckForLoop(int f)
        {
            // 3 Conditions

            f++;
            if (f == 999)
            {
                Debug.Log("LOOP");
                return true;
            }

            return false;
        }

        public static void DrawPlane(Vector3 position, Vector3 normal)
        {
            Vector3 v3;

            if(normal.normalized != Vector3.forward)
            {
                v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
            }
            else
            {
                v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
            }

            var corner0 = position + v3;
            var corner2 = position - v3;
            var q = Quaternion.AngleAxis((float)90.0, normal);
            v3 = q * v3;
            var corner1 = position + v3;
            var corner3 = position - v3;

            Debug.DrawLine(corner0, corner2, Color.green, 100f);
            Debug.DrawLine(corner1, corner3, Color.green, 100f);
            Debug.DrawLine(corner0, corner1, Color.green, 100f);
            Debug.DrawLine(corner1, corner2, Color.green, 100f);
            Debug.DrawLine(corner2, corner3, Color.green, 100f);
            Debug.DrawLine(corner3, corner0, Color.green, 100f);
            Debug.DrawRay(position, normal, Color.red, 100f);
        }
    }
}

