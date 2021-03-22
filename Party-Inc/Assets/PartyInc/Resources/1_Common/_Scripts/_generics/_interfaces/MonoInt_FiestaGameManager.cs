using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    /// <summary>
    /// This class defines a PartyInc GameManager, and posseses everything every GameManager probably has.
    /// One should be very careful when changing this piece of code, since it ties to all the games.
    /// I know this is bad design, but its a sacrifice im willing to make, since its a great to be able to introduce all the basic things
    /// I need every time I make a new game.
    /// </summary>
    /// <typeparam name="G"> The GameManager type </typeparam>
    /// <typeparam name="T"> The Scoring Type </typeparam>
    public abstract class FiestaGameManager<G, T> : MonoSingleton<G> where G : MonoSingleton<G>
    {
        public string GameName { get; protected set; }

        // The network controller in the scene.
        protected Mono_GameMetadata gameMetadata;
        protected Mono_NetworkGameRoomController networkController;

        // The results of the game, every game has results and winners.
        public PlayerResults<T>[] playerResults = new PlayerResults<T>[4];
        public List<PlayerResults<T>> ProvisoryPlayerResults = new List<PlayerResults<T>>();

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
            Debug.Log("WHAT THE FUCK IS GOING ON");
            Debug.Log(typeof(G).ToString());
            Debug.Log(typeof(T).ToString());

            Debug.Log("Default GM Start");

            gameMetadata = FindObjectOfType<Mono_GameMetadata>();
            networkController = FindObjectOfType<Mono_NetworkGameRoomController>();

            gameMetadata.PlayerCount = playerCount;

            if(networkController == null && PhotonNetwork.IsConnected)
            {
                Debug.LogError("CRITICAL: No Network Controller found in scene.");
            }

            playerResults = new PlayerResults<T>[playerCount];

            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                _startCountdown = true;

                if (PhotonNetwork.IsConnected)
                {
                    CustomProps = new ExitGames.Client.Photon.Hashtable();
                    _startTime = PhotonNetwork.Time;
                    CustomProps.Add("StartTime", _startTime);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(CustomProps);
                }
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

            PhotonNetwork.NetworkingClient.EventReceived += GetPlayerResult;

            Debug.Log("Default GM Awake");
            playerCount = PhotonNetwork.PlayerList.Length;
            PlayersConnectedAndReady = false;

            if(PhotonNetwork.SendRate != 10)
            {
                PhotonNetwork.SendRate = 10;
                PhotonNetwork.SerializationRate = 10;
            }
        }

        private void OnDestroy()
        {

            OnDestroyed();
        }

        public virtual void OnDestroyed()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= GetPlayerResult;
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
        /// Does not run OnGameFinishInvoke() within.
        /// </summary>
        /// <param name="descending"></param>
        protected IEnumerator GameFinish(bool biggerScoreWins)
        {
            // Begin loading end scene, as getting the player results will take a tiny bit of time.
            AsyncOperation scene = SceneManager.LoadSceneAsync(Stt_SceneIndexes.GAMERESULTS, LoadSceneMode.Additive);
            scene.allowSceneActivation = false;

            //Must get all the player results from the net (each client should have their own score tracked)
            //Order the list based on descending variable.
            //Get the winner from it
            //Activate the game end screen and pass by the ordered list of player results
            if (PhotonNetwork.IsMasterClient)
            {
                object[] content = new object[] { };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(74, content, raiseEventOptions, SendOptions.SendReliable);
            }

            Debug.Log("Sent for results");

            yield return new WaitUntil(() => ProvisoryPlayerResults.Count() == playerCount);

            playerResults = ProvisoryPlayerResults.ToArray();

            var aux = playerResults.OrderByDescending(result => result.scoring);
            // Order list
            if (!biggerScoreWins)
            {
                aux = playerResults.OrderBy(result => result.scoring);
            }
            
            playerResults = aux.ToArray();

            // Find a winner
            FindWinner();

            // Load up metadata:
            gameMetadata.GameName = GameName;
            gameMetadata.WinnerId = WinnerId;
            gameMetadata.DescendingCondition = biggerScoreWins;

            if (typeof(T) == typeof(int))
            {
                gameMetadata.ScoreType = ScoreType.Int;
                PlayerResults<int>[] res = new PlayerResults<int>[playerCount];

                foreach(PlayerResults<T> val in playerResults)
                {
                    PlayerResults<int> newPR = new PlayerResults<int>();
                    newPR.playerId = val.playerId;
                    newPR.reachedEnd = val.reachedEnd;
                    newPR.scoring = (int)Convert.ChangeType(val.scoring, typeof(int));
                }

                gameMetadata.PlayerResultsInt = res;
                foreach (PlayerResults<int> a in gameMetadata.PlayerResultsInt)
                {
                    Debug.Log(a.ToString());
                }
            }
            else
            {
                gameMetadata.ScoreType = ScoreType.Float;
                PlayerResults<float>[] res = new PlayerResults<float>[playerCount];

                foreach (PlayerResults<T> val in playerResults)
                {
                    PlayerResults<float> newPR = new PlayerResults<float>();
                    newPR.playerId = val.playerId;
                    newPR.reachedEnd = val.reachedEnd;
                    newPR.scoring = (float)Convert.ChangeType(val.scoring, typeof(float));
                }

                gameMetadata.PlayerResultsFloat = res;
            }

            scene.allowSceneActivation = true;

            // Load up the game end screen.
            yield return new WaitUntil(() => scene.isDone);
        }

        /// <summary>
        /// Function that finds who is the winner.
        /// </summary>
        private void FindWinner()
        {
            T contenderScore = playerResults.First().scoring;
            int contender = playerResults.First().playerId;
            int hap = 0;

            for (int i = 0; i < playerResults.Count(); i++)
            {
                if (playerResults[i].scoring.Equals(contenderScore)) hap++;
            }

            if (hap > 1) contender = -1;

            WinnerId = contender;
        }

        private void GetPlayerResult(EventData eventData)
        {
            if (eventData.Code == 75)
            {
                object[] data = (object[])eventData.CustomData;

                PlayerResults<T> thisPlayersResult = new PlayerResults<T>();
                thisPlayersResult.playerId = (int)data[0];
                thisPlayersResult.scoring = (T)data[1];

                ProvisoryPlayerResults.Add(thisPlayersResult);
            }
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
            Debug.Log("PartyInc/GameManager: Custom Properties changed.");
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            _receivedProperties = true;
            CustomProps = propertiesThatChanged;
        }
    }


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

    public enum ScoreType
    {
        Float,
        Int,
    }
}


