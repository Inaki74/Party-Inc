using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace FiestaTime
{
    namespace TT
    {
        public class GameManager : MonoSingleton<GameManager>
        {
            public static Vector3 forwardVector = new Vector3(0f, Mathf.Cos(45f), Mathf.Cos(45f));
            public static float maxDistance = 13f;
            public static float hazardMinimumVelocity = 13f;

            private Vector3[] playerPositions = new Vector3[4];

            public delegate void ActionGameStart();
            public static event ActionGameStart onGameStart;

            [SerializeField] private GameObject playerPrefab;
            [SerializeField] private GameObject uiPrefab;

            public float gameStartCountdown = 3f;

            public bool gameBegan;

            // Start is called before the first frame update
            void Start()
            {
                InitializePlayers();
                InitializeUI();
            }

            // Update is called once per frame
            void Update()
            {
                if (gameStartCountdown <= -1f)
                {
                    gameBegan = true;
                    gameStartCountdown = -1f;
                    onGameStart?.Invoke();
                }
                else
                {
                    gameStartCountdown -= Time.deltaTime;
                }
            }

            private void InitializeUI()
            {
                Instantiate(uiPrefab);
            }

            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    if(PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            /// <summary>
            /// Sets the player position vectors.
            /// </summary>
            private void SetPlayerPositions()
            {
                switch (PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 0.4f, -0.4f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-1.25f, 0.4f, -0.4f);
                        playerPositions[1] = new Vector3(1.25f, 0.4f, -0.4f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-2.5f, 0.4f, -0.4f);
                        playerPositions[1] = new Vector3(0f, 0.4f, -0.4f);
                        playerPositions[2] = new Vector3(2.5f, 0.4f, -0.4f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-2.5f, 0.4f, -0.4f);
                        playerPositions[1] = new Vector3(-0.85f, 0.4f, -0.4f);
                        playerPositions[2] = new Vector3(0.85f, 0.4f, -0.4f);
                        playerPositions[3] = new Vector3(2.5f, 0.4f, -0.4f);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}


