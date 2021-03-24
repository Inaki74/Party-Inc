using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_GameResults : MonoBehaviour
        {
            private Mono_GameMetadata _gameData;
            //private FiestaGameManager gm;

            [Header("Players in the list, in order")]
            [SerializeField] private GameObject[] players;
            [SerializeField] private GameObject highScores;

            [SerializeField] private Text highScore;
            [SerializeField] private Text flavourTitle;
            [SerializeField] private Text _gameNameText;

            [Header("Players in the list, in order")]
            [SerializeField] private Text[] playerPlacings;
            [SerializeField] private Text[] playerNames;
            [SerializeField] private Text[] playerScores;

            // Start is called before the first frame update
            void Start()
            {
                _gameNameText.text = _gameData.GameDisplayName;

                // Set active for the amount of players playing.
                for (int i = 0; i < _gameData.PlayerCount; i++)
                {
                    players[i].SetActive(true);
                }

                if (_gameData.PlayerCount == 1)
                {
                    highScores.SetActive(true);
                    
                    playerNames[0].text = PhotonNetwork.LocalPlayer.NickName;
                    playerPlacings[0].enabled = false;
                    if(_gameData.ScoreType == ScoreType.Float)
                    {
                        playerScores[0].text = _gameData.PlayerResultsFloat[0].scoring.ToString();
                        highScore.text = _gameData.LocalPlayerHighscoreFloat.ToString();
                    }
                    else
                    {
                        playerScores[0].text = _gameData.PlayerResultsInt[0].scoring.ToString();
                        highScore.text = _gameData.LocalPlayerHighscoreInt.ToString();
                    }


                    if (_gameData.WasLocalPlayerHighscore)
                    {
                        flavourTitle.text = "CONGRATULATIONS! New high score!";
                        highScore.color = Color.yellow;
                    }
                    else
                    {
                        flavourTitle.text = "You'll get that high score someday!";
                    }
                }
                else
                {
                    // Set the list with winners and losers
                    if(_gameData.ScoreType == ScoreType.Int)
                    {
                        SetPositionsListInt();
                    }
                    else
                    {
                        SetPositionsListFloat();
                    }

                    // Set flavour text
                    if (_gameData.WinnerId == -1)
                    {
                        flavourTitle.text = "Unbelievable! Its a draw!";
                    }
                    else
                    {
                        if (_gameData.WinnerId == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            flavourTitle.text = "CONGRATULATIONS! You win!";
                        }
                        else
                        {
                            flavourTitle.text = "Welp... Can't have them all buddy.";
                        }
                    }
                }
            }

            private void Awake()
            {
                int openScenes = SceneManager.sceneCount;
                Scene[] allScenes = new Scene[openScenes];

                for (int i = 0; i < openScenes; i++)
                {
                    allScenes[i] = SceneManager.GetSceneAt(i);
                }

                foreach (Scene s in allScenes)
                {
                    GameObject[] sObjects = s.GetRootGameObjects();
                    foreach(GameObject go in sObjects)
                    {
                        Mono_GameMetadata data = go.GetComponent<Mono_GameMetadata>();
                        if (data != null)
                        {
                            _gameData = data;

                            break;
                        }
                    }
                }
            }

            private void SetPositionsListInt()
            {
                PlayerResults<int>[] aux = new PlayerResults<int>[4];
                if (_gameData.DescendingCondition)
                {
                    var aux2 = _gameData.PlayerResultsInt.OrderByDescending(r => r.scoring);
                    aux = aux2.ToArray();
                }
                else
                {
                    var aux2 = _gameData.PlayerResultsInt.OrderBy(r => r.scoring);
                    aux = aux2.ToArray();
                }

                int place = 0;
                int lastScore = -1;
                for (int i = 0; i < aux.Length; i++)
                {
                    if (aux[i].scoring != lastScore)
                    {
                        place++;
                    }
                    lastScore = aux[i].scoring;

                    switch (place)
                    {
                        case 1:
                            playerPlacings[i].text = "1st";
                            break;
                        case 2:
                            playerPlacings[i].text = "2nd";
                            break;
                        case 3:
                            playerPlacings[i].text = "3rd";
                            break;
                        case 4:
                            playerPlacings[i].text = "4th";
                            break;
                        default:
                            Debug.Log("FinishScreenUI, result not possible");
                            break;
                    }

                    playerScores[i].text = aux[i].scoring.ToString();

                    SetPlayerNamesAndFindMine(i, aux[i].playerId);
                }
            }

            private void SetPositionsListFloat()
            {
                PlayerResults<float>[] aux = new PlayerResults<float>[4];
                if (_gameData.DescendingCondition)
                {
                    aux = _gameData.PlayerResultsFloat.OrderByDescending(r => r.scoring).ToArray();
                }
                else
                {
                    aux = _gameData.PlayerResultsFloat.OrderBy(r => r.scoring).ToArray();
                }

                int place = 0;
                float lastScore = -1;
                for (int i = 0; i < aux.Length; i++)
                {
                    if (aux[i].scoring != lastScore)
                    {
                        place++;
                    }
                    lastScore = aux[i].scoring;

                    switch (place)
                    {
                        case 1:
                            playerPlacings[i].text = "1st";
                            break;
                        case 2:
                            playerPlacings[i].text = "2nd";
                            break;
                        case 3:
                            playerPlacings[i].text = "3rd";
                            break;
                        case 4:
                            playerPlacings[i].text = "4th";
                            break;
                        default:
                            Debug.Log("FinishScreenUI, result not possible");
                            break;
                    }

                    playerScores[i].text = aux[i].scoring.ToString();

                    SetPlayerNamesAndFindMine(i, aux[i].playerId);
                }
            }

            private void SetPlayerNamesAndFindMine(int i, int playerId)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (playerId == player.ActorNumber)
                    {
                        playerNames[i].text = player.NickName;
                    }
                }

                if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    playerNames[i].color = Color.yellow;
                    playerPlacings[i].color = Color.yellow;
                    playerScores[i].color = Color.yellow;
                }
            }

            public void BtnExitToMainMenu()
            {
                PhotonNetwork.LeaveRoom();

                SceneManager.LoadScene(Stt_SceneIndexes.HUB);
            }
        }
    }
}


