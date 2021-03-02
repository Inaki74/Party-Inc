using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace AS
    {
        public class Mng_ObstaclePoolManager_AS : MonoSingleton<Mng_ObstaclePoolManager_AS>
        {
            [SerializeField] private Transform _obsHolder;
            public Transform ObsHolder
            {
                get
                {
                    return _obsHolder;
                }

                private set
                {
                    _obsHolder = value;
                }
            }

            [SerializeField] private GameObject _obsPrefab;
            private List<GameObject> _obstacles = new List<GameObject>();

            // Start is called before the first frame update
            void Start()
            {
                for (int i = 0; i < 7; i++)
                {
                    GenerateObstacle();
                }
            }

            /// <summary>
            /// Creates a new log.
            /// </summary>
            /// <param name="subsection"></param>
            private void GenerateObstacle()
            {
                GameObject newObs = Instantiate(_obsPrefab, new Vector3(-20f, 20f, 0f), Quaternion.identity);
                //newLog.GetComponent<PhotonView>().TransferOwnership(0);
                newObs.transform.parent = _obsHolder.transform;
                newObs.SetActive(false);
                _obstacles.Add(newObs);
            }

            /// <summary>
            /// Get requested log
            /// </summary>
            /// <param name="difficulty"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public GameObject RequestObstacle()
            {
                //Get log
                foreach (GameObject sub in _obstacles)
                {
                    if (!sub.activeInHierarchy)
                    {
                        //Activate and return
                        sub.SetActive(true);
                        return sub;
                    }
                }

                // If we get here, its because we haven't got the log
                GenerateObstacle();
                return RequestObstacle();
            }
        }
    }
}