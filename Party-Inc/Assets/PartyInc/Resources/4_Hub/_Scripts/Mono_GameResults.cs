using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_GameResults : MonoBehaviour
        {
            private Mono_GameMetadata _gameData;

            [Header("Players in the list, in order")]
            [SerializeField] private GameObject[] players;
            [SerializeField] private GameObject highScores;

            [SerializeField] private Text highScore;
            [SerializeField] private Text flavourTitle;

            [Header("Players in the list, in order")]
            [SerializeField] private Text[] playerPlacings;
            [SerializeField] private Text[] playerNames;
            [SerializeField] private Text[] playerScores;

            // Start is called before the first frame update
            void Start()
            {

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

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}


