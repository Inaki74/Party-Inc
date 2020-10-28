using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace FiestaTime
{
    namespace DD
    {
        public class GameManager : FiestaGameManager<GameManager, int>
        {

            //The sequence map of moves to be generated.
            public int[] sequenceMap = new int[12];
            public int amountOfMovesThisRound = 4;
            public float countdownGameStart;

            public int winnerId;
            public bool isHighScore;

            // Difficulty scaling factors
            public float timeForInput = 15f;
            public float timeToSeeMove = 1f;
            [SerializeField] private float minTimeForInput = 7f;
            [SerializeField] private float minTimeToSeeMove = 0.5f;
            public int playersHealth = 5;

            private float fieryRoundsTimeToSeeMoveDiscount;
            private float fieryRoundsTimeForInputDiscount;
            private int fieryRoundsSequenceChangerPosition = 0;

            private float amountOfPlayers;
            private Hashtable playersReady = new Hashtable();
            private Hashtable playersLost = new Hashtable();

            public bool[] playersLostAr;

            #region Events

            public delegate void ActionSectionAdvance(int nextPhase);
            public static event ActionSectionAdvance onNextPhase;

            #endregion

            private bool isGameRunning = true;
            private bool startedGame;
            private bool localPlayerReady;

            #region Game Loop

            protected override void InStart()
            {
                Debug.Log("Barrack Obama Start");
                startedGame = false;
                playersLostAr = new bool[playerCount];

                fieryRoundsTimeForInputDiscount = (timeForInput - minTimeForInput) / 10;
                fieryRoundsTimeToSeeMoveDiscount = (timeToSeeMove - minTimeToSeeMove) / 10;

                // Generate the sequence map to be followed.
                if (PhotonNetwork.IsMasterClient)
                {
                    GenerateSequenceMap();
                    SendSequenceMap();
                }

                // Set that no one lost yet.
                SetPlayersLost();

                // Set that no one is ready yet.
                ResetRemotePlayersReady();
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                Debug.Log("Barrack Obama Init");
                // Initialize players
                InitializePlayers();

                // Initialize player UI
                Instantiate(UIPrefab);
            }

            private void Update()
            {
                Debug.Log("Barrack Obama Update");
                if(!startedGame && countdownGameStart <= -1f)
                {
                    startedGame = true;
                    // Start game
                    StartCoroutine(GameLoopCo());
                }
                else if(countdownGameStart > -1f)
                {
                    countdownGameStart -= Time.deltaTime;
                }
            }

            /// <summary>
            /// The game loop coroutine. Im using this rather than update because the WaitFor classes help a lot with the loop flow.
            /// </summary>
            /// <returns></returns>
            private IEnumerator GameLoopCo()
            {
                //A buffer just to make sure master isnt ahead.
                //StartCoroutine(BufferCo());

                //yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Loop Start."));
                //ResetPlayersReady();

                while (isGameRunning)
                {
                    // Trigger "Sequence Showing" Sequence
                    onNextPhase?.Invoke(0);
                    //yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Sequence Showing."));
                    //ResetPlayersReady();
                    yield return new WaitUntil(() => LocalPlayerReady());
                    localPlayerReady = false;

                    // Activate "Player Input" Sequence
                    onNextPhase?.Invoke(1);
                    yield return new WaitUntil(() => RemotePlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Player Input."));
                    ResetRemotePlayersReady();

                    // Activate "Player Demonstration" Sequence
                    onNextPhase?.Invoke(2);
                    //yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Demonstration."));
                    //ResetPlayersReady();
                    yield return new WaitUntil(() => LocalPlayerReady());
                    localPlayerReady = false;

                    // Trigger "Results Showing" Sequence
                    onNextPhase?.Invoke(3);
                    //yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Results Showing."));
                    //ResetPlayersReady();

                    yield return new WaitUntil(() => RemotePlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Results Showing."));
                    ResetRemotePlayersReady();

                    //Fiery round
                    if(amountOfMovesThisRound == sequenceMap.Length)
                    {
                        // In fiery rounds
                        if (timeForInput - fieryRoundsTimeForInputDiscount > minTimeForInput)
                            timeForInput -= fieryRoundsTimeForInputDiscount;

                        if (timeToSeeMove - fieryRoundsTimeForInputDiscount > minTimeToSeeMove)
                            timeToSeeMove -= fieryRoundsTimeToSeeMoveDiscount;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            RerandomizeSequenceMap(fieryRoundsSequenceChangerPosition);
                            SendSequenceMap();

                            fieryRoundsSequenceChangerPosition = (fieryRoundsSequenceChangerPosition + 2) % sequenceMap.Length;
                        }
                    }
                    else
                    {
                        amountOfMovesThisRound += 2;
                    }

                    // Game is over if either of these are true:
                    //      There is one player standing. (unless playing alone)
                    //      There are no players standing.
                    isGameRunning = !CheckPlayersStanding();
                }

                // Outro
                if (playerCount > 1) DecideWinner();
                else
                {
                    winnerId = PhotonNetwork.LocalPlayer.ActorNumber;
                    playerResults = GetPlayerResults();
                } 

                Debug.Log("Invoking Phase 4, winnerId: " + winnerId);
                onNextPhase?.Invoke(4);

                //Register results in the system, yada yada, profit
            }

            //private void PrintArray(PlayerResults[] args)
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

            #endregion

            #region Private Functions

            private void DecideWinner()
            {
                playerResults = GetPlayerResults();

                int max = -1;
                for(int i = 0; i < playerResults.Length; i++)
                {
                    if(playerResults[i].scoring > max)
                    {
                        max = playerResults[i].scoring;
                        winnerId = playerResults[i].playerId;
                    }
                }

                PlayerResults<int> aux = new PlayerResults<int>();
                aux.scoring = max;
                int hap = 0;
                foreach(PlayerResults<int> result in playerResults)
                {
                    if (result.Equals(aux)) hap++;
                }

                if(hap > 1)
                {
                    //Draw
                    winnerId = -1;
                }
            }

            private PlayerResults<int>[] GetPlayerResults()
            {
                PlayerResults<int>[] ret = new PlayerResults<int>[playerCount];
                Player[] players = FindObjectsOfType<Player>();

                for(int i = 0; i < playerCount; i++)
                {
                    ret[i] = players[i].myResults;
                }

                return ret;
            }

            /// <summary>
            /// Initializes players in their positions.
            /// </summary>
            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedVec = Vector3.zero;

                for (int i = 0; i < playerCount; i++)
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
                    {
                        decidedVec = playerPositions[i];
                    }
                }
                Debug.Log(playerPrefab.name);
                PhotonNetwork.Instantiate(playerPrefab.name, decidedVec, Quaternion.identity);
            }

            /// <summary>
            /// Sets the player position vectors.
            /// </summary>
            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 0.5f, -16f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-1.25f, 0.5f, -16f);
                        playerPositions[1] = new Vector3(1.25f, 0.5f, -16f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-2.5f, 0.5f, -16f);
                        playerPositions[1] = new Vector3(0f, 0.5f, -16f);
                        playerPositions[2] = new Vector3(2.5f, 0.5f, -16f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-2.5f, 0.5f, -16f);
                        playerPositions[1] = new Vector3(-0.85f, 0.5f, -16f);
                        playerPositions[2] = new Vector3(0.85f, 0.5f, -16f);
                        playerPositions[3] = new Vector3(2.5f, 0.5f, -16f);
                        break;
                }
            }

            /// <summary>
            /// Resets the state of the PlayersReady array (all false).
            /// </summary>
            private void ResetRemotePlayersReady()
            {
                foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    playersReady.Remove(p.ActorNumber);
                    playersReady.Add(p.ActorNumber, false);
                }
            }

            /// <summary>
            /// Resets the state of the PlayersLost array (all false).
            /// </summary>
            private void SetPlayersLost()
            {
                foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    playersLost.Remove(p.ActorNumber);
                    playersLost.Add(p.ActorNumber, false);
                }
            }

            /// <summary>
            /// Returns true if theres one or less players standing (game is over).
            /// </summary>
            /// <returns></returns>
            private bool CheckPlayersStanding()
            {
                int playersStanding = 0;

                for(int i = 0; i < playerCount; i++)
                {
                    bool lost = (bool) playersLost[PhotonNetwork.PlayerList[i].ActorNumber];
                    playersLostAr[i] = lost;

                    if (!lost) playersStanding++;
                }

                if(playerCount > 1)
                {
                    return playersStanding <= 1;
                }
                else
                {
                    return playersStanding <= 0;
                }
            }

            /// <summary>
            /// Checks and returns true if all players are ready to move on.
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            private bool RemotePlayersReady(string message)
            {
                bool allReady = true;

                Debug.Log(message);

                foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    allReady = allReady && (bool)playersReady[p.ActorNumber];
                }

                return allReady;
            }

            /// <summary>
            /// Checks if the local player is ready for the next phase.
            /// </summary>
            /// <returns></returns>
            private bool LocalPlayerReady()
            {
                return localPlayerReady;
            }

            /// <summary>
            /// Generates a random map of sequences.
            /// </summary>
            private void GenerateSequenceMap()
            {
                for(int i = 0; i < sequenceMap.Length; i++)
                {
                    sequenceMap[i] = Random.Range(1, 5);
                }
            }

            private void RerandomizeSequenceMap(int pos)
            {
                sequenceMap[pos] = Random.Range(1, 5);
                sequenceMap[pos + 1] = Random.Range(1, 5);
            }

            /// <summary>
            /// Sends the sequence map generated across the network.
            /// </summary>
            private void SendSequenceMap()
            {
                photonView.RPC("RPC_SendSequenceMap", RpcTarget.Others, sequenceMap);
            }

            #endregion

            #region Public Functions

            public void NotifyOfLocalPlayerReady()
            {
                Debug.Log("GM Notify finish");
                localPlayerReady = true;
            }

            /// <summary>
            /// Notifies other clients that this player is ready.
            /// </summary>
            /// <param name="playerId"></param>
            public void NotifyOfRemotePlayerReady(int playerId)
            {
                playersReady.Remove(playerId);
                playersReady[playerId] = true;

                photonView.RPC("RPC_SendPlayerReady", RpcTarget.Others, playerId);
            }

            /// <summary>
            /// Notifies other clients that this player has lost.
            /// </summary>
            /// <param name="playerId"></param>
            public void NotifyOfPlayerLost(int playerId)
            {
                playersLost.Remove(playerId);
                playersLost[playerId] = true;

                photonView.RPC("RPC_SendPlayerLost", RpcTarget.Others, playerId);
            }

            #endregion

            #region PUNRPC

            [PunRPC]
            public void RPC_SendPlayerReady(int id)
            {
                playersReady.Remove(id);
                playersReady[id] = true;
            }

            [PunRPC]
            public void RPC_SendPlayerLost(int id)
            {
                playersLost.Remove(id);
                playersLost[id] = true;
            }

            [PunRPC]
            public void RPC_SendSequenceMap(int[] sequence)
            {
                sequenceMap = sequence;
            }

            #endregion
        }
    }
}


