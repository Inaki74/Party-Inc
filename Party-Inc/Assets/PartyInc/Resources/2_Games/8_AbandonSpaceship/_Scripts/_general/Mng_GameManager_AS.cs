using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace PartyInc
{
    namespace AS
    {
        public class Mng_GameManager_AS : FiestaGameManager<Mng_GameManager_AS, int>
        {
            private List<PlayerResults<int>> _deadPlayers = new List<PlayerResults<int>>();

            public float MyPlayerZ { get; private set; }

            public bool GotSeed { get; private set; }

            private float _startingSpeed = 4f;
            private float _finalSpeed = 15f;
            private float _timeToReachFinalSpeedInSeconds = 80f;

            private double _gameBeginTime;

            private float _playersLost;
            private bool _finishedGame;

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

            protected override void InStart()
            {
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
                {
                    int seed = System.Environment.TickCount;

                    Random.InitState(seed);

                    GotSeed = true;
                    photonView.RPC("SendSeed_RPC", RpcTarget.Others, seed);
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

                Mono_ObstaclePassCheck_AS.onPlayerPassed += OnPlayerPassedGate;
                Mono_Player_Controller_AS.onPlayerDied += OnPlayerDied;

                MyPlayerZ = -1f;

                GameName = Stt_GameNames.GAMENAME_AS;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_AS.onPlayerPassed -= OnPlayerPassedGate;
                Mono_Player_Controller_AS.onPlayerDied -= OnPlayerDied;
            }

            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                Debug.Log("Players");

                for (int i = 0; i < playerCount; i++)
                {
                    if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        Debug.Log("Positions");
                        decidedPosition = playerPositions[i];
                        MyPlayerZ = playerPositions[i].z;
                    }
                }

                Debug.Log("INSTANTIATE");
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
                }

                if (GameBegan)
                {
                    if (PhotonNetwork.IsConnectedAndReady) InGameTime = (float)(PhotonNetwork.Time - _gameBeginTime);
                    else InGameTime += Time.deltaTime;

                    if (MovementSpeed <= _finalSpeed)
                    {
                        MovementSpeed = _startingSpeed + InGameTime * ((_finalSpeed - _startingSpeed) / _timeToReachFinalSpeedInSeconds);
                    }

                    if(_playersLost >= playerCount && PhotonNetwork.IsConnected && !_finishedGame)
                    {
                        StartCoroutine(FinishGame());
                    }
                }
            }

            private IEnumerator FinishGame()
            {
                _finishedGame = true;

                yield return new WaitForSeconds(0.5f);

                StartCoroutine(GameFinish(true));
            }

            private void OnPlayerPassedGate()
            {
                CurrentGate++;
            }

            private void OnPlayerDied()
            {
                _playersLost++;
            }


            //// NETWORKING
            ///

            [PunRPC]
            public void SendSeed_RPC(int seed)
            {
                GotSeed = true;
                Random.InitState(seed);
            }

            [PunRPC]
            public void RPC_SendBegin(double startT)
            {
                _gameBeginTime = startT;
            }
        }
    }
}
