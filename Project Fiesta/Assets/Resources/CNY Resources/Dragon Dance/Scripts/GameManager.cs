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
            //The sequence map of moves to be generated.
            public int[] sequenceMap = new int[12];

            private int amountOfPlayers;
            private int[] playerList;
            private Hashtable playersReady = new Hashtable();

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
                // Generate the sequence map to be followed.
                GenerateSequenceMap();

                // Initialize game for x amount of players.
                InitializeGame();

                // Initialize player UI
                InitializeUI();

                // Initialize players
                InitializePlayers();

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

                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    Debug.Log(playerList[i]);
                }

                ResetPlayersReady();

                for (int i = 0; i < playerList.Length; i++)
                {
                    Debug.Log(playersReady[playerList[i]]);
                }
            }

            private void PrintArray(int[] a)
            {
                foreach(var i in a)
                {
                    Debug.Log(i);
                }
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

            private void InitializeUI()
            {
                Instantiate(UIPrefab);
            }

            private void InitializePlayers()
            {
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0.5f, -12), Quaternion.identity);
            }

            /// <summary>
            /// The game loop coroutine. Im using this because the WaitFor classes help a lot with the loop flow.
            /// </summary>
            /// <returns></returns>
            private IEnumerator GameLoopCo()
            {
                yield return StartCoroutine(SetPlayerIDs());

                while (isGameRunning)
                {
                    // Trigger "Sequence Showing" Sequence
                    Debug.Log("Commencing Sequence Showing");
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
                    // IsGameOver();

                }

                // Outro
            }

            private bool PlayersReady()
            {
                bool allReady = true;

                foreach(int id in playerList)
                {
                    allReady = allReady && (bool)playersReady[id];
                }
                Debug.Log("Players ready? " + allReady);

                return allReady;
            }

            private void GenerateSequenceMap()
            {
                for(int i = 0; i < sequenceMap.Length; i++)
                {
                    sequenceMap[i] = Random.Range(1, 5);
                }
            }

            public void NotifyOfPlayerReady(int playerId)
            {
                playersReady.Remove(playerId);
                playersReady[playerId] = true;

                photonView.RPC("RPC_SendPlayerReady", RpcTarget.Others, playerId);
            }

            private void OnPhaseTransit(int nextPhase)
            {
                // 0 -> entered showSeq, 1 -> entered playerInput, 2 -> entered demoSeq, 3 -> entered results
                

                switch (nextPhase)
                {
                    case 0:
                        // Deactivate all pieces of Results phase


                        // Activate all pieces of Showing Sequence phase


                        break;
                    case 1:
                        // Deactivate all pieces of Showing Sequence phase


                        // Activate all pieces of Input Phase phase


                        break;
                    case 2:
                        // Deactivate all pieces of Input phase


                        // Activate all pieces of demonstration phase


                        break;
                    case 3:
                        // Deactivate all pieces of demonstration phase


                        // Activate all pieces of Results phase


                        break;
                    default:
                        Debug.Log("ITS NOT POSSIBLEEEEE!");
                        break;
                }
            }

            #region PUNRPC

            [PunRPC]
            public void RPC_SendPlayerReady(int id)
            {
                playersReady.Remove(id);
                playersReady[id] = true;
            }

            #endregion
        }
    }
}


