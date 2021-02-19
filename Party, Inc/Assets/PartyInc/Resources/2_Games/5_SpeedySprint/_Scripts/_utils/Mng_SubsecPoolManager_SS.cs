using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlayInc
{
    namespace SS
    {
        public class Mng_SubsecPoolManager_SS : MonoSingleton<Mng_SubsecPoolManager_SS>
        {
            [SerializeField] private Transform _subsectionHolder;
            public Transform SubsectionHolder
            {
                get
                {
                    return _subsectionHolder;
                }

                private set
                {
                    _subsectionHolder = value;
                }
            }

            [SerializeField] private GameObject[] _subsecPrefabsEasy;
            [SerializeField] private GameObject[] _subsecPrefabsMedium;
            [SerializeField] private GameObject[] _subsecPrefabsHard;

            private List<GameObject> _subsecs = new List<GameObject>();
            // Start is called before the first frame update

            /// <summary>
            /// Loads Sub-section prefabs.
            /// </summary>
            private void LoadAllSubsections()
            {
                var loadedEasy = Resources.LoadAll(Mng_GameManager_SS.SubsectionsPath + "Easy", typeof(GameObject)).Cast<GameObject>();
                List<GameObject> easy = new List<GameObject>();

                foreach(GameObject sub in loadedEasy)
                {
                    easy.Add(sub);
                }

                var loadedMedium = Resources.LoadAll(Mng_GameManager_SS.SubsectionsPath + "Medium", typeof(GameObject)).Cast<GameObject>();
                List<GameObject> med = new List<GameObject>();

                foreach (GameObject sub in loadedMedium)
                {
                    med.Add(sub);
                }

                var loadedHard = Resources.LoadAll(Mng_GameManager_SS.SubsectionsPath + "Hard", typeof(GameObject)).Cast<GameObject>();
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

                //TODO: See if this is particularly costy.
                foreach(GameObject sub in _subsecPrefabsEasy)
                {
                    GenerateSubsection(sub);
                    GenerateSubsection(sub);
                }
                foreach (GameObject sub in _subsecPrefabsMedium)
                {
                    GenerateSubsection(sub);
                    GenerateSubsection(sub);
                }
                foreach (GameObject sub in _subsecPrefabsHard)
                {
                    GenerateSubsection(sub);
                    GenerateSubsection(sub);
                }
            }

            private void OnDestroy()
            {
                Resources.UnloadUnusedAssets();
            }

            /// <summary>
            /// Creates a new sub-section of the kind.
            /// </summary>
            /// <param name="subsection"></param>
            private void GenerateSubsection(GameObject subsection)
            {
                GameObject newSubsec = Instantiate(subsection);
                newSubsec.transform.parent = _subsectionHolder.transform;
                newSubsec.SetActive(false);
                _subsecs.Add(newSubsec);
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
                GameObject found = null;
                foreach (GameObject sub in _subsecs)
                {
                    // If its what we are looking for
                    if(IsSubsection(sub.name, difficulty, number))
                    {
                        found = sub;
                        if (!sub.activeInHierarchy)
                        {
                            //Activate and return
                            sub.SetActive(true);
                            return sub;
                        }
                    }
                    
                }

                // If we found the sub-section but need more, we create another one.
                if (found != null)
                {
                    GenerateSubsection(found);
                }
                else
                {
                    throw new System.Exception("FATAL ERROR: Subsection not found in subsection pool. SubsectionPoolManager.");
                }
                    
                return RequestSubsection(difficulty, number);
            }

            /// <summary>
            /// Determines if its the subsection we are looking for
            /// </summary>
            /// <param name="name"></param>
            /// <param name="difficulty"></param>
            /// <param name="number"></param>
            /// <returns>True if its the sub-section</returns>
            private bool IsSubsection(string name, string difficulty, int number)
            {
                name = name.ToUpper();
                name = name.Replace("(CLONE)", "").Trim();
                string find = difficulty + number; //eg E1 or E15 or H2

                int lenF = find.Length;
                int lenN = name.Length;
                int letterPos = lenN - lenF;

                string compare = name.Substring(letterPos);

                return compare.Equals(find);
            }
        }
    }
}


