﻿using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace PartyInc
{
    namespace SS
    {
        public class Mng_GameManager_SS : FiestaGameManager<Mng_GameManager_SS, float>
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

            public static string SubsectionsPath = "2_Games/5_SpeedySprint/_Subsections/";

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

                //Mono_Player_Controller_SS.onPlayerDied += OnPlayerLost;
                Mono_Player_Controller_SS.onPlayerDied += OnPlayerDied;

                GameDisplayName = Stt_GameNames.GAMENAME_SS;
                GameDBName = Stt_GameNames.GAMENAME_DB_SS;
            }

            private void OnDestroy()
            {
                //Mono_Player_Controller_SS.onPlayerDied -= OnPlayerLost;
                Mono_Player_Controller_SS.onPlayerDied -= OnPlayerDied;
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
                        if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 1f)
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
                if (_playersAlive <= 0 && !_runOnce && PhotonNetwork.IsConnectedAndReady) 
                {
                    _runOnce = true;
                    GameBegan = false;
                    MovingSpeed = 0f;

                    StartCoroutine(GameFinish(true));
                    //FinishGame();
                }
            }

            private void OnPlayerDied()
            {
                Debug.Log("OnPlayerDied");
                _playersAlive--;
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
                        playerPositions[0] = new Vector3(-16f, 1f, -10f);
                        playerPositions[1] = new Vector3(-6f, 1f, -10f);
                        playerPositions[2] = new Vector3(4f, 1f, -10f);
                        playerPositions[3] = new Vector3(14f, 1f, -10f);
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

                PhotonNetwork.Instantiate("_player/" + playerPrefab.name, decidedPosition, Quaternion.identity);
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