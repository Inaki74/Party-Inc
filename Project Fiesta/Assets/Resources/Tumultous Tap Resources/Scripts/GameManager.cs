using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace TT
    {
        public struct PlayerResults
        {
            public int playerId;
            public float time;
            public bool reachedFinishLine;
        }

        public class GameManager : MonoSingleton<GameManager>
        {
            public static Vector3 forwardVector = new Vector3(0f, Mathf.Cos(45f), Mathf.Cos(45f));
            public static float maxDistance = 13f;
            public static float hazardMinimumVelocity = 9f;
            public int playersInGame;
            public bool isHighScore;
            public int winnerId;
            private int nextToInsert = 0;

            private Vector3[] playerPositions = new Vector3[4];

            public delegate void ActionGameStart();
            public static event ActionGameStart onGameStart;

            public delegate void ActionGameFinished();
            public static event ActionGameFinished onGameFinished;

            [SerializeField] private GameObject playerPrefab;
            [SerializeField] private GameObject uiPrefab;

            private int playersPlaying;
            private bool runOnce = false;

            public float gameStartCountdown = 3f;

            public float inGameTime;

            public PlayerResults[] playerResults;

            public bool gameBegan;

            // Start is called before the first frame update
            void Start()
            {
                playersInGame = PhotonNetwork.PlayerList.Length;
                playerResults = new PlayerResults[playersInGame];
                playersPlaying = playersInGame;

                InitializePlayers();
                InitializeUI();
            }

            // Update is called once per frame
            void Update()
            {
                if (gameStartCountdown <= -1f)
                {
                    inGameTime += Time.deltaTime;
                    gameBegan = true;
                    gameStartCountdown = -1f;
                    onGameStart?.Invoke();
                }
                else
                {
                    gameStartCountdown -= Time.deltaTime;
                }

                // PlayersLeftToLose - playersCrossedLine = playersPlaying
                if (playersPlaying == 0 && !runOnce)
                {
                    runOnce = true;
                    
                    FinishGame();
                }
            }

            private void Awake()
            {
                PlayerController.onCrossFinishLine += OnPlayerWon;
                PlayerController.onPlayerDied += OnPlayerLost;
            }

            private void OnDestroy()
            {
                PlayerController.onCrossFinishLine -= OnPlayerWon;
                PlayerController.onPlayerDied -= OnPlayerLost;
            }

            private void FinishGame()
            {
                // Order list
                OrderResults();
                // Decide winner
                winnerId = playerResults.First().playerId;
                // then invoke on finish
                onGameFinished?.Invoke();
            }

            private void OrderResults()
            {
                // Get the amount of false and trues 
                // Get all those who crossed the line
                // Get all those who didnt
                // Order those
                // Order those
                // Place first the crossed list
                // Place second the not crossed list

                int falseCounts = 0;
                int trueCounts = 0; 
                foreach(PlayerResults res in playerResults)
                {
                    if (!res.reachedFinishLine)
                    {
                        falseCounts++;
                    }
                    else
                    {
                        trueCounts++;
                    }
                }

                PlayerResults[] didntCross = new PlayerResults[falseCounts];
                PlayerResults[] didCross = new PlayerResults[trueCounts];

                for (int j = 0; j < playersInGame; j++)
                {
                    for (int t = 0; t < trueCounts; t++)
                    {
                        if (playerResults[j].reachedFinishLine)
                        {
                            didCross[t] = playerResults[j];
                        }
                    }

                    for (int f = 0; f < falseCounts; f++)
                    {
                        if (!playerResults[j].reachedFinishLine)
                        {
                            didntCross[f] = playerResults[j];
                        }
                    }
                }

                var o1 = didCross.OrderByDescending(r => r.time);
                PlayerResults[] ordDidCross = o1.ToArray();
                var o2 = didntCross.OrderBy(r => r.time);
                PlayerResults[] ordDidntCross = o2.ToArray();
                PlayerResults[] aux = new PlayerResults[playersInGame];

                int i = 0;
                for (int t = 0; t < trueCounts; t++)
                {
                    aux[i] = ordDidCross[t];
                    i++;
                }

                for (int f = 0; f < falseCounts; f++)
                {
                    aux[i] = ordDidntCross[f];
                    i++;
                }

                playerResults = aux;
            }

            private void InitializeUI()
            {
                Instantiate(uiPrefab);
            }

            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                for(int i = 0; i < playersInGame; i++)
                {
                    if(PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            private void OnPlayerWon(int playerId)
            {
                PlayerResults thisPlayerResult = new PlayerResults();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.time = inGameTime;
                thisPlayerResult.reachedFinishLine = true;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert++;
                playersPlaying--;

                // Give the result to the other players
                photonView.RPC("RPC_SendPlayerResult", RpcTarget.Others, thisPlayerResult.playerId, thisPlayerResult.time, thisPlayerResult.reachedFinishLine);
            }

            private void OnPlayerLost(int playerId)
            {
                PlayerResults thisPlayerResult = new PlayerResults();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.time = inGameTime;
                thisPlayerResult.reachedFinishLine = false;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert++;
                playersPlaying--;

                // Give the result to the other players
                photonView.RPC("RPC_SendPlayerResult", RpcTarget.Others, thisPlayerResult.playerId, thisPlayerResult.time, thisPlayerResult.reachedFinishLine);
            }

            /// <summary>
            /// Sets the player position vectors.
            /// </summary>
            private void SetPlayerPositions()
            {
                switch (playersInGame)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 0.4f, -0.4f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-1.25f, 0.4f, -0.4f);
                        playerPositions[1] = new Vector3(1.25f, 0.4f, -0.4f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-2.5f, 0.4f, -0.4f);
                        playerPositions[1] = new Vector3(0f, 0.4f, -0.4f);
                        playerPositions[2] = new Vector3(2.5f, 0.4f, -0.4f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-2.5f, 0.4f, -0.4f);
                        playerPositions[1] = new Vector3(-0.85f, 0.4f, -0.4f);
                        playerPositions[2] = new Vector3(0.85f, 0.4f, -0.4f);
                        playerPositions[3] = new Vector3(2.5f, 0.4f, -0.4f);
                        break;
                    default:
                        break;
                }
            }

            [PunRPC]
            public void RPC_SendPlayerResult(int playerId, float time, bool reachedFinishLine)
            {
                PlayerResults thisPlayerResult = new PlayerResults();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.time = time;
                thisPlayerResult.reachedFinishLine = reachedFinishLine;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert++;
                playersPlaying--;
            }
        }
    }
}


