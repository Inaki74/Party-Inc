using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

/// <summary>
/// Manages the spawning of the eggs relative to a total of (for now) 3 spawners tied to each lane.
/// It gives each spawner its necessary spawning routines (maps).
/// </summary>
public class eggb_EggSpawnerManager : MonoBehaviour
{
    // The spawners specific ID.
    public static int spawnerId;

    #region Configurations & Info
    // The intervals between waves.
    [SerializeField] private float waveIntervals;
    // The wave routines that each spawner must respect.
    [SerializeField] private Array2DInt eggMap;
    // The waiting time between waves.
    [SerializeField] private float timeLimitOffset;
    // Counter of the current wave we are on.
    private int currentWave;
    // The total time of spawning of the eggMap.
    private float timeLimit;
    #endregion

    #region Spawners
    // Spawners GO
    private GameObject leftSpawnerGO;
    private GameObject middleSpawnerGO;
    private GameObject rightSpawnerGO;

    // Spawners
    private eggb_EggSpawner leftSpawner;
    private eggb_EggSpawner middleSpawner;
    private eggb_EggSpawner rightSpawner;
    #endregion

    #region Private Auxiliary Variables
    // Current time we are living.
    private float currentTime;
    // The start time of spawning the map.
    private float startTime;
    private bool editorMode = false;
    private bool runOnce = true;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        if (leftSpawnerGO != null) { leftSpawnerGO.transform.position = Constants.LEFT_LANE; }
        else leftSpawnerGO = InstantiateSpawner(Constants.LEFT_LANE);

        if (middleSpawnerGO != null) { middleSpawnerGO.transform.position = Constants.MID_LANE; }
        else middleSpawnerGO = InstantiateSpawner(Constants.MID_LANE);

        if (rightSpawnerGO != null) { rightSpawnerGO.transform.position = Constants.RIGHT_LANE; }
        else rightSpawnerGO = InstantiateSpawner(Constants.RIGHT_LANE);

        leftSpawner = leftSpawnerGO.GetComponent<eggb_EggSpawner>();
        middleSpawner = middleSpawnerGO.GetComponent<eggb_EggSpawner>();
        rightSpawner = rightSpawnerGO.GetComponent<eggb_EggSpawner>();

        currentWave = 0;
        startTime = Time.time;

        if (FindObjectOfType<eggb_GameManager>() == null)
        {
            editorMode = true;
        }else eggMap = eggb_GameManager.Current.GetEggMap(0);

        Debug.Log(waveIntervals);
        Debug.Log(eggMap.GetCells().GetLength(0));
        Debug.Log(timeLimitOffset);

        timeLimit = waveIntervals * eggMap.GetCells().GetLength(0) + timeLimitOffset;

        UpdateRoutines(leftSpawner, GetColumnOfMatrix(0, eggMap.GetCells()));
        UpdateRoutines(middleSpawner, GetColumnOfMatrix(1, eggMap.GetCells()));
        UpdateRoutines(rightSpawner, GetColumnOfMatrix(2, eggMap.GetCells()));
    }

    private void Update()
    {
        currentTime = Time.time;

        if (!editorMode)
        {
            // The egg map is finished, bring in the next map.
            if (currentTime - startTime > timeLimit)
            {
                currentWave++;
                // Game has ended
                if (currentWave >= 10)
                {
                    Debug.Log("Game has ended.");
                    if (runOnce)
                    {
                        eggb_GameManager.Current.SetGameFinished(true);
                        runOnce = false;
                    }
                    return;
                }

                eggMap = eggb_GameManager.Current.GetEggMap(currentWave);
                timeLimit = waveIntervals * eggMap.GetCells().GetLength(0) + timeLimitOffset;

                UpdateRoutines(leftSpawner, GetColumnOfMatrix(0, eggMap.GetCells()));
                UpdateRoutines(middleSpawner, GetColumnOfMatrix(1, eggMap.GetCells()));
                UpdateRoutines(rightSpawner, GetColumnOfMatrix(2, eggMap.GetCells()));

                startTime = Time.time;
            }
        }
        else
        {
            // Piece of code to loop a single egg map, made to test egg maps.
            // The egg map is finished, bring in the next map.
            if (currentTime - startTime > timeLimit)
            {
                UpdateRoutines(leftSpawner, GetColumnOfMatrix(0, eggMap.GetCells()));
                UpdateRoutines(middleSpawner, GetColumnOfMatrix(1, eggMap.GetCells()));
                UpdateRoutines(rightSpawner, GetColumnOfMatrix(2, eggMap.GetCells()));

                startTime = Time.time;
            }
        }
    }
    #endregion

    /// <summary>
    /// Instantiates the spawners in their appropiate places.
    /// </summary>
    /// <param name="set">Position to be set</param>
    /// <returns>The spawner.</returns>
    private GameObject InstantiateSpawner(Vector3 set)
    {
        GameObject aux = new GameObject();

        aux.name = "EggSpawner" + spawnerId;
        spawnerId++;

        aux.AddComponent<eggb_EggSpawner>();
        aux.GetComponent<eggb_EggSpawner>().SetIntervals(waveIntervals);

        set.Set(set.x, 12f, set.z);
        aux.transform.position = set;
        return aux;
    }

    /// <summary>
    /// Updates the spawning routines of a spawner and gives the OK to start the next spawning routine.
    /// </summary>
    private void UpdateRoutines(eggb_EggSpawner spawner, List<int> newRoutine)
    {
        spawner.SetRoutine(newRoutine);
    }

    /// <summary>
    /// Gets the entire list of the matrix column.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="matrix"></param>
    /// <returns></returns>
    private List<int> GetColumnOfMatrix(int column, int[,] matrix)
    {
        if (column > 2 || column < 0)
        {
            Debug.LogError("That row doesnt exist, you can only ask for rows 0 - 2, not more nor less.");
            if (column > 2) column = 2;
            if (column < 0) column = 0;
        }

        List<int> aux = new List<int>();

        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            aux.Add(matrix[i, column]);
        }

        return aux;
    }

    /// <summary>
    /// Sets some personalized settings such as the interval in which eggs spawn and the offset between waves.
    /// </summary>
    /// <param name="intervals"></param>
    /// <param name="offset"></param>
    public void SetSettings(float intervals, float offset)
    {
        waveIntervals = intervals;
        timeLimitOffset = offset;
    }

}
