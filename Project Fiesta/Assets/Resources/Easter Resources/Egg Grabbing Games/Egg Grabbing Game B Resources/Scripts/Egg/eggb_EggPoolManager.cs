﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Manages the Object Pooling of all of the eggs with a list of each kind respectively.
/// </summary>
public class eggb_EggPoolManager : MonoBehaviour
{
    private int amountOfEggs = 2;

    [SerializeField] private Transform eggHolder;

    [SerializeField] private GameObject easterEggPrefab;
    [SerializeField] private GameObject goldenEggPrefab;
    [SerializeField] private GameObject rottenEggPrefab;

    private List<GameObject> easterEggs = new List<GameObject>();
    private List<GameObject> goldenEggs = new List<GameObject>();
    private List<GameObject> rottenEggs = new List<GameObject>();

    void Start()
    {
        eggHolder = GetComponentInChildren<Transform>();
        //InitializeEggs(amountOfEggs);
    }

    /// <summary>
    /// Initializes the first eggs on start.
    /// </summary>
    /// <param name="n"></param>
    private void InitializeEggs(int n)
    {
        for (int i = 0; i < n; i++)
        {
            GenerateEgg(0);

            GenerateEgg(1);

            GenerateEgg(2);
        }
    }

    /// <summary>
    /// Generates a single Egg of the given kind (0 for Easter, 1 for Rotten and 2 for Golden)
    /// </summary>
    /// <param name="type"></param>
    private void GenerateEgg(int type)
    {
        switch (type)
        {
            case 0:
                GameObject newEasterEgg = PhotonNetwork.Instantiate(easterEggPrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);
                newEasterEgg.transform.parent = eggHolder.transform;
                newEasterEgg.SetActive(false);
                easterEggs.Add(newEasterEgg);
                break;
            case 1:
                GameObject newRottenEgg = PhotonNetwork.Instantiate(rottenEggPrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);
                newRottenEgg.transform.parent = eggHolder.transform;
                newRottenEgg.SetActive(false);
                rottenEggs.Add(newRottenEgg);
                break;
            case 2:
                GameObject newGoldenEgg = PhotonNetwork.Instantiate(goldenEggPrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);
                newGoldenEgg.transform.parent = eggHolder.transform;
                newGoldenEgg.SetActive(false);
                goldenEggs.Add(newGoldenEgg);
                break;
        }
    }

    /// <summary>
    /// Requests an egg of a certain kind (0 for Easter, 1 for Rotten and 2 for Golden).
    /// </summary>
    /// <param name="type"></param>
    /// <returns>An Egg of "type" type.</returns>
    public GameObject RequestEgg(int type)
    {
        switch (type)
        {
            case 0:
                //Get Easter Egg
                foreach (GameObject easterEgg in easterEggs)
                {
                    if (!easterEgg.activeInHierarchy)
                    {
                        easterEgg.SetActive(true);
                        return easterEgg;
                    }
                }
                break;
            case 1:
                //Get Rotten Egg
                foreach (GameObject rottenEgg in rottenEggs)
                {
                    if (!rottenEgg.activeInHierarchy)
                    {
                        rottenEgg.SetActive(true);
                        return rottenEgg;
                    }
                }
                break;
            case 2:
                //Get Golden Egg
                foreach (GameObject goldenEgg in goldenEggs)
                {
                    if (!goldenEgg.activeInHierarchy)
                    {
                        goldenEgg.SetActive(true);
                        return goldenEgg;
                    }
                }
                break;
        }

        GenerateEgg(type);
        return RequestEgg(type);
    }
}
