using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Array2DEditor;
using System.Linq;

/// <summary>
/// The game manager, responsible of starting the game and keeping game settings
/// </summary>
public class eggb_GameManager : MonoBehaviour
{
    private static eggb_GameManager _current;
    public static eggb_GameManager Current
    {
        get
        {
            if(_current == null)
            {
                Debug.Log("No Game manager Instantiated");
            }

            return _current;
        }
    }

    public delegate void ActionGameStart();
    public static event ActionGameStart onGameStart;

    public delegate void ActionGameFinish(bool looped);
    public static event ActionGameFinish onGameFinish;

    private static string path = "Easter Resources/Egg Grabbing Games/Egg Grabbing Game B Resources/EggMaps/";

    [Header("General Game Settings")]
    public GameObject managersPrefab;
    public float countdown;
    private int amountOfEasyMaps = 4;
    private int amountOfMediumMaps = 4;
    private int amountOfHardMaps = 2;

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

    [SerializeField] public Array2DInt[] eggMaps = new Array2DInt[10];
    private float currentTime;
    private float startingTime;
    private bool gameStarted;
    private int startingEggCount = 0;
    public bool isGameFinished;
    public int playerScore;

    private void Awake()
    {
        _current = this;
        eggb_EasterEgg.onObtainEgg += OnEggObtain;
    }

    private void OnDestroy()
    {
        eggb_EasterEgg.onObtainEgg -= OnEggObtain;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("EGG initialized");
        eggMaps = InitializeEggMaps();
        isGameFinished = false;

        gameStarted = false;
        startingTime = Time.time;

        onGameStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.time;
        // START GAME (run once per game)
        if (currentTime - startingTime > countdown + 1 && !gameStarted)
        {
            var GO = Instantiate(managersPrefab);
            GO.GetComponent<eggb_EggSpawnerManager>().SetSettings(waveIntervals, timeLimitOffset);
            gameStarted = true;
        }

        if (isGameFinished)
        {
            onGameFinish?.Invoke(true);
            StartCoroutine("GameFinishCo");
            isGameFinished = false;
        }
    }

    private IEnumerator GameFinishCo()
    {
        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene("EggGrabbingGameB");
    }

    /// <summary>
    /// Loads a randomized set of Egg Maps and places them in the eggMaps array.
    /// </summary>
    public Array2DInt[] InitializeEggMaps()
    {
        Array2DInt[] aux = new Array2DInt[10];

        int[] randomNumberE = RandomlyDecideMap(TotalAmountOfMaps("Easy"), amountOfEasyMaps); //TODO: TotalAmountOfMaps is very inefficient, find a better way of getting the amount of files.
        int[] randomNumberM = RandomlyDecideMap(TotalAmountOfMaps("Medium"), amountOfMediumMaps);
        int[] randomNumberH = RandomlyDecideMap(TotalAmountOfMaps("Hard"), amountOfHardMaps);

        int m = 0;
        int h = 0;
        string decided = "";

        for (int e = 0; e < randomNumberE.Length + randomNumberM.Length + randomNumberH.Length; e++)
        {
            if (e < amountOfEasyMaps)
            {
                decided = path + "Easy/eggmp_Easy" + randomNumberE[e];
            }
            else if (e >= amountOfEasyMaps && e < amountOfEasyMaps + amountOfMediumMaps)
            {
                decided = path + "Medium/eggmp_Medium" + randomNumberM[m];
                m++;
            }
            else
            {
                decided = path + "Hard/eggmp_Hard" + randomNumberH[h];
                h++;
            }
            aux[e] = Resources.Load(decided, typeof(Array2DInt)) as Array2DInt;
        }

        startingEggCount = CountEggs(aux);

        return aux;
    }

    private int TotalAmountOfMaps(string difficulty)
    {
        Object[] all = Resources.LoadAll(path + difficulty, typeof(Array2DInt)).Cast<Array2DInt>().ToArray();
        Resources.UnloadUnusedAssets();
        return all.Length;
    }

    /// <summary>
    /// Randomly generates a map id array.
    /// </summary>
    /// <param name="amountOfMaps"></param>
    /// <returns></returns>
    private int[] RandomlyDecideMap(int amountOfMaps, int n)
    {
        int[] aux = new int[n];

        //Must initialize the array with -1s, else map number 0 will always be in the selection.
        for(int j = 0; j < n; j++)
        {
            aux[j] = -1;
        }

        for (int i = 0; i < n; i++)
        {
            int r = Random.Range(0, amountOfMaps);

            while (aux.Contains(r))
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
        foreach (Array2DInt matrix in arr)
        {
            int[,] aux = matrix.GetCells();
            for (int i = 0; i < aux.GetLength(0); i++)
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
    /// Sets if the game is finished.
    /// </summary>
    /// <param name="b"></param>
    public void SetGameFinished(bool b)
    {
        isGameFinished = b;
    }

    /// <summary>
    /// Actions to do when an onObtainEgg event is triggered. Sets the player score in here
    /// and updates the highest score if possible.
    /// </summary>
    /// <param name="score"></param>
    public void OnEggObtain(int score)
    {
        playerScore += score;
    }
}
