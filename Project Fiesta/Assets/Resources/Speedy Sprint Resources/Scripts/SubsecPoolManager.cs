using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        public class SubsecPoolManager : MonoSingleton<SubsecPoolManager>
        {
            [SerializeField] private Transform _subsectionHolder;

            [SerializeField] private GameObject[] _subsecPrefabsEasy;
            [SerializeField] private GameObject[] _subsecPrefabsMedium;
            [SerializeField] private GameObject[] _subsecPrefabsHard;

            

            private List<GameObject> _subsecs = new List<GameObject>();
            // Start is called before the first frame update
            void Start()
            {
                //Resources.Load(decided, typeof(Array2DInt)) as Array2DInt;
                _subsecPrefabsEasy = Resources.LoadAll(GameManager.SubsectionsPath + "Easy") as GameObject[];
                _subsecPrefabsMedium = Resources.LoadAll(GameManager.SubsectionsPath + "Medium") as GameObject[];
                _subsecPrefabsHard = Resources.LoadAll(GameManager.SubsectionsPath + "Hard") as GameObject[];

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
                string find = difficulty + number; //eg E1 or E15 or H2

                return name.Contains(find);
            }
        }
    }
}


