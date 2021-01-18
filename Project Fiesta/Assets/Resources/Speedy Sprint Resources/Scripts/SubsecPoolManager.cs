using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FiestaTime
{
    namespace SS
    {
        public class SubsecPoolManager : MonoSingleton<SubsecPoolManager>
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

            private void LoadAllSubsections()
            {
                var loadedEasy = Resources.LoadAll(GameManager.SubsectionsPath + "Easy", typeof(GameObject)).Cast<GameObject>();
                List<GameObject> easy = new List<GameObject>();

                foreach(GameObject sub in loadedEasy)
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
                //Resources.Load(decided, typeof(Array2DInt)) as Array2DInt;

                LoadAllSubsections();

                
                ///StartCoroutine("");

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

            private void GenerateSubsection(GameObject subsection)
            {
                //PhotonNetwork.Instantiate(_tilePrefab.name, new Vector3(0, 13f, 0), Quaternion.identity);
                GameObject newSubsec = Instantiate(subsection);
                newSubsec.transform.parent = _subsectionHolder.transform;
                newSubsec.SetActive(false);
                _subsecs.Add(newSubsec);
            }

            public GameObject RequestSubsection(string difficulty, int number)
            {
                //Get Tile
                GameObject found = null;
                foreach (GameObject sub in _subsecs)
                {
                    if(IsSubsection(sub.name, difficulty, number))
                    {
                        found = sub;
                        if (!sub.activeInHierarchy)
                        {
                            sub.SetActive(true);
                            return sub;
                        }
                    }
                    
                }

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


