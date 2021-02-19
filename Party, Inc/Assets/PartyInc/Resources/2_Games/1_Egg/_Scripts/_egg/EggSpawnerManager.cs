using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace PlayInc
{
    namespace EGG
    {
        
        /// <summary>
        /// Manages the spawning of the eggs relative to a total of (for now) 3 spawners tied to each lane.
        /// It gives each spawner its necessary spawning routines (maps).
        /// </summary>
        public class EggSpawnerManager : MonoBehaviourPun
        {
            // The spawners specific ID.
            public static int spawnerId;

            #region Configurations & Info

            [Header("Spawner Game Settings")]
            // The waiting time between waves.
            [SerializeField] private float waveIntervals;
            // The waiting time between maps.
            [SerializeField] private float timeLimitOffset;

            // The wave routines that each spawner must respect.
            [SerializeField] private int[,] eggMap;
            // Counter of the current wave we are on.
            private int currentWave;
            // The total time of spawning of the eggMap.
            private float timeLimit;

            private Vector3 middleSpawnerPosition;

            [SerializeField] private GameObject spawnerPrefab;
            #endregion

            #region Spawners
            [SerializeField] private GameObject playerOneSpawnersGO;
            [SerializeField] private GameObject playerTwoSpawnersGO;
            [SerializeField] private GameObject playerThreeSpawnersGO;
            [SerializeField] private GameObject playerFourSpawnersGO;

            private List<EggSpawner> eggSpawners = new List<EggSpawner>();
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
                DecideSpawnerPositions();

                SetUpSpawners();

                currentWave = -1;
                startTime = Time.time;
            }

            private void Update()
            {
                if (!GameManager.Current.GameBegan) return;

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
                                if (PhotonNetwork.IsMasterClient) GameManager.Current.SetGameFinished(true);
                                runOnce = false;
                            }
                            return;
                        }

                        SwapEggMap(currentWave);

                        UpdateAllSpawners(eggMap);

                        startTime = Time.time;
                    }
                }
                else
                {
                    // Piece of code to loop a single egg map, made to test egg maps.
                    // The egg map is finished, bring in the next map.
                    if (currentTime - startTime > timeLimit)
                    {
                        UpdateAllSpawners(eggMap);

                        startTime = Time.time;
                    }
                }
            }
            #endregion

            /// <summary>
            /// Decide where to spawn the spawners.
            /// </summary>
            private void DecideSpawnerPositions()
            {
                switch (GameManager.Current.playerCount)
                {
                    case 1:
                        playerOneSpawnersGO.transform.position = new Vector3(0f, 12f, 0f);
                        playerTwoSpawnersGO.SetActive(false);
                        playerThreeSpawnersGO.SetActive(false);
                        playerFourSpawnersGO.SetActive(false);
                        break;
                    case 2:
                        playerOneSpawnersGO.transform.position = new Vector3(-4f, 12f, 0f);
                        playerTwoSpawnersGO.transform.position = new Vector3(4f, 12f, 0f);
                        playerThreeSpawnersGO.SetActive(false);
                        playerFourSpawnersGO.SetActive(false);
                        break;
                    case 3:
                        playerOneSpawnersGO.transform.position = new Vector3(-5f, 12f, 0f);
                        playerTwoSpawnersGO.transform.position = new Vector3(0f, 12f, 0f);
                        playerThreeSpawnersGO.transform.position = new Vector3(5f, 12f, 0f);
                        playerFourSpawnersGO.SetActive(false);
                        break;
                    case 4:
                        playerOneSpawnersGO.transform.position = new Vector3(-6f, 12f, 0f);
                        playerTwoSpawnersGO.transform.position = new Vector3(-2f, 12f, 0f);
                        playerThreeSpawnersGO.transform.position = new Vector3(2f, 12f, 0f);
                        playerFourSpawnersGO.transform.position = new Vector3(6f, 12f, 0f);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Spawns spawners in their places, saves the Spawners and load their positions.
            /// </summary>
            private void SetUpSpawners()
            {
                List<Transform> spawnerPositions = new List<Transform>();

                LoadPositions(spawnerPositions, playerOneSpawnersGO);

                if (playerTwoSpawnersGO.activeInHierarchy)
                {
                    LoadPositions(spawnerPositions, playerTwoSpawnersGO);
                }

                if (playerThreeSpawnersGO.activeInHierarchy)
                {
                    LoadPositions(spawnerPositions, playerThreeSpawnersGO);
                }

                if (playerFourSpawnersGO.activeInHierarchy)
                {
                    LoadPositions(spawnerPositions, playerFourSpawnersGO);
                }

                int c = 0;
                int p = 0;
                foreach (Transform go in spawnerPositions)
                {
                    GameObject spGo = Instantiate(spawnerPrefab);
                    EggSpawner sp = spGo.GetComponent<EggSpawner>();
                    sp.SetIntervals(waveIntervals);
                    // THis is to make that eggs spawning from certain spawners appear as "theirs"
                    if (p % GameManager.Current.playerCount == PhotonNetwork.LocalPlayer.ActorNumber - 1)
                    {
                        sp.IsMine = true;
                    }
                    eggSpawners.Add(sp);
                    spGo.transform.SetParent(go);
                    spGo.transform.position = go.position;

                    c++;

                    if (c % 3 == 0) p++;
                }
            }

            /// <summary>
            /// Loads spawner position transforms to a list of transforms.
            /// </summary>
            /// <param name="spawnerPositions"></param>
            /// <param name="spawnerGO"></param>
            private void LoadPositions(List<Transform> spawnerPositions, GameObject spawnerGO)
            {
                foreach (Transform t in spawnerGO.transform)
                {
                    spawnerPositions.Add(t);
                }
            }

            /// <summary>
            /// Updates the spawning routines of a spawner and gives the OK to start the next spawning routine.
            /// </summary>
            private void UpdateRoutines(EggSpawner spawner, List<int> newRoutine)
            {
                spawner.SetRoutine(newRoutine);
            }

            /// <summary>
            /// Updates all spawners spawning routines.
            /// </summary>
            /// <param name="map"></param>
            private void UpdateAllSpawners(int[,] map)
            {
                // Need to serialize map
                List<int> eggColOne = GetColumnOfMatrix(0, map);
                List<int> eggColTwo = GetColumnOfMatrix(1, map);
                List<int> eggColThree = GetColumnOfMatrix(2, map);

                // Load all Spawners
                int c = 0;
                foreach(EggSpawner s in eggSpawners)
                {
                    Debug.Log("Setting routines: " + c);
                    if(c % 3 == 0)
                    {
                        UpdateRoutines(s, eggColOne);
                    }
                    if (c % 3 == 1)
                    {
                        UpdateRoutines(s, eggColTwo);
                    }
                    if (c % 3 == 2)
                    {
                        UpdateRoutines(s, eggColThree);
                    }

                    c++;
                }
            }

            private void SwapEggMap(int round)
            {
                eggMap = GameManager.Current.GetEggMap(round);
                timeLimit = waveIntervals * eggMap.GetLength(0) + timeLimitOffset;
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

                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    aux.Add(matrix[i, column]);
                }

                return aux;
            }
        }
    }
}
