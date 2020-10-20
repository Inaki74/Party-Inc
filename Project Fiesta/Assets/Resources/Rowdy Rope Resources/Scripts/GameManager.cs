using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FiestaTime
{
    namespace RR
    {
        public class GameManager : MonoSingleton<GameManager>
        {
            private NetworkGameRoomController networkController;

            PlayerResults<int>[] playerResults;

            public delegate void ActionGameStart();
            public static event ActionGameStart onGameStart;

            public delegate void CheckpointReach(int checkpoint);
            public static event CheckpointReach onCheckpointReached;

            [SerializeField] private GameObject stagePrefab;
            [SerializeField] private GameObject playerPrefab;
            [SerializeField] private GameObject UIPrefab;

            public float startingSpeed;
            public float startingAngle;

            private int playerCount;
            private Vector3[] playerPositions = new Vector3[4];


            public float gameStartCountdown = 3f; //Have to take into account the start text
            public int movesForSpeedIncrease;
            public int thresholdInverse;
            public int thresholdBurst;
            public int thresholdPeak;
            public int thresholdDeath;

            private bool firstRun;

            public int currentJump;

            // Start is called before the first frame update
            void Start()
            {
                networkController = FindObjectOfType<NetworkGameRoomController>();

                playerCount = PhotonNetwork.PlayerList.Length;
                playerResults = new PlayerResults<int>[playerCount];
                firstRun = true;

                StartCoroutine("");
                InitializePlayers();
                InitializeStage();
                InitializeUI();
            }

            // Update is called once per frame
            void Update()
            {
                if(gameStartCountdown <= -1f && firstRun)
                {
                    gameStartCountdown = -1f;
                    firstRun = false;
                    onGameStart?.Invoke();
                }
                else
                {
                    gameStartCountdown -= Time.deltaTime;
                }


                //
            }

            public override void Init()
            {
                RopeControllerM.onLoopComplete += OnRoundCompleted;
            }

            private void OnDestroy()
            {
                RopeControllerM.onLoopComplete -= OnRoundCompleted;
            }

            private void InitializePlayers()
            {
                SetPlayerPositions();

                Vector3 decidedPosition = Vector3.zero;

                for(int i = 0; i < playerCount; i++)
                {
                    if(PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        decidedPosition = playerPositions[i];
                    }
                }

                PhotonNetwork.Instantiate(playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            private void InitializeStage()
            {
                //Instantiate(stagePrefab);
            }

            private void InitializeUI()
            {
                Instantiate(UIPrefab);
            }

            private void SetPlayerPositions()
            {
                switch (playerCount)
                {
                    case 1:
                        playerPositions[0] = new Vector3(1f, 1f, 10f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(1f, 1f, 8f);
                        playerPositions[1] = new Vector3(1f, 1f, 12f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(1f, 1f, 7f);
                        playerPositions[1] = new Vector3(1f, 1f, 10f);
                        playerPositions[2] = new Vector3(1f, 1f, 13f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(1f, 1f, 7f);
                        playerPositions[1] = new Vector3(1f, 1f, 9f);
                        playerPositions[2] = new Vector3(1f, 1f, 11f);
                        playerPositions[3] = new Vector3(1f, 1f, 13f);
                        break;
                    default:
                        break;
                }
            }

            private void CheckCheckpoints()
            {
                // Check currentJump
                // Here invoke the checkpoints
                if(currentJump % movesForSpeedIncrease == 0)
                {
                    onCheckpointReached?.Invoke(0);
                }

                if(currentJump == thresholdInverse)
                {
                    onCheckpointReached?.Invoke(1);
                }

                if(currentJump == thresholdBurst)
                {
                    onCheckpointReached?.Invoke(2);
                }

                if(currentJump == thresholdPeak)
                {
                    onCheckpointReached?.Invoke(3);
                }

                if(currentJump == thresholdDeath)
                {
                    onCheckpointReached?.Invoke(4);
                }
            }

            private void OnRoundCompleted()
            {
                currentJump += 1;

                CheckCheckpoints();
            }
        }
    }
}