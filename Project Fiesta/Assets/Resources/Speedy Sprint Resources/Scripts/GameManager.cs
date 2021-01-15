using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

namespace FiestaTime
{
    namespace SS
    {
        public class GameManager : FiestaGameManager<GameManager, float>
        {
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

            public bool GameBegan { get; private set; }
            public float InGameTime { get; private set; }
            public float ZToBe { get; private set; }

            private float _gravityMovingRatio = 0.46666666666f;
            private float _logValue = 8.02255787562f;

            private int _nextToInsert = 0;
            private float _timeElapsed;
            private bool _isHighScore;

            [SerializeField] private GameObject _gameCamera;
            [SerializeField] private GameObject _proceduralGenerator;

            private float _startTime;
            private float _masterClientAvgDeltaTime;
            private float realGameStartCountdown ;

            public static string SubsectionsPath = "Speedy Sprint Resources/Prefabs/Resources/Sub-Sections/";

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
            }

            protected override void InStart()
            {
                GameBegan = false;
                _gravity = -15;
                MovingSpeed = 0f;
                InGameTime = 0f;
                ZToBe = -10f;
                if(PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) realGameStartCountdown  = gameStartCountdown + 1;
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
                if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;

                if (GameBegan)
                {
                    MovingSpeed = 13.2f * Mathf.Log(0.6f * Mathf.Pow(InGameTime + _logValue, 0.5f));
                    Gravity = -1 * (MovingSpeed / _gravityMovingRatio);
                    InGameTime += Time.deltaTime;
                    ZToBe += MovingSpeed * Time.deltaTime;
                    photonView.RPC("RPC_SendZToBe", RpcTarget.Others, ZToBe);
                    photonView.RPC("RPC_SendInGameTime", RpcTarget.Others, InGameTime);
                }

                if (!GameBegan && realGameStartCountdown <= 0f)
                {
                    GameBegan = true;
                    photonView.RPC("RPC_SendBegin", RpcTarget.Others, GameBegan);
                }
                else if (!GameBegan)
                {
                    realGameStartCountdown -= Time.deltaTime;
                }
            }

            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 1f, -10f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-5f, 1f, -10f);
                        playerPositions[1] = new Vector3(5f, 1f, -10f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-10f, 1f, -10f);
                        playerPositions[1] = new Vector3(0f, 1f, -10f);
                        playerPositions[2] = new Vector3(10f, 1f, -10f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-15f, 1f, -10f);
                        playerPositions[1] = new Vector3(-5f, 1f, -10f);
                        playerPositions[2] = new Vector3(5f, 1f, -10f);
                        playerPositions[3] = new Vector3(15f, 1f, -10f);
                        break;
                    default:
                        break;
                }
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
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            private void OnPlayerLost(int playerId)
            {
                PlayerResults<float> results = new PlayerResults<float>();
                results.playerId = playerId;
                results.scoring = _timeElapsed;
                playerResults[_nextToInsert] = results;
                _nextToInsert++;

                if (PhotonNetwork.LocalPlayer.ActorNumber == playerId) _isHighScore = GeneralHelperFunctions.DetermineHighScoreFloat(Constants.SS_KEY_HISCORE, results.scoring, true);

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
            }

            [PunRPC]
            public void RPC_SendBegin(bool gameBegan)
            {
                GameBegan = gameBegan;
            }

            [PunRPC]
            public void RPC_SendInGameTime(float time)
            {
                InGameTime = time;
            }

            [PunRPC]
            public void RPC_SendZToBe(float z)
            {
                ZToBe = z;
            }

        }
    }
}