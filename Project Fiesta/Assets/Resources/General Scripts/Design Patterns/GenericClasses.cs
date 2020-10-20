using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    public struct PlayerResults<T>
    {
        public T scoring;
        public int playerId;
        public bool reachedEnd;

        public override bool Equals(object obj)
        {
            PlayerResults<T> a = (PlayerResults<T>)obj;
            return a.scoring.Equals(scoring);
        }

        public override int GetHashCode()
        {
            return playerId * 17;
        }

        public override string ToString()
        {
            return "PlayerID: " + playerId + " , scoring: " + scoring;
        }
    }

    /// <summary>
    /// This class defines a FiestaTime GameManager, and posseses everything every GameManager probably has.
    /// </summary>
    /// <typeparam name="G"> The GameManager type </typeparam>
    /// <typeparam name="T"> The Scoring Type </typeparam>
    public class FiestaGameManager<G, T> : MonoSingleton<G> where G : MonoSingleton<G>
    {
        // The network controller in the scene.
        protected NetworkGameRoomController networkController;

        // The results of the game, every game has results and winners.
        protected PlayerResults<T>[] playerResults = new PlayerResults<T>[4];

        // Every game has a start.
        public delegate void ActionGameStart();
        public static event ActionGameStart onGameStart;

        // And end.
        public delegate void ActionGameFinish();
        public static event ActionGameFinish onGameFinish;

        // All games have players and UI.
        [SerializeField] protected GameObject playerPrefab;
        [SerializeField] protected GameObject UIPrefab;

        // Every game has a count of players.
        protected int playerCount;

        // Every game has a players that start somewhere.
        protected Vector3[] playerPositions = new Vector3[4];

        // I think all of our games will have some type of countdown.
        public float gameStartCountdown = 3f; //Have to take into account the start text

        private void Start()
        {
            networkController = FindObjectOfType<NetworkGameRoomController>();

            if(networkController == null && PhotonNetwork.IsConnected)
            {
                Debug.LogError("CRITICAL: No Network Controller found in scene.");
            }

            playerCount = PhotonNetwork.PlayerList.Length;
            playerResults = new PlayerResults<T>[playerCount];

            InStart();
            StartCoroutine("AllPlayersReady");
        }

        /// <summary>
        /// Starts the game once all players are ready.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AllPlayersReady()
        {
            yield return new WaitUntil(() => networkController.playersAreReady);

            InitializeGameManagerDependantObjects();
        }

        protected void OnGameStartInvoke()
        {
            onGameStart?.Invoke();
        }

        protected void OnGameFinishInvoke()
        {
            onGameFinish?.Invoke();
        }

        /// <summary>
        /// Use this instead of start
        /// </summary>
        protected virtual void InStart()
        {

        }

        /// <summary>
        /// Put in here the instantiations of everything you know depends on the game managers existance and isnt already on the scene. (Like players)
        /// Probably networked objects will lie here.
        /// This method will run after every player has connected.
        /// </summary>
        protected virtual void InitializeGameManagerDependantObjects()
        {

        }
    }
}


