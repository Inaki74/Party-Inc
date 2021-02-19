using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace PlayInc
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

    public static class PlayerResultsHelper
    {
        public static void PrintArray(PlayerResults<int>[] pr)
        {
            foreach(var p in pr)
            {
                Debug.Log(p.ToString())
;            }
        }
    }

    /// <summary>
    /// This class defines a PlayInc GameManager, and posseses everything every GameManager probably has.
    /// One should be very careful when changing this piece of code, since it ties to all the games.
    /// I know this is bad design, but its a sacrifice im willing to make, since its a great to be able to introduce all the basic things
    /// I need every time I make a new game.
    /// </summary>
    /// <typeparam name="G"> The GameManager type </typeparam>
    /// <typeparam name="T"> The Scoring Type </typeparam>
    public abstract class FiestaGameManager<G, T> : MonoSingleton<G> where G : MonoSingleton<G>
    {
        // The network controller in the scene.
        protected NetworkGameRoomController networkController;

        // The results of the game, every game has results and winners.
        public PlayerResults<T>[] playerResults = new PlayerResults<T>[4];

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
        public int playerCount;

        // I think all of our games will have some type of countdown.
        public float gameStartCountdown = 3f; //Have to take into account the start text

        public bool PlayersConnectedAndReady { get; protected set; }

        public ExitGames.Client.Photon.Hashtable CustomProps { get; protected set; }

        public bool IsHighScore { get; set; }
        public int WinnerId { get; protected set; }
        public bool GameBegan { get; protected set; }
        public float InGameTime { get; protected set; }

        protected bool _receivedProperties;
        protected bool _startCountdown;
        protected double _startTime;

        // Every game has a players that start somewhere.
        protected Vector3[] playerPositions = new Vector3[4];

        private void Start()
        {
            Debug.Log("Default GM Start");

            networkController = FindObjectOfType<NetworkGameRoomController>();

            if(networkController == null && PhotonNetwork.IsConnected)
            {
                Debug.LogError("CRITICAL: No Network Controller found in scene.");
            }

            playerResults = new PlayerResults<T>[playerCount];

            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                CustomProps = new ExitGames.Client.Photon.Hashtable();
                _startTime = PhotonNetwork.Time;
                _startCountdown = true;
                CustomProps.Add("StartTime", _startTime);
                PhotonNetwork.CurrentRoom.SetCustomProperties(CustomProps);
            }
            else
            {
                StartCoroutine("WaitForPropertiesCo");
            }

            InStart();
            StartCoroutine("AllPlayersReady");
        }

        public override void Init()
        {
            base.Init();

            Debug.Log("Default GM Awake");
            playerCount = PhotonNetwork.PlayerList.Length;
            PlayersConnectedAndReady = false;
        }

        /// <summary>
        /// Starts the game once all players are ready.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AllPlayersReady()
        {
            yield return new WaitUntil(() => networkController.playersAreReady || !PhotonNetwork.IsConnected);

            PlayersConnectedAndReady = true;

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
        protected abstract void InStart();

        /// <summary>
        /// Put in here the instantiations of everything you know depends on the game managers existance and isnt already on the scene. (Like players)
        /// Probably networked objects will lie here.
        /// This method will run after every player has connected.
        /// </summary>
        protected abstract void InitializeGameManagerDependantObjects();

        private IEnumerator WaitForPropertiesCo()
        {
            yield return new WaitUntil(() => _receivedProperties);

            _startTime = double.Parse(CustomProps["StartTime"].ToString());
            _startCountdown = true;
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            Debug.Log("PlayInc/GameManager: Custom Properties changed.");
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            _receivedProperties = true;
            CustomProps = propertiesThatChanged;
        }
    }
}


