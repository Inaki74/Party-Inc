using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Array2DEditor;
using System.Linq;

using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    // TODO: Rework this broken ass code and game.
    // Its so spaghetti it hurts my pride and eyes.
    namespace EGG
    {
        /// <summary>
        /// The game manager, responsible of starting the game and managing its functionality.
        /// </summary>
        ///
        // Change to FiestaManager
        public class GameManager : FiestaGameManager<GameManager, int>
        {
            #region Events
            public delegate void ActionEnemyScore(int score);
            public static event ActionEnemyScore onEnemyScore;
            #endregion

            // Path to the Egg Maps directory, relative to the resources folder //TODO: Check if this is consistent with other devices.
            private static string Path = "Easter Resources/Egg Grabbing Games/Egg Grabbing Game B Resources/EggMaps/";

            #region General Game Settings
            [Header("General Game Settings")]
            public int amountOfEasyMaps = 4;
            public int amountOfMediumMaps = 4;
            public int amountOfHardMaps = 2;
            #endregion

            #region Spawner Settings
            [Header("Spawner Game Settings")]
            public float waveIntervals;
            public float timeLimitOffset;
            #endregion

            [SerializeField] public int[][,] eggMaps;
            private bool gameStarted;
            private bool runOnce = false;
            private int startingEggCount = 0;
            public bool isGameFinished;
            public bool isHighScore;
            public int winnerId;

            [SerializeField] private GameObject playerAssetsPrefab;
            [SerializeField] private GameObject spawnerManagerPrefab;
            [SerializeField] private GameObject eggPoolManagerPrefab;

            #region FiestaFunctions

            protected override void InStart()
            {
                eggMaps = new int[amountOfEasyMaps + amountOfMediumMaps + amountOfHardMaps][,];

                if (PhotonNetwork.IsMasterClient)
                {
                    eggMaps = InitializeEggMaps();
                    NotifyPlayersMaps();
                }

                isGameFinished = false;
                gameStarted = false;
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayer();

                Instantiate(UIPrefab);

                OnGameStartInvoke();
            }
            #endregion

            #region Unity Callbacks
            void Update()
            {
                // START GAME (run once per game)
                if (gameStartCountdown <= -1f && !gameStarted)
                {
                    InstantiateSpawnerManagers();

                    gameStarted = true;
                }
                else
                {
                    gameStartCountdown -= Time.deltaTime;
                }

                if (isGameFinished && !runOnce)
                {
                    FinishGame();
                    runOnce = true;
                }
            }
            #endregion

            #region Private Methods

            /// <summary>
            /// Initializes position vector.
            /// </summary>
            private void SetPositionsVector()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = Vector3.zero;
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(4f, 0f, 0f);
                        playerPositions[1] = new Vector3(-4f, 0f, 0f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(5f, 0f, 0f);
                        playerPositions[1] = new Vector3(0f, 0f, 0f);
                        playerPositions[2] = new Vector3(-5f, 0f, 0f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(6f, 0f, 0f);
                        playerPositions[1] = new Vector3(2f, 0f, 0f);
                        playerPositions[2] = new Vector3(-2f, 0f, 0f);
                        playerPositions[3] = new Vector3(-6f, 0f, 0f);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Initializes the player/s.
            /// </summary>
            private void InitializePlayer()
            {
                SetPositionsVector();

                Vector3 decidedVector = Vector3.zero;

                for(int i = 0; i < playerCount; i++)
                {
                    if(PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        decidedVector = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerAssetsPrefab.name, decidedVector, Quaternion.identity);
                PhotonNetwork.Instantiate(playerPrefab.name, decidedVector + new Vector3(0f, 0.7f, 0f), Quaternion.identity);
            }

            private void InstantiateSpawnerManagers()
            {
                var GO = Instantiate(spawnerManagerPrefab);

                Vector3 decidedVector = Vector3.zero;

                for (int i = 0; i < playerCount; i++)
                {
                    if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        decidedVector = playerPositions[i];
                    }
                }

                GO.GetComponent<EggSpawnerManager>().SetSettings(waveIntervals, timeLimitOffset, decidedVector);
            }

            private void FinishGame()
            {
                DecideWinner();

                OnGameFinishInvoke();
                StartCoroutine("GameFinishCo");
            }

            private void DecideWinner()
            {
                playerResults = GetPlayerResults();

                int max = -1;
                for (int i = 0; i < playerResults.Length; i++)
                {
                    if (playerResults[i].scoring > max)
                    {
                        max = playerResults[i].scoring;
                        winnerId = playerResults[i].playerId;
                    }
                }

                PlayerResults<int> aux = new PlayerResults<int>();
                aux.scoring = max;
                int hap = 0;
                foreach (PlayerResults<int> result in playerResults)
                {
                    if (result.Equals(aux)) hap++;
                }

                if (hap > 1)
                {
                    //Draw
                    winnerId = -1;
                }
            }

            /// <summary>
            /// The Game Finish coroutine which restarts the entire game.
            /// </summary>
            /// <returns></returns>
            private IEnumerator GameFinishCo()
            {
                yield return new WaitForSeconds(2.5f);

                Resources.UnloadUnusedAssets();

                StopAllCoroutines();
            }

            private PlayerResults<int>[] GetPlayerResults()
            {
                PlayerResults<int>[] ret = new PlayerResults<int>[playerCount];
                Player[] players = FindObjectsOfType<Player>();

                for (int i = 0; i < playerCount; i++)
                {
                    ret[i] = players[i].MyResults;
                }

                return ret;
            }

            /// <summary>
            /// Loads a randomized set of Egg Maps and places them in the eggMaps array.
            /// </summary>
            private int[][,] InitializeEggMaps()
            {
                Array2DInt[] aux = new Array2DInt[amountOfEasyMaps + amountOfMediumMaps + amountOfHardMaps];

                int[] randomNumberE = RandomlyDecideMap(TotalAmountOfMaps("Easy"), amountOfEasyMaps); //TODO: TotalAmountOfMaps is very inefficient, find a better way of getting the amount of files.
                int[] randomNumberM = RandomlyDecideMap(TotalAmountOfMaps("Medium"), amountOfMediumMaps);
                int[] randomNumberH = RandomlyDecideMap(TotalAmountOfMaps("Hard"), amountOfHardMaps);

                int m = 0;
                int h = 0;

                for (int e = 0; e < randomNumberE.Length + randomNumberM.Length + randomNumberH.Length; e++)
                {
                    string decided = "";
                    if (e < amountOfEasyMaps)
                    {
                        decided = Path + "Easy/eggmp_Easy" + randomNumberE[e];
                    }
                    else if (e >= amountOfEasyMaps && e < amountOfEasyMaps + amountOfMediumMaps)
                    {
                        decided = Path + "Medium/eggmp_Medium" + randomNumberM[m];
                        m++;
                    }
                    else
                    {
                        decided = Path + "Hard/eggmp_Hard" + randomNumberH[h];
                        h++;
                    }
                    aux[e] = Resources.Load(decided, typeof(Array2DInt)) as Array2DInt;
                }

                startingEggCount = CountEggs(aux);

                int[][,] ret = new int[randomNumberE.Length + randomNumberM.Length + randomNumberH.Length][,];

                for (int i = 0; i < randomNumberE.Length + randomNumberM.Length + randomNumberH.Length; i++)
                {
                    ret[i] = aux[i].GetCells();
                }

                return ret;
            }

            private int TotalAmountOfMaps(string difficulty)
            {
                Object[] all = Resources.LoadAll(Path + difficulty, typeof(Array2DInt)).Cast<Array2DInt>().ToArray();
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
                for (int j = 0; j < n; j++)
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

            private void NotifyPlayersMaps()
            {
                int[][][] ret = new int[amountOfEasyMaps + amountOfMediumMaps + amountOfHardMaps][][];

                for (int i = 0; i < amountOfEasyMaps + amountOfMediumMaps + amountOfHardMaps; i++)
                {
                    ret[i] = SerializationHelperClass.SerializeTDArray(eggMaps[i]);
                }

                photonView.RPC("RPC_SendEggMaps", RpcTarget.Others, new object[] { ret, startingEggCount });
            }

            #endregion

            #region Public Methods
            /// <summary>
            /// Returns the egg map, wave number n.
            /// </summary>
            /// <param name="n">The wave number of the egg map</param>
            /// <returns>Wave n egg map.</returns>
            public int[,] GetEggMap(int n)
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
                photonView.RPC("RPC_SendFinishedGame", RpcTarget.Others, b);
            }
            #endregion

            #region PUN RPC

            [PunRPC]
            public void RPC_SendFinishedGame(bool finished)
            {
                isGameFinished = finished;
            }

            [PunRPC]
            public void RPC_SendEggMaps(object[] args)
            {
                for (int i = 0; i < amountOfEasyMaps + amountOfMediumMaps + amountOfHardMaps; i++)
                {
                    int[][][] received = (int[][][])args[0];
                    eggMaps[i] = SerializationHelperClass.DeserializeTDArray(received[i]);
                }
                startingEggCount = (int)args[1];
            }
            #endregion

        }
    }
}
