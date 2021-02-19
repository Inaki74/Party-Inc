using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

namespace PlayInc
{
    namespace CC
    {
        public class Mng_GameManager_CC : FiestaGameManager<Mng_GameManager_CC, float>
        {
            public delegate void ActionSpawnLog(float p, float a, float t);
            public static event ActionSpawnLog onLogSpawned;

            [SerializeField] private GameObject _logSpawner;

            private float _windowTime;

            private List<PlayerResults<float>> _provisoryPlayerResults = new List<PlayerResults<float>>();
            private int _wave = 0;

            private bool _runOnce;

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

                PhotonNetwork.Instantiate("_logs/" + _logSpawner.name, decidedPosition, Quaternion.identity);
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

            protected override void InStart()
            {
                GameBegan = false;
                _windowTime = 0.7f;
            }

            public override void Init()
            {
                base.Init();

                PhotonNetwork.NetworkingClient.EventReceived += SpawnLog;
                PhotonNetwork.NetworkingClient.EventReceived += ClearSliced;
                PhotonNetwork.NetworkingClient.EventReceived += GetPlayerResult;
                Mono_Log_Controller_CC.onLogDestroyed += SendSliced;
            }

            private void OnDestroy()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= SpawnLog;
                PhotonNetwork.NetworkingClient.EventReceived -= ClearSliced;
                PhotonNetwork.NetworkingClient.EventReceived -= GetPlayerResult;
                Mono_Log_Controller_CC.onLogDestroyed -= SendSliced;
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
                        a.Code = Constants.NextLogWaveEventCode;
                        SpawnLog(a);
                    }
                }

                if(_wave > Constants.AMOUNT_OF_LOGS_PER_MATCH && !_runOnce)
                {
                    GameBegan = false;
                    _runOnce = true;
                    StartCoroutine(FinishGame());
                }
            }

            /// <summary>
            /// Function that finishes the game
            /// </summary>
            private IEnumerator FinishGame()
            {
                //Get Player Results
                if (PhotonNetwork.IsMasterClient)
                {
                    object[] content = new object[] { };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(Constants.GivePlayerResultEventCode, content, raiseEventOptions, SendOptions.SendReliable);
                }
                
                yield return new WaitUntil(() => _provisoryPlayerResults.Count == playerCount);

                playerResults = _provisoryPlayerResults.ToArray();

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

            private void ClearSliced(EventData data)
            {
                if (data.Code == Constants.NextLogWaveEventCode)
                {
                    Sliced.Clear();
                }
            }

            //0 -> Large, 1 -> Normal, 2 -> Small H, 3 -> Small V, 4 -> VSmall H, 5 -> VSmall V
            private int DecideLogType(int wave)
            {
                int ret = 0;
                float w1 = 0f;
                float w2 = 0f;
                float w3 = 0f;

                if(wave <= 12)
                {
                    w1 = 0.65f;
                    w2 = 0.25f;
                    w3 = 0f;
                }
                else if(wave > 12 && wave <= 24)
                {
                    w1 = 0.15f;
                    w2 = 0.60f;
                    w3 = 0.15f;
                }
                else if(wave > 24)
                {
                    w1 = 0.10f;
                    w2 = 0.10f;
                    w3 = 0.70f;
                }

                float r = Random.value;

                if(r <= w1)
                {
                    ret = Random.Range(0, 2);
                }
                else if(r > w1 && r <= w1 + w2)
                {
                    if(Random.value > 0.5f)
                    {
                        ret = 2;
                    }
                    else
                    {
                        ret = 4;
                    }
                }
                else if(r > w1 + w2 && r <= w1 + w2 + w3)
                {
                    if (Random.value > 0.5f)
                    {
                        ret = 3;
                    }
                    else
                    {
                        ret = 5;
                    }
                }

                return ret;
            }

            private float DecideMarkAngle(int logType)
            {
                float angleMax;
                float angleMin;

                if (logType == 3 || logType == 5)
                {
                    //Vert
                    angleMax = Mono_Log_Info_CC.AngleMaxBoundV;
                    angleMin = Mono_Log_Info_CC.AngleMinBoundV;
                }
                else
                {
                    angleMax = Mono_Log_Info_CC.AngleBoundH;
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
                        maxPos = Mono_Log_Info_CC.LargeBoundH;
                    }
                    else if (logType == 1)
                    {
                        // Normal
                        maxPos = Mono_Log_Info_CC.MediumBoundH;
                    }
                    else if (logType == 2)
                    {
                        // Small
                        maxPos = Mono_Log_Info_CC.SmallBoundH;
                    }
                    else
                    {
                        // V Small
                        maxPos = Mono_Log_Info_CC.VerySmallBoundH;
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
            private void GetPlayerResult(EventData eventData)
            {
                if (eventData.Code == Constants.GetPlayerResultsEventCode)
                {
                    object[] data = (object[])eventData.CustomData;

                    PlayerResults<float> thisPlayersResult = new PlayerResults<float>();
                    thisPlayersResult.playerId = (int)data[0];
                    thisPlayersResult.scoring = (float)data[1];

                    _provisoryPlayerResults.Add(thisPlayersResult);
                }
            }

            private void SpawnLog(EventData eventData)
            {
                if (eventData.Code == Constants.NextLogWaveEventCode && PhotonNetwork.IsMasterClient)
                {
                    if (_wave < Constants.AMOUNT_OF_LOGS_PER_MATCH)
                    {
                        // Any wait time that is greater than the serverLag * 2 will mean instant spawn. Approx 100ms at most 500ms (very bad server ping (250))
                        float windowTime = _windowTime;

                        int logType = DecideLogType(_wave); // 0 -> Large, 1 -> Normal, 2 -> Small H, 3 -> Small V, 4 -> VSmall H, 5 -> VSmall V

                        float markAngle = DecideMarkAngle(logType);

                        float markPos = DecideMarkPosition(logType, markAngle);

                        _wave++;
                        Debug.Log("WAVE: " + _wave + ", TIME: " + InGameTime + " , WINDOW: " + _windowTime);

                        object[] content = new object[] { windowTime, logType, markPos, markAngle, PhotonNetwork.Time };
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        PhotonNetwork.RaiseEvent(Constants.SpawnLogEventCode, content, raiseEventOptions, SendOptions.SendReliable);
                    }
                    else
                    {
                        _wave++;
                    }
                }
                else if(eventData.Code == Constants.NextLogWaveEventCode)
                {
                    _wave++;
                }
            }

            [PunRPC]
            public void RPC_SendSliced(PhotonMessageInfo info)
            {
                Sliced.Add(info.Sender.ActorNumber);
            }
        }
    }
}