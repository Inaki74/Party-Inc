using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actual spawner, in charge of spawning the eggs.
/// </summary>
public class eggb_EggSpawner : MonoBehaviour
{
    private Vector3 spawningPosition;
    private eggb_EggPoolManager pool;

    void Start()
    {
        pool = FindObjectOfType<eggb_EggPoolManager>();
        spawningPosition = transform.position;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Spawns egg of type t, 0 for normal, 1 for Rotten, 2 for golden, -1 for void.
    /// </summary>
    /// <param name="t">Type of egg.</param>
    public void SpawnEgg(int t)
    {
        if (t == -1) return;

        GameObject egg = pool.RequestEgg(t);
        egg.transform.position = spawningPosition;
    }
}
