using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace PlayInc
{
    namespace RR
    {
        /// <summary>
        /// The game manager class.
        /// </summary>
        public class GameManager : FiestaGameManager<GameManager, int>
        {
            [SerializeField] private RopeControllerM rope;

            private Player[] players;

            public float startingSpeed;
            public float startingAngle;

            #region Checkpoint Variables

            private int movesForSpeedIncrease = 4;
            private float speedIncreasePerMoves = 0.45f;
            private int thresholdInverseE = 10;
            private int thresholdInverseM = 25;
            private int thresholdInverseH = 50;
            private int thresholdBurst = 20;
            private int thresholdPeak = 40;
            private int thresholdDeath = 100;

            private bool chkpnt_Inverse;
            private bool chkpnt_Bursts;
            private bool chkpnt_Peak;

            #endregion

            private int playersAlive;
            private bool gameEnded = false;

            private int nextToInsert;

            public bool isHighScore;
            public int winnerId;

            public int currentJump;

            #region Inverse Mechanic Variables

            private int superiorBoundRandomInverse = 7;
            private int inferiorBoundRandomInverse = 5;
            private int randomInverse = 0;

            private float inferiorBoundAngleNormPos = 260f; // 260 easy
            private float superiorBoundAngleNormPos = 355f; //355 easy
            private float inferiorBoundAngleNormNeg = 160f;// 160 easy
            private float superiorBoundAngleNormNeg = 270f; // 270 easy

            private float inferiorBoundAngleDecPos = 20f;
            private float superiorBoundAngleDecPos = 60f;
            private float inferiorBoundAngleDecNeg = 120f;
            private float superiorBoundAngleDecNeg = 160f;

            #endregion

            #region Burst Mechanic Variables

            private int randomBurst = 0;
            private int superiorBoundRandomBurst = 7;
            private int inferiorBoundRandomBurst = 5;

            private int burstDuration;
            private int superiorBoundBurstDuration = 6;
            private int inferiorBoundBurstDuration = 4;

            private float decreaseBurstPossibility = 0.5f;

            #endregion

            #region Fiesta Overrides
            protected override void InStart()
            {
                GameBegan = false;
                playersAlive = playerCount;
                nextToInsert = playerCount - 1;
                players = new Player[playerCount];

                startingAngle = 45.1f;
                startingAngle = startingAngle * Mathf.Deg2Rad;
            }

            protected override void InitializeGameManagerDependantObjects()
            {
                InitializePlayers();
                InitializeStage();
                InitializeUI();
            }

            public override void Init()
            {
                base.Init();

                PhotonNetwork.SendRate = 15;
                PhotonNetwork.SerializationRate = 15;

                PhotonNetwork.NetworkingClient.EventReceived += OnRoundCompleted;
                Player.onPlayerDied += OnPlayerFinished;
            }

            #endregion

            #region Unity Callbacks
            private void OnDestroy()
            {
                PhotonNetwork.SendRate = 10;
                PhotonNetwork.SerializationRate = 10;

                PhotonNetwork.NetworkingClient.EventReceived -= OnRoundCompleted;
                Player.onPlayerDied -= OnPlayerFinished;
            }

            // Update is called once per frame
            void Update()
            {
                if (PhotonNetwork.IsConnectedAndReady && _startCountdown && !GameBegan)
                {
                    if (_startTime != 0 && (float)(PhotonNetwork.Time - _startTime) >= gameStartCountdown + 1f)
                    {
                        GameBegan = true;
                        players = FindObjectsOfType<Player>();
                        OnGameStartInvoke();
                    }
                }
                else if (_startCountdown && !GameBegan)
                {
                    if (gameStartCountdown <= -1f)
                    {
                        GameBegan = true;
                        gameStartCountdown = float.MaxValue;
                        players = FindObjectsOfType<Player>();
                        OnGameStartInvoke();
                    }
                    else
                    {
                        gameStartCountdown -= Time.deltaTime;
                    }
                }

                if (GameBegan)
                {
                    InGameTime += Time.deltaTime;
                }

                //Here go difficulty factors
                if (!gameEnded && PhotonNetwork.IsMasterClient)
                {
                    // Inversion mode activated
                    if (chkpnt_Inverse)
                    {
                        InverseMechanic();
                    }

                    if (chkpnt_Bursts)
                    {
                        BurstMechanic();
                    }
                }

                if (playerCount >= 1 && playersAlive <= 0 && !gameEnded && rope.loopCompleted)
                {
                    Debug.Log("Game should end");
                    gameEnded = true;
                    winnerId = FindWinner();
                    OnGameFinishInvoke();
                }
            }

            #endregion

            #region Initializations

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

                PhotonNetwork.Instantiate("_player/" + playerPrefab.name, decidedPosition, Quaternion.identity);
            }

            private void InitializeStage()
            {
                //Instantiate(stagePrefab);
                rope.enabled = true;
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
                        playerPositions[0] = new Vector3(0f, 1f, 10f);
                        break;
                    case 2:
                        playerPositions[0] = new Vector3(0f, 1f, 8f);
                        playerPositions[1] = new Vector3(0f, 1f, 12f);
                        break;
                    case 3:
                        playerPositions[0] = new Vector3(0f, 1f, 7f);
                        playerPositions[1] = new Vector3(0f, 1f, 10f);
                        playerPositions[2] = new Vector3(0f, 1f, 13f);
                        break;
                    case 4:
                        playerPositions[0] = new Vector3(0f, 1f, 7f);
                        playerPositions[1] = new Vector3(0f, 1f, 9f);
                        playerPositions[2] = new Vector3(0f, 1f, 11f);
                        playerPositions[3] = new Vector3(0f, 1f, 13f);
                        break;
                    default:
                        break;
                }
            }

            #endregion

            

            #region Mechanics

            /// <summary>
            /// The function that manages the rope inversion mechanic.
            /// </summary>
            private void InverseMechanic()
            {
                // We reached zero, time to inverse
                if(randomInverse == 0)
                {
                    // Regenerate the random inverse number
                    randomInverse = Random.Range(inferiorBoundRandomInverse, superiorBoundRandomInverse + 1);

                    // Decide where to inverse
                    float whereToInverse = 0f;
                    // The a number decides if its a deceiving inverse or a normal inverse
                    float a = Random.Range(0f, 1f);

                    if (rope.rotationSpeed > 0f)
                    {
                       if(a < 0.75f)
                        {
                            // Normal inverse
                            whereToInverse = Random.Range(inferiorBoundAngleNormPos, superiorBoundAngleNormPos);
                        }
                        else
                        {
                            // Deceiving inverse
                            whereToInverse = Random.Range(inferiorBoundAngleDecPos, superiorBoundAngleDecPos);
                        }
                    }else if(rope.rotationSpeed < 0f)
                    {
                        if (a < 0.75f)
                        {
                            whereToInverse = Random.Range(inferiorBoundAngleNormNeg, superiorBoundAngleNormNeg);
                        }
                        else
                        {
                            whereToInverse = Random.Range(inferiorBoundAngleDecNeg, superiorBoundAngleDecNeg);
                        }
                    }

                    // Inverse
                    rope.InvertRope(whereToInverse * Mathf.Deg2Rad);
                }
            }

            /// <summary>
            /// The function that manages the rope burst speeds mechanic.
            /// </summary>
            private void BurstMechanic()
            {
                if(randomBurst == 0)
                {
                    burstDuration = Random.Range(inferiorBoundBurstDuration, superiorBoundBurstDuration + 1);
                    randomBurst = Random.Range(inferiorBoundRandomBurst + burstDuration, superiorBoundRandomBurst + burstDuration + 1);

                    float howMuchBurst = 0f;
                    float a = Random.Range(0f, 1f);
                    if(decreaseBurstPossibility < 0f)
                    {
                        decreaseBurstPossibility = 0.01f;
                    }

                    if(rope.rotationSpeed > 0f)
                    {
                        howMuchBurst = 1f;
                        //if (a > decreaseBurstPossibility)
                        //{
                        //    //Increase
                        //    decreaseBurstPossibility += 0.1f;
                        //    howMuchBurst = 1f;
                        //}
                        //else
                        //{
                        //    //Decrease
                        //    decreaseBurstPossibility -= 0.1f;
                        //    howMuchBurst = -1f;
                        //}
                    }else if (rope.rotationSpeed < 0f)
                    {
                        howMuchBurst = -1f;
                        //if (a > decreaseBurstPossibility)
                        //{
                        //    decreaseBurstPossibility += 0.1f;
                        //    howMuchBurst = 1f;
                        //}
                        //else
                        //{
                        //    decreaseBurstPossibility -= 0.1f;
                        //    howMuchBurst = -1f;
                        //}
                    }

                    rope.BurstRope(howMuchBurst);
                    StartCoroutine(ResetBurstCo(-howMuchBurst, currentJump + burstDuration));
                }
            }

            /// <summary>
            /// A co-routine that waits until enough rounds have happened to reset the burst speed change.
            /// </summary>
            private IEnumerator ResetBurstCo(float amount, int duration)
            {
                Debug.Log("Waiting for reset");
                yield return new WaitUntil(() => currentJump >= duration);
                Debug.Log("BURST RESET!");

                rope.BurstRope(amount);
            }

            #endregion

            #region Game Results Functions
            /// <summary>
            /// Finds the winner of the game.
            /// </summary>
            /// <returns></returns>
            private int FindWinner()
            {
                foreach (Player p in players)
                {
                    // If the player isnt in the list, its because:
                    // It never lost
                    // It lost but got through the sync somehow.
                    AddPlayerResult(p.photonView.Owner.ActorNumber, currentJump);
                }

                PlayerResultsHelper.PrintArray(playerResults);
                return CheckWinner();
            }

            /// <summary>
            /// Ultimately checks the winner.
            /// </summary>
            /// <returns></returns>
            private int CheckWinner()
            {
                var ordered = playerResults.OrderByDescending(p => p.scoring);
                PlayerResults<int>[] o = ordered.ToArray();

                for (int i = 1; i < o.Length; i++)
                {
                    if (o[0].Equals(o[1]))
                    {
                        return -1;
                    }
                }

                return o[0].playerId;
            }

            /// <summary>
            /// Checks if a player result is in the list.
            /// </summary>
            /// <param name="playerId"></param>
            /// <returns></returns>
            private bool PlayerResultInList(int playerId)
            {
                // Cant use LINQ contains, the EQUALS is already defined with score

                foreach(var p in playerResults)
                {
                    if (p.playerId == playerId) return true;
                }

                return false;
            }

            /// <summary>
            /// Adds player results to the list.
            /// </summary>
            /// <param name="playerId"></param>
            /// <param name="score"></param>
            private void AddPlayerResult(int playerId, int score)
            {
                // Safeguard, just in case someone accessed off sync.
                if (PlayerResultInList(playerId)) return;

                Debug.Log("Added results of: " + playerId + " of score: " + score);
                PlayerResults<int> thisPlayerResult = new PlayerResults<int>();
                thisPlayerResult.playerId = playerId;
                thisPlayerResult.scoring = score;

                playerResults[nextToInsert] = thisPlayerResult;
                nextToInsert--;
                playersAlive--;

                if (playerId == PhotonNetwork.LocalPlayer.ActorNumber) isHighScore = GeneralHelperFunctions.DetermineHighScoreInt(PlayInc.Constants.RR_KEY_HISCORE, thisPlayerResult.scoring, true);
            }

            #endregion

            #region Event Functions

            private void OnRoundCompleted(EventData eventData)
            {
                if(eventData.Code == Constants.OnLoopEventCode && playersAlive > 0)
                {
                    currentJump += 1;

                    UIManager.Current.OnRoundCompleted();

                    Debug.Log("CURRENT JUMP: " + currentJump + ", CURRENT SPEED: " + rope.rotationSpeed + ", AT TIME: " + InGameTime);

                    if (!PhotonNetwork.IsMasterClient) return;

                    CheckCheckpoints();
                }
            }

            private void OnPlayerFinished(int playerId, int score)
            {
                Debug.Log("Player " + playerId + " finished.");
                AddPlayerResult(playerId, score);
                // Give the result to the other players
                photonView.RPC("RPC_SendPlayerResult", RpcTarget.Others, playerId, score);
            }

            private void CheckCheckpoints()
            {
                // Check currentJump
                // Here invoke the checkpoints
                if (currentJump % movesForSpeedIncrease == 0)
                {
                    if (!chkpnt_Peak)
                    {
                        // Increase speed
                        rope.rotationSpeed += speedIncreasePerMoves * Mathf.Sign(rope.rotationSpeed);
                    }
                }


                if (randomInverse != 0)
                {
                    randomInverse--;
                }

                if (randomBurst != 0)
                {
                    randomBurst--;
                }

                if (currentJump >= thresholdInverseE)
                {
                    chkpnt_Inverse = true;
                }

                if (currentJump >= thresholdBurst)
                {
                    chkpnt_Bursts = true;
                }

                if (currentJump == thresholdInverseM)
                {
                    superiorBoundRandomInverse = 8;
                    inferiorBoundRandomInverse = 6;
                    inferiorBoundAngleNormPos = 240f; // 240 medium
                    superiorBoundAngleNormPos = 290f; // 290 medium
                    inferiorBoundAngleNormNeg = 250f;// 250 medium
                    superiorBoundAngleNormNeg = 320f; // 320 medium
                }

                if (currentJump == thresholdPeak)
                {
                    chkpnt_Peak = true;
                    rope.rotationSpeed += 0.5f * Mathf.Sign(rope.rotationSpeed);
                }

                if (currentJump == thresholdInverseH)
                {
                    superiorBoundRandomInverse = 9;
                    inferiorBoundRandomInverse = 7;
                    inferiorBoundAngleNormPos = 180f; // 180 hard
                    superiorBoundAngleNormPos = 210f; //210 hard
                    inferiorBoundAngleNormNeg = 340f;// 340 hard
                    superiorBoundAngleNormNeg = 359.9f; // 359.9 hard
                }

                if (currentJump == thresholdDeath)
                {
                    rope.rotationSpeed += 0.5f * Mathf.Sign(rope.rotationSpeed);
                }
            }

            #endregion

            public bool MyPlayerHasLost()
            {
                foreach (var p in players)
                {
                    if (p.photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        return p.hasLost;
                    }
                }

                return true;
            }

            #region RPCs

            [PunRPC]
            public void RPC_SendPlayerResult(int playerId, int score)
            {
                Debug.Log("Player " + playerId + " finished over the net.");
                AddPlayerResult(playerId, score);
            }

            #endregion
        }
    }
}