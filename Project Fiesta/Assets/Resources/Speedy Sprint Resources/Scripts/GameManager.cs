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

            [SerializeField] private GameObject _generatorPrefab;
            [SerializeField] private GameObject _gameCamera;

            public static string SubsectionsPath = "Speedy Sprint Resources/Prefabs/Resources/Sub-Sections/";

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializeProceduralGenerator();

                InitializePlayers();
            }

            protected override void InStart()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(0f, 1f, 20f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(-5f, 1f, 20f);
                        playerPositions[1] = new Vector3(5f, 1f, 20f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(-10f, 1f, 20f);
                        playerPositions[1] = new Vector3(0f, 1f, 20f);
                        playerPositions[2] = new Vector3(10f, 1f, 20f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(-15f, 1f, 20f);
                        playerPositions[1] = new Vector3(-5f, 1f, 20f);
                        playerPositions[2] = new Vector3(5f, 1f, 20f);
                        playerPositions[3] = new Vector3(15f, 1f, 20f);
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

            private void InitializeProceduralGenerator()
            {
                GameObject generator = PhotonNetwork.InstantiateRoomObject(_generatorPrefab.name, new Vector3(15f, 17f, 7f), Quaternion.identity);

                _gameCamera.GetComponent<CinemachineVirtualCamera>().Follow = generator.transform;
            }
        }
    }
}