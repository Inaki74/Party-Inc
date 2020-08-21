using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game manager, responsible of starting the game and keeping game settings
/// </summary>
public class eggb_GameManager : MonoBehaviour
{
    [Header("General Game Settings")]
    public GameObject managersPrefab;
    public float countdown;
    [Header("Difficulty isnt working right now")]
    public EGGBDifficulty difficulty;
    public enum EGGBDifficulty
    {
        easy,
        medium,
        hard
    }

    [Header("Spawner Game Settings")]
    public float waveIntervals;
    public float timeLimitOffset;

    private float currentTime;
    private float startingTime;
    private bool runOnce;

    // Start is called before the first frame update
    void Start()
    {
        runOnce = true;
        startingTime = Time.time;

        // Prepare the EGG
        eggb_EGG.Current.InitializeEGG();

        // Countdown visual to start
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.time;
        // START GAME (run once per game)
        if (currentTime - startingTime > countdown + 1 && runOnce)
        {
            var GO = Instantiate(managersPrefab);
            GO.GetComponent<eggb_EggSpawnerManager>().SetSettings(waveIntervals, timeLimitOffset);
            runOnce = false;
        }
    }
}
