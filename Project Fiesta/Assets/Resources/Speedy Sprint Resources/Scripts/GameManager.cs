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
            [SerializeField] private GameObject _proceduralGenerator;

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
                if (PhotonNetwork.IsMasterClient)
                {
                    _proceduralGenerator = PhotonNetwork.Instantiate(_generatorPrefab.name, new Vector3(15f, 17f, 7f), Quaternion.identity);
                    _gameCamera.GetComponent<CinemachineVirtualCamera>().Follow = _proceduralGenerator.transform;
                }
                else
                {
                    _gameCamera.GetComponent<CinemachineVirtualCamera>().Follow = FindProceduralGenerator().transform;
                }
            }

            private GameObject FindProceduralGenerator()
            {
                GameObject[] aux = FindObjectsOfType<GameObject>();

                foreach (GameObject a in aux)
                {
                    if (a.CompareTag("ProceduralGenerator"))
                    {
                        return a;
                    }
                }

                return null;
            }

        }
    }
}