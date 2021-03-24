using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace PartyInc
{
    namespace LL
    {
        public class Mng_GameManager_LL : FiestaGameManager<Mng_GameManager_LL, int>
        {
            private List<PlayerResults<int>> _deadPlayers = new List<PlayerResults<int>>();

            //public bool GotSeed { get; private set; }

            private float _startingSpeed = 4f;
            private float _finalSpeed = 15f;
            private float _timeToReachFinalSpeedInSeconds = 80f;

            private double _gameBeginTime;

            [SerializeField] private float _movementSpeed;
            public float MovementSpeed
            {
                get
                {
                    return _movementSpeed;
                }
                set
                {
                    _movementSpeed = value;
                }
            }

            private float _startingMaxX = 12.6f; // -> 4/13 = r1, where 4 = starting game speed
            private float _finalMaxX = 15.1f; // -> 15/15.1 = r2, where 15 = final game speed
            private float _timeToReachFinalMaxXInSeconds = 80f; // Same as GameManager function

            [SerializeField] private float _maxX = 11.9f;
            public float MaxX
            {
                get
                {
                    return _maxX;
                }
                set
                {
                    _maxX = value;
                }
            }

            [SerializeField] private float _lastRecordedMovementSpeed;
            public float LastRecordedMovementSpeed
            {
                get
                {
                    return _lastRecordedMovementSpeed;
                }
                set
                {
                    _lastRecordedMovementSpeed = value;
                }
            }

            [SerializeField] private float _gravity;

            private int _currentGate;
            public int CurrentGate
            {
                get
                {
                    return _currentGate;
                }
                private set
                {
                    _currentGate = value;
                }
            }

            public float MyPlayerZ { get; private set; } = -1f;

            protected override void InStart()
            {
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
                {
                    //int seed = System.Environment.TickCount;

                    //Random.InitState(seed);

                    //GotSeed = true;
                    ////photonView.RPC("SendSeed_RPC", RpcTarget.Others, seed);
                }
                else if (!PhotonNetwork.IsConnected)
                {
                    //int seed = System.Environment.TickCount;

                    //Random.InitState(seed);

                    //GotSeed = true;
                }
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
            }

            public override void Init()
            {
                base.Init();

                Physics.gravity = new Vector3(0f, _gravity, 0f);

                Mono_ObstaclePassCheck_LL.onPlayerPassed += OnPlayerPassedGate;
                PhotonNetwork.NetworkingClient.EventReceived += OnPlayerDied;

                GameDisplayName = Stt_GameNames.GAMENAME_LL;
                GameDBName = Stt_GameNames.GAMENAME_DB_LL;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_LL.onPlayerPassed -= OnPlayerPassedGate;
                PhotonNetwork.NetworkingClient.EventReceived -= OnPlayerDied;
            }

            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                for (int i = 0; i < playerCount; i++)
                {
                    if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                        MyPlayerZ = playerPositions[i].z;
                    }
                }

                PhotonNetwork.Instantiate("_player/" + playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(-11f, -1.5f, 0f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-11f, 0, 0f);
                        playerPositions[1] = new Vector3(-11f, -3f, 2f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-11f, 2f, 0f);
                        playerPositions[1] = new Vector3(-11f, -1f, 2f);
                        playerPositions[2] = new Vector3(-11f, -4f, 4f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-11f, 3.5f, 0f);
                        playerPositions[1] = new Vector3(-11f, 0.5f, 2f);
                        playerPositions[2] = new Vector3(-11f, -2.5f, 4f);
                        playerPositions[3] = new Vector3(-11f, -5.5f, 6f);
                        break;
                    default:
                        break;
                }
            }

            private void Update()
            {
                if (PhotonNetwork.IsConnectedAndReady && _startCountdown && !GameBegan)
                {
                    if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 1f)
                    {
                        GameBegan = true;
                        _gameBeginTime = PhotonNetwork.Time;
                        if (PhotonNetwork.IsConnectedAndReady) photonView.RPC("RPC_SendBegin", RpcTarget.Others, _gameBeginTime);
                    }
                }else if (_startCountdown && !GameBegan)
                {
                    GameBegan = true;
                }

                if (GameBegan)
                {
                    if (PhotonNetwork.IsConnectedAndReady) InGameTime = (float)(PhotonNetwork.Time - _gameBeginTime);
                    else InGameTime += Time.deltaTime;

                    _lastRecordedMovementSpeed = _movementSpeed;

                    if (MovementSpeed <= _finalSpeed)
                    {
                        MovementSpeed = _startingSpeed + InGameTime * ((_finalSpeed - _startingSpeed) / _timeToReachFinalSpeedInSeconds);
                    }

                    if(_deadPlayers.Count >= playerCount && PhotonNetwork.IsConnected)
                    {
                        FinishGame();
                    }
                }
            }

            private void FinishGame()
            {
                GameBegan = false;

                playerResults = _deadPlayers.ToArray();
                WinnerId = DetermineWinner();

                OnGameFinishInvoke();
            }

            private int DetermineWinner()
            {
                var ordered = playerResults.OrderByDescending(p => p.scoring);
                PlayerResults<int>[] o = ordered.ToArray();

                for (int i = 1; i < o.Length; i++)
                {
                    if (o[0].Equals(o[1]))
                    {
                        return -1;
                    }
                }

                return o[0].playerId;
            }

            private void OnPlayerPassedGate()
            {
                CurrentGate++;
            }


            //// NETWORKING
            ///

            [PunRPC]
            public void RPC_SendBegin(double startT)
            {
                _gameBeginTime = startT;
            }

            private void OnPlayerDied(EventData eventData)
            {
                if(eventData.Code == Constants.PlayerDiedEventCode)
                {
                    object[] data = (object[])eventData.CustomData;
                    PlayerResults<int> res = new PlayerResults<int>();

                    res.scoring = (int)data[0];
                    res.playerId = (int)data[1];

                    _deadPlayers.Add(res);
                }
            }
        }
    }
}