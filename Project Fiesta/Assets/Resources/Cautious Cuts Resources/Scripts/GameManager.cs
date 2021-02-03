using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    namespace CC
    {
        public class GameManager : FiestaGameManager<GameManager, int>
        {
            public delegate void ActionSpawnLog(float p, float a, float t);
            public static event ActionSpawnLog onLogSpawned;

            [SerializeField] private GameObject _logSpawner;

            public GameObject test;

            public const byte SpawnLogEventCode = 1;
            private float _currentTime;

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
                InitializeSpawners();
            }

            protected override void InStart()
            {
                GameBegan = false;
            }

            public override void Init()
            {
                base.Init();
            }

            // Update is called once per frame
            void Update()
            {
                if (PhotonNetwork.IsConnectedAndReady && _startCountdown && !GameBegan)
                {
                    if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 1f)
                    {
                        GameBegan = true;
                    }
                }
                else if (_startCountdown && !GameBegan)
                {
                    if (gameStartCountdown <= -1f)
                    {
                        GameBegan = true;
                        gameStartCountdown = float.MaxValue;
                        // Start game
                    }
                    else
                    {
                        gameStartCountdown -= Time.deltaTime;
                    }
                }

                if(Input.touchCount > 0)
                {
                    if(Input.touches[0].phase == TouchPhase.Began)
                    {
                        //Instantiate(test, new Vector3(0f, -2f, -5f), Quaternion.identity);
                    }
                }

                if (GameBegan)
                {
                    SpawnLog();
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

            private void InitializeSpawners()
            {
                Vector3 decidedPosition = Vector3.zero;

                for (int i = 0; i < playerCount; i++)
                {
                    if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                        decidedPosition = new Vector3(decidedPosition.x, -10f, -5f);
                    }
                }

                PhotonNetwork.Instantiate(_logSpawner.name, decidedPosition, Quaternion.identity);
            }

            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 1.2f, 0f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(4f, 1.2f, 0f);
                        playerPositions[1] = new Vector3(-4f, 1.2f, 0f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(4f, 1.2f, 0f);
                        playerPositions[1] = new Vector3(0f, 1.2f, 0f);
                        playerPositions[2] = new Vector3(-4f, 1.2f, 0f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(6f, 1.2f, 0f);
                        playerPositions[1] = new Vector3(2f, 1.2f, 0f);
                        playerPositions[2] = new Vector3(-2f, 1.2f, 0f);
                        playerPositions[3] = new Vector3(-6f, 1.2f, 0f);
                        break;
                    default:
                        break;
                }
            }

            private void SpawnLog()
            {
                if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
                {
                    //logType: Randomized
                    //waitTime: customized
                    //window: x = -1/60 * (b - a) * t + a, a = startWindow, b = finalWindow

                    _currentTime += Time.deltaTime;

                    if (_currentTime > 4f)
                    {
                        float waitTime = 0f;
                        float windowTime = 0f;

                        int logType = Random.Range(0, 5); // 0 -> Large, 1 -> Normal, 2 -> Small H, 3 -> Small V, 4 -> VSmall H, 5 -> VSmall V

                        float markAngle = DecideMarkAngle(logType);

                        float markPos = DecideMarkPosition(logType, markAngle);

                        if (PhotonNetwork.IsConnected)
                        {
                            object[] content = new object[] { waitTime, windowTime, logType, markPos, markAngle, PhotonNetwork.Time };
                            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                            PhotonNetwork.RaiseEvent(SpawnLogEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
                            _currentTime = 0f;
                        }
                        else
                        {

                        }
                    }
                }
            }

            private float DecideMarkAngle(int logType)
            {
                float angleMax;
                float angleMin;

                if (logType == 3 || logType == 5)
                {
                    //Vert
                    angleMax = FallingLog.AngleMaxBoundV;
                    angleMin = FallingLog.AngleMinBoundV;
                }
                else
                {
                    angleMax = FallingLog.AngleBoundH;
                    angleMin = 0f;
                }

                int side;
                if (Random.Range(0, 1) == 0)
                {
                    side = -1;
                }
                else
                {
                    side = 1;
                }

                return Random.Range(angleMin, angleMax) * side;
            }

            private float DecideMarkPosition(int logType, float markAngle)
            {
                if (logType == 3 || logType == 5)
                {
                    // Vertical
                    float maxPos = (Mathf.Abs(markAngle) - 70f) / 100;
                    return Random.Range(-maxPos, maxPos);
                }
                else
                {
                    float maxPos;
                    float minPos;
                    // Horizontal
                    if (logType == 0)
                    {
                        // Large
                        maxPos = FallingLog.LargeBoundH;
                    }
                    else if (logType == 1)
                    {
                        // Normal
                        maxPos = FallingLog.MediumBoundH;
                    }
                    else if (logType == 2)
                    {
                        // Small
                        maxPos = FallingLog.SmallBoundH;
                    }
                    else
                    {
                        // V Small
                        maxPos = FallingLog.VerySmallBoundH;
                    }

                    minPos = -maxPos;

                    return Random.Range(minPos, maxPos);
                }
            }
        }
    }
}


