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
    namespace EGG
    {
        /// <summary>
        /// The game manager, responsible of starting the game and managing its functionality.
        /// </summary>
        public class GameManager : MonoSingleton<GameManager>
        {
            #region Events
            public delegate void ActionGameStart();
            public static event ActionGameStart onGameStart;

            public delegate void ActionGameFinish();
            public static event ActionGameFinish onGameFinish;

            public delegate void ActionEnemyScore(int score);
            public static event ActionEnemyScore onEnemyScore;
            #endregion

            // Path to the Egg Maps directory, relative to the resources folder //TODO: Check if this is consistent with other devices.
            private static string path = "Easter Resources/Egg Grabbing Games/Egg Grabbing Game B Resources/EggMaps/";

            #region General Game Settings
            [Header("General Game Settings")]
            public float countdown;
            public int amountOfEasyMaps = 4;
            public int amountOfMediumMaps = 4;
            public int amountOfHardMaps = 2;
            #endregion

            [Header("Difficulty isnt working right now")]
            public EGGBDifficulty difficulty;
            public enum EGGBDifficulty
            {
                easy,
                medium,
                hard
            }

            #region Spawner Settings
            [Header("Spawner Game Settings")]
            public float waveIntervals;
            public float timeLimitOffset;
            #endregion

            [SerializeField] public int[][,] eggMaps;
            private float currentTime;
            private bool gameStarted;
            private bool runOnce = false;
            private int startingEggCount = 0;
            public bool isGameFinished;
            public bool isHighScore;
            public int playerScore;
            public int enemyScore;
            public int winner;

            [SerializeField] private GameObject playerAssetsPrefab;
            [SerializeField] private GameObject playerPrefab;
            [SerializeField] private GameObject spawnerManagerPrefab;
            [SerializeField] private GameObject eggPoolManagerPrefab;
            [SerializeField] private GameObject uiPrefab;

            #region Network Decided Settings
            private Vector3 playerOneMiddleLane;
            private Vector3 playerTwoMiddleLane;
            #endregion

            #region Unity Callbacks
            public override void Init()
            {
                base.Init();

                EasterEgg.onObtainEgg += OnEggObtain;
                eggMaps = new int[amountOfEasyMaps + amountOfMediumMaps + amountOfHardMaps][,];
                if (PhotonNetwork.IsMasterClient) { eggMaps = InitializeEggMaps(); NotifyPlayersMaps(); }
            }

            private void OnDestroy()
            {
                EasterEgg.onObtainEgg -= OnEggObtain;
            }

            void Start()
            {
                if (PhotonNetwork.IsMasterClient) PhotonNetwork.InstantiateRoomObject(eggPoolManagerPrefab.name, new Vector3(0, 12f, 0), Quaternion.identity);

                isGameFinished = false;

                gameStarted = false;
                currentTime = 0;

                InitializeGameSettings();

                InitializePlayer();

                Instantiate(uiPrefab);

                onGameStart?.Invoke();
            }

            void Update()
            {
                currentTime += Time.deltaTime;
                // START GAME (run once per game)
                if (currentTime > countdown + 1 && !gameStarted)
                {
                    var GO = Instantiate(spawnerManagerPrefab);

                    Debug.Log("GameManager: Instantiating Egg Spawner, " + PhotonNetwork.NickName);

                    if (PhotonNetwork.IsMasterClient)
                        GO.GetComponent<EggSpawnerManager>().SetSettings(waveIntervals, timeLimitOffset, playerOneMiddleLane);
                    else GO.GetComponent<EggSpawnerManager>().SetSettings(waveIntervals, timeLimitOffset, playerTwoMiddleLane);

                    gameStarted = true;
                }

                if (isGameFinished && !runOnce)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        winner = DecideWinner();
                        photonView.RPC("RPC_SendFinishedGame", RpcTarget.Others, isGameFinished, winner);
                    }

                    onGameFinish?.Invoke();
                    StartCoroutine("GameFinishCo");
                    runOnce = true;
                }

                photonView.RPC("RPC_SendCountdown", RpcTarget.Others, currentTime);
            }
            #endregion

            #region Private Methods

            private int DecideWinner()
            {
                int ret = 0;

                if(PhotonNetwork.PlayerList.Count() == 1)
                {
                    return -1;
                }

                if(playerScore > enemyScore)
                {
                    ret = PhotonNetwork.LocalPlayer.ActorNumber; //Master client wins
                }
                else if(playerScore < enemyScore)
                {
                    foreach(var player in PhotonNetwork.PlayerList)
                    {
                        if(PhotonNetwork.LocalPlayer.ActorNumber != player.ActorNumber)
                        {
                            ret = player.ActorNumber; // Other client wins
                        }
                    }
                }

                return ret;
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

                if (photonView.IsMine) isHighScore = GeneralHelperFunctions.DetermineHighScoreInt(FiestaTime.Constants.EGG_KEY_HISCORE, playerScore, true);
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

                int[][,] ret = new int[randomNumberE.Length + randomNumberM.Length + randomNumberH.Length][,];

                for (int i = 0; i < randomNumberE.Length + randomNumberM.Length + randomNumberH.Length; i++)
                {
                    ret[i] = aux[i].GetCells();
                }

                return ret;
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

            /// <summary>
            /// Initializes the unmodifiable settings of the game which are dependant on the amount of players playing.
            /// </summary>
            private void InitializeGameSettings()
            {
                playerTwoMiddleLane = Constants.TWOPLAYER_MID_LANE_PLYRTWO;

                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    Debug.Log("GameManager: Setting up for two players...");
                    playerOneMiddleLane = Constants.TWOPLAYER_MID_LANE_PLYRONE;
                }
                else
                {
                    playerOneMiddleLane = Constants.ONEPLAYER_MID_LANE;
                }
            }

            /// <summary>
            /// Initializes the player/s.
            /// </summary>
            private void InitializePlayer()
            {
                Vector3 decidedVector;

                if (PhotonNetwork.IsMasterClient) decidedVector = playerOneMiddleLane;
                else decidedVector = playerTwoMiddleLane;

                var holder = PhotonNetwork.Instantiate(playerAssetsPrefab.name, decidedVector, Quaternion.identity);
                var plyr = PhotonNetwork.Instantiate(playerPrefab.name, decidedVector + new Vector3(0f, 0.7f, 0f), Quaternion.identity);

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
            }

            /// <summary>
            /// Actions to do when an onObtainEgg event is triggered. Sets the player score.
            /// </summary>
            /// <param name="score"></param>
            public void OnEggObtain(int score)
            {
                playerScore += score;
                photonView.RPC("RPC_SendScore", RpcTarget.Others, playerScore);
            }
            #endregion

            #region PUN RPC
            [PunRPC]
            public void RPC_SendCountdown(float time)
            {
                currentTime = time;
            }

            [PunRPC]
            public void RPC_SendFinishedGame(bool finished, int theWinner)
            {
                isGameFinished = finished;
                winner = theWinner;
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

            [PunRPC]
            public void RPC_SendScore(int score)
            {
                enemyScore = score;
                onEnemyScore?.Invoke(score);
            }
            #endregion

        }
    }
}
