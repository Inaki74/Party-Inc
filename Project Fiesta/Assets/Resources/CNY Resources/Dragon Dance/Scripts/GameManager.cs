using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace DD
    {
        public class GameManager : MonoSingleton<GameManager>
        {
            private Vector3[] playerPositions = { new Vector3(-2.5f, 0.5f, -16f),
                                                  new Vector3(-0.75f, 0.5f, -16f),
                                                  new Vector3(0.75f, 0.5f, -16f),
                                                  new Vector3(2.5f, 0.5f, -16f)};

            //The sequence map of moves to be generated.
            public int[] sequenceMap = new int[12];

            private int amountOfPlayers;
            private int[] playerList;
            private Hashtable playersReady = new Hashtable();
            private Hashtable playersLost = new Hashtable();

            public delegate void ActionGameStart();
            public static event ActionGameStart onGameStart;

            public delegate void ActionSectionAdvance(int nextPhase);
            public static event ActionSectionAdvance onNextPhase;

            private bool isGameRunning = true;

            [SerializeField] private GameObject playerPrefab;
            [SerializeField] private GameObject UIPrefab;

            public int amountOfMovesThisRound = 4;

            // Start is called before the first frame update
            void Start()
            {
                // Start game
                StartCoroutine(GameLoopCo());
            }

            private IEnumerator SetPlayerIDs()
            {
                playerList = new int[PhotonNetwork.CurrentRoom.PlayerCount];

                while (!HasFetched(playerList))
                {
                    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("PlayerIDsList"))
                        playerList = (int[])PhotonNetwork.CurrentRoom.CustomProperties["PlayerIDsList"];

                    yield return new WaitForEndOfFrame();
                }

                //for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                //{
                //    Debug.Log(playerList[i]);
                //}

                SetPlayersLost();
                ResetPlayersReady();

                //for (int i = 0; i < playerList.Length; i++)
                //{
                //    Debug.Log(playersReady[playerList[i]]);
                //}
            }

            private bool HasFetched(int[] list)
            {
                foreach(int i in list)
                {
                    if (i == 0) return false;
                }

                return true;
            }

            private void InitializeGame()
            {
                amountOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            }

            private void ResetPlayersReady()
            {
                foreach (int id in playerList)
                {
                    playersReady.Remove(id);
                    playersReady.Add(id, false);
                }
            }

            private void SetPlayersLost()
            {
                foreach (int id in playerList)
                {
                    playersLost.Remove(id);
                    playersLost.Add(id, false);
                }
            }

            private void InitializeUI()
            {
                Instantiate(UIPrefab);
            }

            private void InitializePlayers()
            {
                Vector3 decidedVec = Vector3.zero;

                for(int i = 0; i < playerList.Length; i++)
                {
                    if(PhotonNetwork.LocalPlayer.ActorNumber == playerList[i])
                    {
                        decidedVec = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedVec, Quaternion.identity);
            }

            /// <summary>
            /// The game loop coroutine. Im using this because the WaitFor classes help a lot with the loop flow.
            /// </summary>
            /// <returns></returns>
            private IEnumerator GameLoopCo()
            {
                // Generate the sequence map to be followed.
                if (PhotonNetwork.IsMasterClient)
                {
                    GenerateSequenceMap();
                    SendSequenceMap();
                }

                // Initialize game for x amount of players.
                InitializeGame();

                yield return StartCoroutine(SetPlayerIDs());

                // Initialize players
                InitializePlayers();

                // Initialize player UI
                InitializeUI();

                yield return new WaitUntil(() => PlayersReady());
                ResetPlayersReady();

                while (isGameRunning)
                {
                    // Trigger "Sequence Showing" Sequence
                    onNextPhase?.Invoke(0);
                    yield return new WaitUntil(() => PlayersReady());
                    ResetPlayersReady();

                    // Activate "Player Input" Sequence
                    onNextPhase?.Invoke(1);
                    yield return new WaitUntil(() => PlayersReady());
                    ResetPlayersReady();

                    // Activate "Player Demonstration" Sequence
                    onNextPhase?.Invoke(2);
                    yield return new WaitUntil(() => PlayersReady());
                    ResetPlayersReady();

                    // Trigger "Results Showing" Sequence
                    onNextPhase?.Invoke(3);
                    yield return new WaitUntil(() => PlayersReady());
                    ResetPlayersReady();

                    amountOfMovesThisRound += 2;

                    isGameRunning = !CheckGameOver();
                }

                // Outro
                // TODO: Decide winner, put on a FINISH screen.
                onNextPhase?.Invoke(4);
            }

            /// <summary>
            /// Checks if the game is over. True if the game is over.
            /// </summary>
            /// <returns></returns>
            private bool CheckGameOver()
            {
                // Game is over if either of these are true:
                //      There is one player standing.
                //      There are no players standing.
                //      All rounds have been played.

                return amountOfMovesThisRound > sequenceMap.Length || CheckPlayersStanding();
            }

            /// <summary>
            /// Returns true if theres one or less players standing (game is over).
            /// </summary>
            /// <returns></returns>
            private bool CheckPlayersStanding()
            {
                int playersStanding = 0;

                for(int i = 0; i < playerList.Length; i++)
                {
                    bool lost = (bool) playersLost[playerList[i]];

                    if (!lost) playersStanding++;
                }

                if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    return playersStanding <= 1;
                }
                else
                {
                    return playersStanding <= 0;
                }
            }

            private bool PlayersReady()
            {
                bool allReady = true;

                foreach(int id in playerList)
                {
                    allReady = allReady && (bool)playersReady[id];
                }

                return allReady;
            }

            private void GenerateSequenceMap()
            {
                for(int i = 0; i < sequenceMap.Length; i++)
                {
                    sequenceMap[i] = Random.Range(1, 5);
                }
            }

            private void SendSequenceMap()
            {
                photonView.RPC("RPC_SendSequenceMap", RpcTarget.Others, sequenceMap);
            }

            public void NotifyOfPlayerReady(int playerId)
            {
                playersReady.Remove(playerId);
                playersReady[playerId] = true;

                photonView.RPC("RPC_SendPlayerReady", RpcTarget.Others, playerId);
            }

            public void NotifyOfPlayerLost(int playerId)
            {
                playersLost.Remove(playerId);
                playersLost[playerId] = true;

                photonView.RPC("RPC_SendPlayerLost", RpcTarget.Others, playerId);
            }

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


