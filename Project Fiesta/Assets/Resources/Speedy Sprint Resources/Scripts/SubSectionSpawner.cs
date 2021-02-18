using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FiestaTime
{
    namespace SS
    {
        public class SubSectionSpawner : MonoSingleton<SubSectionSpawner>
        {
            [SerializeField] private Transform _subsectionHolder;

            [SerializeField] private GameObject[] _subsecPrefabsEasy;
            [SerializeField] private GameObject[] _subsecPrefabsMedium;
            [SerializeField] private GameObject[] _subsecPrefabsHard;
            // Start is called before the first frame update

            /// <summary>
            /// Loads Sub-section prefabs.
            /// </summary>
            private void LoadAllSubsections()
            {
                var loadedEasy = Resources.LoadAll(GameManager.SubsectionsPath + "Easy", typeof(GameObject)).Cast<GameObject>();
                List<GameObject> easy = new List<GameObject>();

                foreach (GameObject sub in loadedEasy)
                {
                    easy.Add(sub);
                }

                var loadedMedium = Resources.LoadAll(GameManager.SubsectionsPath + "Medium", typeof(GameObject)).Cast<GameObject>();
                List<GameObject> med = new List<GameObject>();

                foreach (GameObject sub in loadedMedium)
                {
                    med.Add(sub);
                }

                var loadedHard = Resources.LoadAll(GameManager.SubsectionsPath + "Hard", typeof(GameObject)).Cast<GameObject>();
                List<GameObject> hard = new List<GameObject>();

                foreach (GameObject sub in loadedHard)
                {
                    hard.Add(sub);
                }

                _subsecPrefabsEasy = easy.ToArray();
                _subsecPrefabsMedium = med.ToArray();
                _subsecPrefabsHard = hard.ToArray();
            }

            void Start()
            {
                LoadAllSubsections();
            }

            private void OnDestroy()
            {
                Resources.UnloadUnusedAssets();
            }

            /// <summary>
            /// Get subsection requested.
            /// </summary>
            /// <param name="difficulty"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public GameObject RequestSubsection(string difficulty, int number)
            {
                //Get Subsection
                GameObject newSubsec = null;
                switch (difficulty)
                {
                    case "E":
                        newSubsec = Instantiate(_subsecPrefabsEasy[number - 1]);
                        break;
                    case "M":
                        newSubsec = Instantiate(_subsecPrefabsMedium[number - 1]);
                        break;
                    case "H":
                        newSubsec = Instantiate(_subsecPrefabsHard[number - 1]);
                        break;
                    default:
                        Debug.LogError("FiestaTime/SS/SubSectionSpawner: Wrong difficulty arrived");
                        break;
                }

                // If we found the sub-section but need more, we create another one.
                if (newSubsec == null)
                {
                    throw new System.Exception("FATAL ERROR: Subsection not found in subsection pool. SubSectionSpawner.");
                }

                newSubsec.transform.parent = _subsectionHolder.transform;

                return newSubsec;
            }
        }
    }
}


