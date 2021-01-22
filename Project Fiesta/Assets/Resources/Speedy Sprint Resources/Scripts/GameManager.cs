using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        public class GameManager : FiestaGameManager<GameManager, float>
        {
            // Moving Speed in all components
            [SerializeField] private float _movingSpeed;
            public float MovingSpeed {
                get
                {
                    return _movingSpeed;
                }
                private set
                {
                    _movingSpeed = value;
                }
            }

            // Gravity of the game
            [SerializeField] private float _gravity;
            public float Gravity
            {
                get
                {
                    return _gravity;
                }
                private set
                {
                    _gravity = value;
                }
            }

            // PhotonNetwork time in which the game began.
            private double _gameBeginTime;

            // Ration between MovingSpeed and Gravity
            private float _gravityMovingRatio = 0.46666666666f;

            // Value so that Moving Speed is 10 when time = 0
            private float _logValue = 10.2677451405f;
            //8.02255787562f

            private int _nextToInsert = 0;
            private int _playersAlive;
            private bool _runOnce = false;
            
            [SerializeField] private GameObject _gameCamera;
            [SerializeField] private GameObject _proceduralGenerator;

            private float _realGameStartCountdown;

            public static string SubsectionsPath = "Speedy Sprint Resources/Prefabs/Resources/Sub-Sections/";

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
            }

            protected override void InStart()
            {
                _playersAlive = playerCount;
                GameBegan = false;
                _startCountdown = false;
                _gravity = -15;
                InGameTime = 0f;
                
                if (!PhotonNetwork.IsConnectedAndReady) _realGameStartCountdown = gameStartCountdown; _startCountdown = true;
            }

            public override void Init()
            {
                base.Init();
                
                Player.onPlayerDied += OnPlayerLost;
            }

            private void OnDestroy()
            {
                Player.onPlayerDied -= OnPlayerLost;
            }

            // Update is called once per frame
            void Update()
            {
                if (GameBegan)
                {
                    // Increase the moving speed per design decisions.
                    if(!Testing) MovingSpeed = 5.5f * Mathf.Log(0.6f * (InGameTime + _logValue));
                    else MovingSpeed = _testMovingSpeed;

                    // Gravity must be increased with moving speed to maintain the jump lengths.
                    Gravity = -1 * (MovingSpeed / _gravityMovingRatio);

                    // InGameTime calculation
                    if (PhotonNetwork.IsConnectedAndReady) InGameTime = (float)(PhotonNetwork.Time - _gameBeginTime);
                    else InGameTime += Time.deltaTime;
                }
                else
                {
                    // TIMER to start

                    if (PhotonNetwork.IsConnectedAndReady && _startCountdown)
                    {
                        if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 0.5f)
                        {
                            GameBegan = true;
                            _gameBeginTime = PhotonNetwork.Time;
                            if (PhotonNetwork.IsConnectedAndReady) photonView.RPC("RPC_SendBegin", RpcTarget.Others, _gameBeginTime);
                        }
                    }
                    else if (_startCountdown)
                    {
                        if (_realGameStartCountdown <= -0.5f)
                        {
                            GameBegan = true;
                            _realGameStartCountdown = float.MaxValue;
                        }
                        else
                        {
                            _realGameStartCountdown -= Time.deltaTime;
                        }
                    }
                }

                // Game finish logic
                if (_playersAlive == 0 && !_runOnce && PhotonNetwork.IsConnectedAndReady) 
                {
                    _runOnce = true;
                    GameBegan = false;
                    MovingSpeed = 0f;

                    FinishGame();
                }
            }

            /// <summary>
            /// Function that finishes the game
            /// </summary>
            private void FinishGame()
            {
                // Order list
                var aux = playerResults.OrderByDescending(result => result.scoring);
                playerResults = aux.ToArray();

                // Find a winner
                FindWinner();

                // Invoke finishing functions
                OnGameFinishInvoke();
            }

            /// <summary>
            /// Function that finds who is the winner.
            /// </summary>
            private void FindWinner()
            {
                float contenderScore = playerResults.First().scoring;
                int contender = playerResults.First().playerId;
                int hap = 0;

                for (int i = 0; i < playerResults.Count(); i++)
                {
                    if (playerResults[i].scoring == contenderScore) hap++;
                }

                if (hap > 1) contender = -1;

                WinnerId = contender;
            }

            /// <summary>
            /// Sets player positions taking account amount of players.
            /// </summary>
            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 1f, -10f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-7f, 1f, -10f);
                        playerPositions[1] = new Vector3(7f, 1f, -10f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-12f, 1f, -10f);
                        playerPositions[1] = new Vector3(0f, 1f, -10f);
                        playerPositions[2] = new Vector3(12f, 1f, -10f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-18f, 1f, -10f);
                        playerPositions[1] = new Vector3(-6f, 1f, -10f);
                        playerPositions[2] = new Vector3(6f, 1f, -10f);
                        playerPositions[3] = new Vector3(18f, 1f, -10f);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Spawns players and gives them their position.
            /// </summary>
            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                for (int i = 0; i < playerCount; i++)
                {
                    if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            /// <summary>
            /// Event function triggered when a player loses.
            /// </summary>
            /// <param name="playerId"></param>
            private void OnPlayerLost(int playerId)
            {
                PlayerResults<float> results = new PlayerResults<float>();
                results.playerId = playerId;
                results.scoring = InGameTime;
                playerResults[_nextToInsert] = results;
                _nextToInsert++;
                _playersAlive--;

                if (PhotonNetwork.LocalPlayer.ActorNumber == playerId) IsHighScore = GeneralHelperFunctions.DetermineHighScoreFloat(Constants.SS_KEY_HISCORE, results.scoring, true);

                photonView.RPC("RPC_SendPlayerResult", RpcTarget.Others, results.playerId, results.scoring);
            }

            [PunRPC]
            public void RPC_SendPlayerResult(int playerId, float time)
            {
                PlayerResults<float> thisPlayerResult = new PlayerResults<float>();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.scoring = time;

                playerResults[_nextToInsert] = thisPlayerResult;
                _nextToInsert++;
                _playersAlive--;
            }

            [PunRPC]
            public void RPC_SendBegin(double startT)
            {
                _gameBeginTime = startT;
            }

            // TESTING
            public bool Testing;

            [SerializeField] private float _testMovingSpeed;

            public void TestSetMoveSpeed(float m)
            {
                _testMovingSpeed = m;
            }

        }
    }
}