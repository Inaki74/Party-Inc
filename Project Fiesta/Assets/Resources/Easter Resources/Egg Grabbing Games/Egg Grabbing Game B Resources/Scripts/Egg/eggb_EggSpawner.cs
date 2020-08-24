using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actual spawner, in charge of spawning the eggs.
/// </summary>
public class eggb_EggSpawner : MonoBehaviour
{
    private bool OK;
    private List<int> eggMapCol = new List<int>();
    private Vector3 spawningPosition;
    private eggb_EggPoolManager pool;
    private float waveInterval;

    void Start()
    {
        pool = FindObjectOfType<eggb_EggPoolManager>();
        spawningPosition = transform.position;
        OK = true;
        StartCoroutine("WaitForOKCo");
    }

    private void Update()
    {
        // If there are no more eggs available
        if(eggMapCol.Count == 0)
        {
            // Ask if the game is not finished
            if (!eggb_GameManager.Current.isGameFinished)
            {
                // Wait for the OK of the manager
                StartCoroutine("WaitForOKCo");
            }
        }
    }

    /// <summary>
    /// Co routine that waits for the manager OK.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForOKCo()
    {
        while (!OK)
        {
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine("SpawningCo");

        OK = false;
    }

    /// <summary>
    /// The Spawning Co Routine
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawningCo()
    {
        foreach (int egg in eggMapCol)
        {
            SpawnEgg(egg);
            yield return new WaitForSeconds(waveInterval);
        }
        eggMapCol.Clear();
    }

    /// <summary>
    /// Spawns egg of type t, 0 for normal, 1 for Rotten, 2 for golden, -1 for void.
    /// </summary>
    /// <param name="t">Type of egg.</param>
    private void SpawnEgg(int t)
    {
        if (t == -1) return;

        GameObject egg = pool.RequestEgg(t);
        egg.transform.position = spawningPosition;
    }

    public void SetIntervals(float interval)
    {
        waveInterval = interval;
    }

    public void SetRoutine(List<int> routine)
    {
        OK = true;
        eggMapCol = routine;
    }
}
