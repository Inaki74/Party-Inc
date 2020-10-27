using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace TT
    {
        public class GameManager : FiestaGameManager<GameManager, float>
        {
            public static Vector3 forwardVector = new Vector3(0f, Mathf.Cos(45f), Mathf.Cos(45f));
            public static float maxDistance = 13f;
            public static float hazardMinimumVelocity = 9f;
            public bool isHighScore;
            public int winnerId;
            private int nextToInsert = 0;

            private int playersPlaying;
            private bool runOnce = false;

            public float inGameTime = 0;

            public bool gameBegan;

            protected override void InStart()
            {
                playersPlaying = playerCount;
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
                InitializeUI();
            }

            // Update is called once per frame
            void Update()
            {
                if (gameStartCountdown <= -1f) // Taking into account the start text
                {
                    if (inGameTime == 0)
                    {
                        gameBegan = true;
                        gameStartCountdown = -1f;
                        OnGameStartInvoke();
                    } 

                    if(gameBegan) inGameTime += Time.deltaTime;
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

            public override void Init()
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
                gameBegan = false;
                // Order list
                OrderResults();
                // Decide winner
                winnerId = DecideWinner();

                // then invoke on finish
                OnGameFinishInvoke();
            }

            private int DecideWinner()
            {
                // Game was too short
                if (inGameTime < 7f)
                {
                    return -1;
                }

                PlayerResults<float> first = playerResults.First();
                int hap = 0;
                // PlayerResults equals is defined as having the same score/time.
                if (playerResults.Contains(first))
                {
                    hap++;
                }

                if(hap > 1)
                {
                    return -1;
                }
                else
                {
                    return first.playerId;
                }
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
                foreach(PlayerResults<float> res in playerResults)
                {
                    if (!res.reachedEnd)
                    {
                        falseCounts++;
                    }
                    else
                    {
                        trueCounts++;
                    }
                }

                PlayerResults<float>[] didntCross = new PlayerResults<float>[falseCounts];
                PlayerResults<float>[] didCross = new PlayerResults<float>[trueCounts];

                int k = 0;
                int l = 0;
                for (int j = 0; j < playerCount; j++)
                {
                    if (playerResults[j].reachedEnd)
                    {
                        didCross[k] = playerResults[j];
                        k++;
                    }

                    if (!playerResults[j].reachedEnd)
                    {
                        didntCross[l] = playerResults[j];
                        l++;
                    }
                }

                var o1 = didCross.OrderByDescending(r => r.scoring);
                PlayerResults<float>[] ordDidCross = o1.ToArray();
                var o2 = didntCross.OrderByDescending(r => r.scoring);
                PlayerResults<float>[] ordDidntCross = o2.ToArray();
                PlayerResults<float>[] aux = new PlayerResults<float>[playerCount];

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

            //private void PrintArray(PlayerResults<T>[] args)
            //{
            //    foreach (var o in args)
            //    {
            //        foreach (var player in PhotonNetwork.PlayerList)
            //        {
            //            if (player.ActorNumber == o.playerId)
            //            {
            //                Debug.Log(player.NickName);
            //            }
            //        }

            //        Debug.Log(o.ToString());
            //    }
            //}

            private void InitializeUI()
            {
                Instantiate(UIPrefab);
            }

            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                for(int i = 0; i < playerCount; i++)
                {
                    if(PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                    }
                }
                Debug.Log(playerPrefab.name);
                PhotonNetwork.Instantiate(playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            private void OnPlayerWon(int playerId)
            {
                PlayerResults<float> thisPlayerResult = new PlayerResults<float>();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.scoring = inGameTime;
                thisPlayerResult.reachedEnd = true;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert++;
                playersPlaying--;

                Debug.Log("Player won: " + playerId + " , time: " + inGameTime + " , playersLeft: " + playersPlaying);

                if (playerId == PhotonNetwork.LocalPlayer.ActorNumber) isHighScore = GeneralHelperFunctions.DetermineHighScoreFloat(Constants.TT_KEY_HISCORE, thisPlayerResult.scoring, false);
                // Give the result to the other players
                photonView.RPC("RPC_SendPlayerResult", RpcTarget.Others, thisPlayerResult.playerId, thisPlayerResult.scoring, thisPlayerResult.reachedEnd);
            }

            private void OnPlayerLost(int playerId)
            {
                PlayerResults<float> thisPlayerResult = new PlayerResults<float>();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.scoring = inGameTime;
                thisPlayerResult.reachedEnd = false;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert++;
                playersPlaying--;

                if (playerId == PhotonNetwork.LocalPlayer.ActorNumber) isHighScore = false;

                // Give the result to the other players
                photonView.RPC("RPC_SendPlayerResult", RpcTarget.Others, thisPlayerResult.playerId, thisPlayerResult.scoring, thisPlayerResult.reachedEnd);
            }

            /// <summary>
            /// Sets the player position vectors.
            /// </summary>
            private void SetPlayerPositions()
            {
                switch (playerCount)
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
                PlayerResults<float> thisPlayerResult = new PlayerResults<float>();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.scoring = time;
                thisPlayerResult.reachedEnd = reachedFinishLine;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert++;
                playersPlaying--;
            }
        }
    }
}


