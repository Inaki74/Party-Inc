using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace FiestaTime
{
    namespace CC
    {
        /// <summary>
        /// GAME LOOP:
        ///          Start
        /// ---------> |  ----->
        /// Countdown   
        /// </summary>

        public class GameManager : FiestaGameManager<GameManager, int>
        {
            public delegate void ActionSpawnLog(float p, float a, float t);
            public static event ActionSpawnLog onLogSpawned;

            [SerializeField] private GameObject _logSpawner;

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

                if (GameBegan)
                {
                    SpawnLog();
                }
            }

            private void SpawnLog()
            {
                if(PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
                {
                    Debug.Log("AA");
                    //EXAMPLE OF SENDING AN EVENT
                    //object[] content = new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; // Array contains the target position and the IDs of the selected units
                    //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
                    //PhotonNetwork.RaiseEvent(SpawnLogEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);

                    _currentTime += Time.deltaTime;

                    if(_currentTime > 4f)
                    {
                        if (PhotonNetwork.IsConnected)
                        {
                            object[] content = new object[] { };
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
                        decidedPosition = new Vector3(decidedPosition.x, 10f, -5f);
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
        }
    }
}


