using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager in charge of the UI.
/// </summary>
public class eggb_UIManager : MonoBehaviour
{
    public int playerScore;
    public int totalScore = 0;

    #region UI Elements
    public Text playerScoreText;
    public Text enemyScoreText;
    public Text totalScoreText;
    public Text countdownText;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        playerScore = 0;
        GetTotalScore();
    }

    private void Awake()
    {
        eggb_EasterEgg.onObtainEgg += OnEggObtain;
        eggb_EasterEgg.onSpawnEgg += OnEggSpawn;
        eggb_GameManager.onGameStart += GameStartCountdown;
        eggb_GameManager.onGameFinish += GameFinishDisplay;
        eggb_GameManager.onEnemyScore += OnEnemyScoreChange;
    }

    private void OnDestroy()
    {
        eggb_EasterEgg.onObtainEgg -= OnEggObtain;
        eggb_EasterEgg.onSpawnEgg -= OnEggSpawn;
        eggb_GameManager.onGameStart -= GameStartCountdown;
        eggb_GameManager.onGameFinish -= GameFinishDisplay;
        eggb_GameManager.onEnemyScore -= OnEnemyScoreChange;
    }
    #endregion

    /// <summary>
    /// Countdown coroutine starter.
    /// </summary>
    private void GameStartCountdown()
    {
        StartCoroutine("CountdownCo");
    }

    /// <summary>
    /// Game finish coroutine starter.
    /// </summary>
    /// <param name="looped"></param>
    private void GameFinishDisplay(bool looped)
    {
        StartCoroutine(GameFinishCo(looped));
    }

    /// <summary>
    /// The countdown coroutine.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// The game finish coroutine.
    /// </summary>
    /// <param name="looped"></param>
    /// <returns></returns>
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

    /// <summary>
    /// A Coroutine that waits for the 
    /// </summary>
    /// <returns></returns>
    private void GetTotalScore()
    {
        totalScore = eggb_GameManager.Current.GetEggCount();
        string str = "LEFT@" + totalScore;
        str = str.Replace("@", System.Environment.NewLine);
        totalScoreText.text = str;
    }

    /// <summary>
    /// What happens when the player obtains an egg.
    /// </summary>
    /// <param name="score"></param>
    private void OnEggObtain(int score)
    {
        playerScore += score;
        string str = "SCORE@" + playerScore;
        str = str.Replace("@", System.Environment.NewLine);
        playerScoreText.text = str;
    }

    private void OnEnemyScoreChange(int score)
    {
        string str = "SCORE@" + score;
        str = str.Replace("@", System.Environment.NewLine);
        enemyScoreText.text = str;
    }

    /// <summary>
    /// What happens when an egg is spawned.
    /// </summary>
    /// <param name="score"></param>
    private void OnEggSpawn(int score)
    {
        totalScore -= score;
        string str = "LEFT@" + totalScore;
        str = str.Replace("@", System.Environment.NewLine);
        totalScoreText.text = str;
    }

}
