using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;
using System.Linq;

/// <summary>
/// The present game session where we store the spawning routines and info of the game.
/// EGG, Egg Grabbing Game
/// </summary>
[System.Serializable]
public class eggb_EGG
{
    private static readonly eggb_EGG current = new eggb_EGG();

    private static int totalAmountOfMaps = -1;
    public static int TotalAmountOfMaps
    {
        get
        {
            if(totalAmountOfMaps == -1)
            {
                Object[] all = Resources.LoadAll(path + "Easy", typeof(Array2DInt)).Cast<Array2DInt>().ToArray();
                totalAmountOfMaps = all.Length;
                Resources.UnloadUnusedAssets();
            }
            return totalAmountOfMaps;
        }
    }

    private static string path = "Easter Resources/Egg Grabbing Games/Egg Grabbing Game B Resources/EggMaps/";

    public static eggb_EGG Current
    {
        get { return current; }
    }

    private eggb_EGG()
    {
    }

    [SerializeField] public Array2DInt[] eggMaps = new Array2DInt[10];
    public bool isGameFinished;
    public int HighestScore { get; private set; }
    public int playerScore;
    private int startingEggCount = 0;

    /// <summary>
    /// Initializes the EGG structure.
    /// </summary>
    public void InitializeEGG()
    {
        eggMaps = InitializeEggMaps();
        isGameFinished = false;
        HighestScore = 0;

        //TODO: Unsubscribe event
        eggb_EasterEgg.onObtainEgg += OnEggObtain;
    }

    /// <summary>
    /// Loads a randomized set of Egg Maps and places them in the eggMaps array.
    /// </summary>
    public Array2DInt[] InitializeEggMaps()
    {
        Array2DInt[] aux = new Array2DInt[10];

        int[] randomNumber = RandomlyDecideMap(TotalAmountOfMaps); //TODO: TotalAmountOfMaps is very inefficient, find a better way of getting the amount of files.

        for (int i = 0; i < 10; i++)
        {
            aux[i] = Resources.Load(path + "Easy/eggmp_Easy" + randomNumber[i], typeof(Array2DInt)) as Array2DInt;
        }

        startingEggCount = CountEggs(aux);
        Debug.Log(startingEggCount);

        return aux;
    }

    /// <summary>
    /// Randomly generates a map id array.
    /// </summary>
    /// <param name="amountOfMaps"></param>
    /// <returns></returns>
    private int[] RandomlyDecideMap(int amountOfMaps)
    {
        int[] aux = new int[10];
        
        for(int i = 0; i < 10; i++)
        {
            int r = Random.Range(0, amountOfMaps);

            while(aux.Contains(r))
                r = Random.Range(0, amountOfMaps);

            aux[i] = r;
        }

        return aux;
    }


    /// <summary>
    /// Counts the total score available in the game.
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    private int CountEggs(Array2DInt[] arr)
    {
        int totalCount = 0;
        foreach(Array2DInt matrix in arr)
        {
            int[,] aux = matrix.GetCells();
            for(int i = 0; i < aux.GetLength(0); i++)
            {
                for (int j = 0; j < aux.GetLength(1); j++)
                {
                    switch (aux[i, j])
                    {
                        case 0:
                            totalCount += 1;
                            break;
                        case 2:
                            totalCount += 3;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        return totalCount;
    }

    /// <summary>
    /// Returns the egg map, wave number n.
    /// </summary>
    /// <param name="n">The wave number of the egg map</param>
    /// <returns>Wave n egg map.</returns>
    public Array2DInt GetEggMap(int n)
    {
        return eggMaps[n];
    }

    /// <summary>
    /// Returns the egg count.
    /// </summary>
    /// <returns></returns>
    public int GetEggCount()
    {
        return startingEggCount;
    }

    /// <summary>
    /// Actions to do when an onObtainEgg event is triggered. Sets the player score in here
    /// and updates the highest score if possible.
    /// </summary>
    /// <param name="score"></param>
    public void OnEggObtain(int score)
    {
        playerScore += score;

        if (HighestScore < playerScore)
            HighestScore = playerScore;
    }
} 
