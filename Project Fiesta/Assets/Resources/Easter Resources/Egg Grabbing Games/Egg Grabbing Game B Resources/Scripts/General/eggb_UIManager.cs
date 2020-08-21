using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class eggb_UIManager : MonoBehaviour
{

    public int playerScore;
    public int totalScore = 0;
    public Text playerScoreText;
    public Text totalScoreText;
    // Start is called before the first frame update
    void Start()
    {
        playerScore = 0;
        StartCoroutine("WaitForTotalScoreCo");
        playerScoreText.text = "SCORE: " + playerScore;

        //TODO: Unsubscribe event
        eggb_EasterEgg.onObtainEgg += OnEggObtain;
        eggb_EasterEgg.onSpawnEgg += OnEggSpawn;
    }

    private IEnumerator WaitForTotalScoreCo()
    {
        while(totalScore == 0)
        {
            totalScore = eggb_EGG.Current.GetEggCount();
            totalScoreText.text = "LEFT: " + totalScore;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnEggObtain(int score)
    {
        playerScore += score;
        playerScoreText.text = "SCORE: " + playerScore;
    }

    private void OnEggSpawn(int score)
    {
        totalScore -= score;
        totalScoreText.text = "LEFT: " + totalScore;
    }
}
