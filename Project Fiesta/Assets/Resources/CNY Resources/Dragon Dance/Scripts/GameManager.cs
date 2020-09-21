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
                // Generate the sequence map to be followed.
                if (PhotonNetwork.IsMasterClient)
                {
                    GenerateSequenceMap();
                    SendSequenceMap();
                }

                // Initialize game for x amount of players.
                InitializeGame();

                SetPlayerIDs();

                // Initialize players
                InitializePlayers();

                // Initialize player UI
                InitializeUI();

                // Start game
                StartCoroutine(GameLoopCo());
            }

            private void SetPlayerIDs()
            {
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    Debug.Log(PhotonNetwork.PlayerList[i].ActorNumber);
                }

                SetPlayersLost();
                ResetPlayersReady();

                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    Debug.Log(playersReady[PhotonNetwork.PlayerList[i].ActorNumber]);
                }
            }

            private void InitializeGame()
            {
                amountOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            }

            private void ResetPlayersReady()
            {
                foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    playersReady.Remove(p.ActorNumber);
                    playersReady.Add(p.ActorNumber, false);
                }
            }

            private void SetPlayersLost()
            {
                foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    playersLost.Remove(p.ActorNumber);
                    playersLost.Add(p.ActorNumber, false);
                }
            }

            private void InitializeUI()
            {
                Instantiate(UIPrefab);
            }

            private void InitializePlayers()
            {
                Vector3 decidedVec = Vector3.zero;

                for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if(PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
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
                yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on UI instantiation."));
                ResetPlayersReady();

                while (isGameRunning)
                {
                    // Trigger "Sequence Showing" Sequence
                    onNextPhase?.Invoke(0);
                    yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Sequence Showing."));
                    ResetPlayersReady();

                    // Activate "Player Input" Sequence
                    onNextPhase?.Invoke(1);
                    yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Player Input."));
                    ResetPlayersReady();

                    // Activate "Player Demonstration" Sequence
                    onNextPhase?.Invoke(2);
                    yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Demonstration."));
                    ResetPlayersReady();

                    // Trigger "Results Showing" Sequence
                    onNextPhase?.Invoke(3);
                    yield return new WaitUntil(() => PlayersReady("Fiesta Time/ DD/ GameManager: Awaiting player confirmation on Results Showing."));
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

                for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    bool lost = (bool) playersLost[PhotonNetwork.PlayerList[i].ActorNumber];

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

            private bool PlayersReady(string message)
            {
                bool allReady = true;

                Debug.Log(message);

                foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    allReady = allReady && (bool)playersReady[p.ActorNumber];
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


