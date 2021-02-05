using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace FiestaTime
{
    namespace CC
    {
        public class GameManager : FiestaGameManager<GameManager, int>
        {
            public delegate void ActionSpawnLog(float p, float a, float t);
            public static event ActionSpawnLog onLogSpawned;

            [SerializeField] private GameObject _logSpawner;

            private float _windowTime; // -t/(h/a-b) + a
            private float h = 60f; // Time for us to reach min window time
            private float a = 1f; // Starting window time
            private float b = 0.5f; // Min window time

            public GameObject test;

            public const byte SpawnLogEventCode = 1;
            public const byte NextLogWaveEventCode = 2;

            private float _currentTime;
            private int _wave;

            private List<int> _sliced = new List<int>();
            public List<int> Sliced
            {
                get
                {
                    return _sliced;
                }
                set
                {
                    _sliced = value;
                }
            }

            public bool Testing { get; set; }

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
                InitializeSpawners();
            }

            protected override void InStart()
            {
                GameBegan = false;
                _windowTime = 1f;
            }

            public override void Init()
            {
                base.Init();

                PhotonNetwork.NetworkingClient.EventReceived += SpawnLog;
                PhotonNetwork.NetworkingClient.EventReceived += ClearSliced;
                LogController.onLogDestroyed += SendSliced;
            }

            private void OnDestroy()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= SpawnLog;
                PhotonNetwork.NetworkingClient.EventReceived -= ClearSliced;
                LogController.onLogDestroyed -= SendSliced;
            }

            // Update is called once per frame
            void Update()
            {
                if (PhotonNetwork.IsConnectedAndReady && _startCountdown && !GameBegan)
                {
                    if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 1f)
                    {
                        GameBegan = true;
                        // The first log spawned, run once only
                        EventData a = new EventData();
                        a.Code = NextLogWaveEventCode;
                        SpawnLog(a);
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

                if (GameBegan && !Testing)
                {
                    InGameTime += Time.deltaTime;

                    if(InGameTime < h)
                    {
                        float div = h / (a - b);
                        _windowTime = (-InGameTime / div) + a;
                    }
                    else
                    {
                        _windowTime = b;
                    }
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
                        playerPositions[0] = new Vector3(0f, 1.2f, 5f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(4f, 1.2f, 5f);
                        playerPositions[1] = new Vector3(-4f, 1.2f, 5f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(4f, 1.2f, 5f);
                        playerPositions[1] = new Vector3(0f, 1.2f, 5f);
                        playerPositions[2] = new Vector3(-4f, 1.2f, 5f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(6f, 1.2f, 5f);
                        playerPositions[1] = new Vector3(2f, 1.2f, 5f);
                        playerPositions[2] = new Vector3(-2f, 1.2f, 5f);
                        playerPositions[3] = new Vector3(-6f, 1.2f, 5f);
                        break;
                    default:
                        break;
                }
            }

            private void ClearSliced(EventData data)
            {
                if (data.Code == NextLogWaveEventCode)
                {
                    Sliced.Clear();
                }
            }

            private void SpawnLog(EventData eventData)
            {
                if (eventData.Code == NextLogWaveEventCode && PhotonNetwork.IsMasterClient)
                {
                    _wave += 1;
                    // Any wait time that is greater than the serverLag * 2 will mean instant spawn. Approx 100ms at most 500ms (very bad server ping (250))
                    //Debug.Log("WAVE: " + _wave + ", TIME: " + InGameTime + " , WINDOW: " + _windowTime);
                    float waitTime = 0f;
                    float windowTime = _windowTime;

                    int logType = Random.Range(0, 5); // 0 -> Large, 1 -> Normal, 2 -> Small H, 3 -> Small V, 4 -> VSmall H, 5 -> VSmall V

                    float markAngle = DecideMarkAngle(logType);

                    float markPos = DecideMarkPosition(logType, markAngle);

                    object[] content = new object[] { waitTime, windowTime, logType, markPos, markAngle, PhotonNetwork.Time };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(SpawnLogEventCode, content, raiseEventOptions, SendOptions.SendReliable);
                    _currentTime = 0f;
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
                if (Random.Range(0, 2) == 0)
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

            public void SendSliced()
            {
                photonView.RPC("RPC_SendSliced", RpcTarget.MasterClient);
            }

            public void TestSetWindowTime(float set)
            {
                _windowTime = set;
            }

            /// NETWORKING
            ///
            [PunRPC]
            public void RPC_SendSliced(PhotonMessageInfo info)
            {
                Sliced.Add(info.Sender.ActorNumber);
            }
        }
    }
}


