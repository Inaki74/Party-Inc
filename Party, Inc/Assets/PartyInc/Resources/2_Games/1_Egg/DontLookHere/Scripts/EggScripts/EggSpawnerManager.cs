using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of the eggs and its random factors like speed of spawning, which to spawn, where and in which direction.
/// </summary>
public class Mng_Mono_EggSpawner_EGGManager_EGG : MonoBehaviour
{
    public float startTimeInterval;
    public float deltaTimeIncrease;

    private float lastSpawnedTime;
    private float currentTime;

    [Header("Random generation settings for eggs (sum up to 100 please)")]
    [Tooltip("Please try that the three sum up to 100, if they sum up beyond 100 it won't break though, but it will be less accurate")]
    [Range(0f, 100f)]
    public float easterEggRate;
    [Range(0f, 100f)]
    public float goldenEggRate;
    [Range(0f, 100f)]
    public float rottenEggRate;

    private Mng_EggPoolManager_EGG pool;
    private Camera mainCamera;
    private float cameraWidth;
    private Vector3 spawningHeight = new Vector3(1f, 17f, 0f);

    void Start()
    {
        mainCamera = Camera.main;
        cameraWidth = mainCamera.scaledPixelWidth;
        pool = GetComponent<Mng_EggPoolManager_EGG>();
        lastSpawnedTime = Time.time;
    }

    void Update()
    {
        currentTime = Time.time;

        if(currentTime - lastSpawnedTime > startTimeInterval)
        {
            BeginSpawnEgg();
            lastSpawnedTime = Time.time;
        }
    }

    /// <summary>
    /// Spawns an egg of type t, in x position p, with a direction of d at a speed of s.
    /// </summary>
    /// <param name="t">Type of egg</param>
    /// <param name="p">Spawning position</param>
    /// <param name="d">Direction of flight</param>
    /// <param name="s">Speed of flight</param>
    private void SpawnEgg(int t, float p, Vector3 d)
    {
        //Request an egg of type t
        GameObject egg = pool.RequestEgg(t);
        //Position it in position p
        spawningHeight.Set(p, spawningHeight.y, spawningHeight.z);
        egg.transform.position = spawningHeight;
        //Launch it in direction d
        egg.GetComponent<Mono_Egg_EGG>().SetDirectionVector(d);
    }

    /// <summary>
    /// Decides the parameters in spawning an egg.
    /// </summary>
    private void BeginSpawnEgg()
    {
        //First we must decide what type of egg its going to be.
        int type = DecideType(easterEggRate / 100, goldenEggRate / 100, rottenEggRate / 100);
        //Once decided, we must decide its position with Perlin Noise
        float pos = GeneratePosition();
        //Last but not least, we must decide the direction it must go to
        //Implemented later, so ill keep it as down.
        SpawnEgg(type, pos, Vector3.down);
    }

    private float GeneratePosition()
    {
        //Vector3 aux = new Vector3(cameraWidth * Mathf.PerlinNoise(Time.time * xScale, 0.0f), 0f, mainCamera.transform.position.z);
        Vector3 aux = new Vector3(cameraWidth * Random.Range(0.05f,0.95f), 0f, mainCamera.transform.position.z);
        float posX = mainCamera.ScreenToWorldPoint(aux).x;
        return posX;
    }

    /// <summary>
    /// Decides the type of egg according to certain ratios of probability.
    /// </summary>
    /// <returns>The decided type</returns>
    private int DecideType(float easterRatio, float goldenRatio, float rottenRatio)
    {
        // If the numbers exceed the 100 total percentile
        if (easterRatio + goldenRatio + rottenRatio > 1.01)
        {
            //Divide all by 3 (worst case scenario they are all 1)
            goldenRatio = goldenRatio / 3;
            rottenRatio = rottenRatio / 3;

            //Then throw the rest of the possibilities to the easterRatio.
            easterRatio = 1 - (goldenRatio + rottenRatio);

            //Debug.Log("Easter :" + easterRatio);
            //Debug.Log("Rotten :" + rottenRatio);
            //Debug.Log("Golden :" + goldenRatio);
        }

        float randomNumber = Random.value;

        //Easter range is [0, easterRatio], golden range is (easterRatio, easterRatio + goldenRatio], rotten range is (easterRatio + goldenRatio, 1]
        //(Rotten ratio is not here since the equation has enough parameters to fill, but we need it in case they are bigger than 100%)
        if(randomNumber >= 0 && randomNumber <= easterRatio)
        {
            Debug.Log(0);
            return 0;
        }else if(randomNumber > easterRatio && randomNumber <= easterRatio + goldenRatio)
        {
            Debug.Log(2);
            return 2;
        }
        else
        {
            Debug.Log(1);
            return 1;
        }
    }
}
