using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
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
            // The intervals between waves.
            [SerializeField] private float waveIntervals;
            // The wave routines that each spawner must respect.
            [SerializeField] private int[,] eggMap;
            // The waiting time between waves.
            [SerializeField] private float timeLimitOffset;
            // Counter of the current wave we are on.
            private int currentWave;
            // The total time of spawning of the eggMap.
            private float timeLimit;

            private Vector3 middleSpawnerPosition;

            [SerializeField] private GameObject spawnerPrefab;
            #endregion

            #region Spawners
            // Spawners GO
            private GameObject leftSpawnerGO;
            private GameObject middleSpawnerGO;
            private GameObject rightSpawnerGO;

            // Spawners
            private EggSpawner leftSpawner;
            private EggSpawner middleSpawner;
            private EggSpawner rightSpawner;
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
                if (leftSpawnerGO != null) { leftSpawnerGO.transform.position = middleSpawnerPosition + Vector3.left * 1.5f; }
                else leftSpawnerGO = InstantiateSpawner(middleSpawnerPosition + Vector3.left * 1.5f);

                if (middleSpawnerGO != null) { middleSpawnerGO.transform.position = middleSpawnerPosition; }
                else middleSpawnerGO = InstantiateSpawner(middleSpawnerPosition);

                if (rightSpawnerGO != null) { rightSpawnerGO.transform.position = middleSpawnerPosition + Vector3.right * 1.5f; }
                else rightSpawnerGO = InstantiateSpawner(middleSpawnerPosition + Vector3.right * 1.5f);

                leftSpawner = leftSpawnerGO.GetComponent<EggSpawner>();
                middleSpawner = middleSpawnerGO.GetComponent<EggSpawner>();
                rightSpawner = rightSpawnerGO.GetComponent<EggSpawner>();

                currentWave = 0;
                startTime = Time.time;

                if (FindObjectOfType<GameManager>() == null)
                {
                    editorMode = true;
                }
                else
                {
                    SwapEggMap(0);
                }

                UpdateAllSpawners(eggMap);
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
            /// Instantiates the spawners in their appropiate places.
            /// </summary>
            /// <param name="set">Position to be set</param>
            /// <returns>The spawner.</returns>
            private GameObject InstantiateSpawner(Vector3 set)
            {
                set.Set(set.x, 12f, set.z);

                Debug.Log("Spawner in play");

                GameObject aux = Instantiate(spawnerPrefab);

                aux.GetComponent<EggSpawner>().SetIntervals(waveIntervals);
                aux.transform.position = set;

                return aux;
            }

            /// <summary>
            /// Updates the spawning routines of a spawner and gives the OK to start the next spawning routine.
            /// </summary>
            private void UpdateRoutines(EggSpawner spawner, List<int> newRoutine)
            {
                spawner.SetRoutine(newRoutine);
            }

            private void UpdateAllSpawners(int[,] map)
            {
                // Need to serialize map

                UpdateRoutines(leftSpawner, GetColumnOfMatrix(0, map));
                UpdateRoutines(middleSpawner, GetColumnOfMatrix(1, map));
                UpdateRoutines(rightSpawner, GetColumnOfMatrix(2, map));
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

            /// <summary>
            /// Sets some personalized settings such as the interval in which eggs spawn and the offset between waves.
            /// </summary>
            /// <param name="intervals"></param>
            /// <param name="offset"></param>
            public void SetSettings(float intervals, float offset, Vector3 spawnerPosition)
            {
                waveIntervals = intervals;
                timeLimitOffset = offset;
                middleSpawnerPosition = spawnerPosition;
            }
        }
    }
}
