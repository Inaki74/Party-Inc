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
    public Text countdownText;
    // Start is called before the first frame update
    void Start()
    {
        playerScore = 0;
        StartCoroutine("WaitForTotalScoreCo");
        playerScoreText.text = "SCORE: " + playerScore;
    }

    private void Awake()
    {
        Debug.Log("UI awakened");
        eggb_EasterEgg.onObtainEgg += OnEggObtain;
        eggb_EasterEgg.onSpawnEgg += OnEggSpawn;
        eggb_GameManager.onGameStart += GameStartCountdown;
        eggb_GameManager.onGameFinish += GameFinishDisplay;
    }

    private void OnDestroy()
    {
        eggb_EasterEgg.onObtainEgg -= OnEggObtain;
        eggb_EasterEgg.onSpawnEgg -= OnEggSpawn;
        eggb_GameManager.onGameStart -= GameStartCountdown;
        eggb_GameManager.onGameFinish -= GameFinishDisplay;
    }

    private void GameStartCountdown()
    {
        StartCoroutine("CountdownCo");
    }

    private void GameFinishDisplay(bool looped)
    {
        StartCoroutine(GameFinishCo(looped));
    }

    private IEnumerator CountdownCo()
    {
        //3
        yield return new WaitForSeconds(1.0f);

        //2
        countdownText.text = "" + 2;
        yield return new WaitForSeconds(1.0f);

        //1
        countdownText.text = "" + 1;
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "START!";

        yield return new WaitForSeconds(1.0f);

        countdownText.text = "";
    }

    private IEnumerator GameFinishCo(bool looped)
    {
        countdownText.text = "FINISH!";
        yield return new WaitForSeconds(1.0f);
        if (looped)
        {
            countdownText.text = "Restarting...";

            yield return new WaitForSeconds(1.0f);
        }

        countdownText.text = "";
    }

    private IEnumerator WaitForTotalScoreCo()
    {
        yield return new WaitUntil(() => totalScore != 0);
        totalScore = eggb_GameManager.Current.GetEggCount();
        totalScoreText.text = "LEFT: " + totalScore;
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
