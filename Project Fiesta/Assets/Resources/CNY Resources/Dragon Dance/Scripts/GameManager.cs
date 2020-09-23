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
            private Vector3[] playerPositions;

            //The sequence map of moves to be generated.
            public int[] sequenceMap = new int[12];

            private float amountOfPlayers;
            private Hashtable playersReady = new Hashtable();
            private Hashtable playersLost = new Hashtable();

            #region Events

            public delegate void ActionGameStart();
            public static event ActionGameStart onGameStart;

            public delegate void ActionSectionAdvance(int nextPhase);
            public static event ActionSectionAdvance onNextPhase;

            #endregion

            private bool isGameRunning = true;

            [SerializeField] private GameObject playerPrefab;
            [SerializeField] private GameObject UIPrefab;

            public int amountOfMovesThisRound = 4;

            #region Game Loop

            void Start()
            {
                // Generate the sequence map to be followed.
                if (PhotonNetwork.IsMasterClient)
                {
                    GenerateSequenceMap();
                    SendSequenceMap();
                }

                // Set that no one lost yet.
                SetPlayersLost();

                // Set that no one is ready yet.
                ResetPlayersReady();

                // Initialize players
                InitializePlayers();

                // Initialize player UI
                Instantiate(UIPrefab);

                // Start game
                StartCoroutine(GameLoopCo());
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

            #endregion

            #region Private Functions

            /// <summary>
            /// Initializes players in their positions.
            /// </summary>
            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedVec = Vector3.zero;

                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
                    {
                        decidedVec = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedVec, Quaternion.identity);
            }

            /// <summary>
            /// Sets the player position vectors.
            /// </summary>
            private void SetPlayerPositions()
            {
                switch (PhotonNetwork.CurrentRoom.PlayerCount)
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
                        playerPositions[1] = new Vector3(-0.75f, 0.5f, -16f);
                        playerPositions[2] = new Vector3(0.75f, 0.5f, -16f);
                        playerPositions[3] = new Vector3(2.5f, 0.5f, -16f);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Resets the state of the PlayersReady array (all false).
            /// </summary>
            private void ResetPlayersReady()
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

            /// <summary>
            /// Checks and returns true if all players are ready to move on.
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
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

            /// <summary>
            /// Sends the sequence map generated across the network.
            /// </summary>
            private void SendSequenceMap()
            {
                photonView.RPC("RPC_SendSequenceMap", RpcTarget.Others, sequenceMap);
            }

            #endregion

            #region Public Functions

            /// <summary>
            /// Notifies other clients that this player is ready.
            /// </summary>
            /// <param name="playerId"></param>
            public void NotifyOfPlayerReady(int playerId)
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


