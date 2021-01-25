using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace CC
    {
        public class GameManager : FiestaGameManager<GameManager, int>
        {
            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
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
                        // Start game
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

            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 0f, 0f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(0f, 0f, 0f);
                        playerPositions[1] = new Vector3(0f, 0f, 0f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(0f, 0f, 0f);
                        playerPositions[1] = new Vector3(0f, 0f, 0f);
                        playerPositions[2] = new Vector3(0f, 0f, 0f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(0f, 0f, 0f);
                        playerPositions[1] = new Vector3(0f, 0f, 0f);
                        playerPositions[2] = new Vector3(0f, 0f, 0f);
                        playerPositions[3] = new Vector3(0f, 0f, 0f);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}


